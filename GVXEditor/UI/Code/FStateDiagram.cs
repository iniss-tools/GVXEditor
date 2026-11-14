using System.Globalization;
using GVXEditor.Domain;
using GVXEditor.Domain.Code;
using GVXEditor.Domain.Code.Drafts;
using GVXEditor.Domain.Rules;
using GVXEditor.Domain.Views;
using GVXEditor.Properties;
using ToolsCore.CodeEditor;
using ToolsCore.Tools;

namespace GVXEditor.UI.Code;

/// <summary>
/// Okno Stavovy diagram: grafy a stavy stanice (statedgm.xml) s vyrazmi VB.NET. Pravidla sa kontroluju pri kazdej zmene,
/// kod prelozi server cez most chvilu po poslednej zmene a chyby podciarkne vo vyraze.
/// </summary>
internal partial class FStateDiagram : Form
{
    private readonly EditorContext _ctx;
    private readonly Iniss2Workspace _workspace;
    private readonly GroupDocument _group;
    private readonly int _groupIndex;
    private readonly Dictionary<int, StateDiagramEdit> _edits = [];
    private readonly CodeCheck _check;
    private readonly ToolTip _flagTip = new();
    private readonly List<Action?> _targets = [];
    private CodeCheckResult? _result;
    private StateDiagramEdit? _edit;
    private bool _loading;

    /// <summary>Stanica v ponuke okna.</summary>
    private sealed record StationChoice(int Index, string Label)
    {
        public override string ToString() => Label;
    }

    /// <summary>Variant hlasenia v ponuke automatiky.</summary>
    private sealed record VariantChoice(int? Variant, string Label)
    {
        public override string ToString() => Label;
    }

    /// <param name="context">kontext editora (otvorena instalacia)</param>
    /// <param name="groupIndex">poradie grafikonu</param>
    /// <param name="station">poradie stanice, ktora sa ma ukazat</param>
    public FStateDiagram(EditorContext context, int groupIndex, int station)
    {
        ArgumentNullException.ThrowIfNull(context);
        _ctx = context;
        _workspace = context.Workspace ?? throw new InvalidOperationException("Instalacia nie je otvorena.");
        _groupIndex = groupIndex;
        _group = _workspace.Groups[groupIndex];
        InitializeComponent();
        this.ApplyThemeAndFonts();
        Text = string.Format(CultureInfo.CurrentCulture, "{0} {1}", Resources.Page_StateDiagram, _group.Group.Name);

        foreach (var box in new[] { codeIndex, codeTime, codeMode, codeRepeat, codeEnd })
            box.SingleLine = true;
        CodeWindow.Prepare(context, this, [], codeIndex, codeCondition, codeTime, codeAutoCondition, codeMode, codeRepeat, codeEnd);
        cboVariant.DataSource = new List<VariantChoice>
        {
            new(null, Resources.Code_VariantAuto), new(1, Resources.Code_VariantShort), new(2, Resources.Code_VariantLong)
        };
        // priznaky slovami s klucom; do suboru ide kluc
        clbAutoFlags.Items.AddRange([.. CodeRules.AutomationFlags.Select(Flag)]);

        _check = new CodeCheck(_workspace, this, () => _workspace.History.Current.With(_groupIndex, Apply(_group)), ShowResult);
        Disposed += (_, _) =>
        {
            _check.Dispose();
            _flagTip.Dispose();
        };

        _loading = true;
        cboStation.DataSource = _group.Stations.Select((document, index) =>
            new StationChoice(index, string.Create(CultureInfo.CurrentCulture, $"{document.Station.Directory} ({_workspace.StationName(document.Station.Station)})"))).ToList();
        if (station >= 0 && station < _group.Stations.Count)
            cboStation.SelectedIndex = station;
        _loading = false;
        ShowStation();
        _check.Schedule();
    }

    /// <summary>
    /// Grafikon so stavovymi diagramami z okna; bez zmeny vrati <paramref name="group" />.
    /// </summary>
    public GroupDocument Apply(GroupDocument group)
    {
        ArgumentNullException.ThrowIfNull(group);
        var result = group;
        foreach (var edit in _edits.Values)
        {
            if (group.Stations[edit.Index].Code is not { } code)
                continue;
            var diagram = edit.Apply();
            if (!ReferenceEquals(diagram, code.Diagram))
                result = result.WithStationCode(edit.Index, code with { Diagram = diagram });
        }

        return result;
    }

    private StateDiagramEdit EditOf(int index)
    {
        if (!_edits.TryGetValue(index, out var edit))
            _edits[index] = edit = StateDiagramEdit.Of(index, _group.Stations[index].Code?.Diagram);
        return edit;
    }

    private void cboStation_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (!_loading)
            ShowStation();
    }

    private void ShowStation()
    {
        var choice = cboStation.SelectedItem as StationChoice;
        var document = choice is null ? null : _group.Stations[choice.Index];
        _edit = choice is null || document?.Code is null ? null : EditOf(choice.Index);
        lStationInfo.Text = document?.Code is null && document is not null
            ? string.Format(CultureInfo.CurrentCulture, Resources.Code_LoadFailed, document.CodeError)
            : "";

        _loading = true;
        codeIndex.Code = _edit?.GraphIndex ?? "";
        // hlasenia s pismenom z typov hlaseni stanice, potom ostatne funkcie skriptu hlaseni
        cboReport.Items.Clear();
        var keys = document?.Setup?.ReportKeys.Select(key => key.Name).ToList() ?? [];
        foreach (var name in keys.Concat((_workspace.Functions?.Keys ?? []).Where(name => !keys.Contains(name)).Order(StringComparer.CurrentCulture)))
            cboReport.Items.Add(name);
        _loading = false;
        pIndex.Enabled = splitMain.Enabled = _edit is not null;
        FillTree(null);
        Recheck();
    }

    // strom grafov a stavov; vyberie danu polozku (graf alebo stav)
    private void FillTree(object? select)
    {
        tvGraphs.BeginUpdate();
        tvGraphs.Nodes.Clear();
        TreeNode? selected = null;
        foreach (var graph in _edit?.Graphs ?? [])
        {
            var graphNode = new TreeNode(GraphText(graph)) { Tag = graph };
            foreach (var node in graph.Nodes)
            {
                var child = new TreeNode(NodeText(node)) { Tag = node };
                graphNode.Nodes.Add(child);
                if (ReferenceEquals(node, select))
                    selected = child;
            }

            tvGraphs.Nodes.Add(graphNode);
            if (ReferenceEquals(graph, select))
                selected = graphNode;
        }

        tvGraphs.ExpandAll();
        tvGraphs.EndUpdate();
        tvGraphs.SelectedNode = selected ?? (tvGraphs.Nodes.Count > 0 ? tvGraphs.Nodes[0] : null);
        ShowSelected();
    }

    private static string GraphText(GraphDraft graph) => graph.Description.Length > 0 ? graph.Description : "?";

    private static string NodeText(NodeDraft node) => node.Report.Length > 0 && node.Report != node.Key ? $"{node.Key} ({node.Report})" : node.Key;

    private void tvGraphs_AfterSelect(object? sender, TreeViewEventArgs e) => ShowSelected();

    private NodeDraft? SelectedNode => tvGraphs.SelectedNode?.Tag as NodeDraft;

    private GraphDraft? SelectedGraph => tvGraphs.SelectedNode?.Tag switch
    {
        GraphDraft graph => graph,
        NodeDraft => tvGraphs.SelectedNode.Parent?.Tag as GraphDraft,
        _ => null
    };

    private void ShowSelected()
    {
        var node = SelectedNode;
        var graph = tvGraphs.SelectedNode?.Tag as GraphDraft;
        pNode.Visible = node is not null;
        pGraph.Visible = graph is not null;
        _loading = true;
        if (graph is not null)
            tbGraphDescription.Text = graph.Description;
        if (node is not null)
            ShowNode(node);
        _loading = false;
        UpdateButtons();
        ShowIssues();
    }

    private void ShowNode(NodeDraft node)
    {
        tbKey.Text = node.Key;
        tbName.Text = node.Name;
        cboReport.Text = node.Report;
        clbFlags.Items.Clear();
        foreach (var flag in CodeRules.NodeFlags.Concat(node.Flags.Where(flag => !CodeRules.NodeFlags.Contains(flag))))
            clbFlags.Items.Add(Flag(flag), node.Flags.Contains(flag));
        codeCondition.Code = node.Condition;
        cboxButton.Checked = node.HasButton;
        tbButtonText.Text = node.ButtonText;
        tbButtonPosition.Text = node.ButtonPosition;
        cboxAutomation.Checked = node.HasAutomation;
        codeTime.Code = node.Time;
        codeAutoCondition.Code = node.AutomationCondition;
        codeMode.Code = node.Mode;
        codeRepeat.Code = node.Repeat;
        codeEnd.Code = node.End;
        cboVariant.SelectedIndex = node.Variant switch { 1 => 1, 2 => 2, _ => 0 };
        for (var i = 0; i < clbAutoFlags.Items.Count; i++)
            clbAutoFlags.SetItemChecked(i, node.AutomationFlags.Contains(((ValueChoice)clbAutoFlags.Items[i]).Key));
        foreach (var flag in node.AutomationFlags.Where(flag => !clbAutoFlags.Items.Cast<ValueChoice>().Any(item => item.Key == flag)))
            clbAutoFlags.Items.Add(Flag(flag), true);
        EnableNodeParts();
    }

    private void EnableNodeParts()
    {
        tbButtonText.Enabled = tbButtonPosition.Enabled = cboxButton.Checked;
        foreach (Control control in new Control[] { codeTime, codeAutoCondition, codeMode, codeRepeat, codeEnd, cboVariant, clbAutoFlags })
            control.Enabled = cboxAutomation.Checked;
    }

    private void Node_Changed(object? sender, EventArgs e)
    {
        if (_loading || SelectedNode is not { } node)
            return;

        node.Key = tbKey.Text;
        node.Name = tbName.Text;
        node.Report = cboReport.Text;
        node.Flags.Clear();
        node.Flags.AddRange(clbFlags.CheckedItems.Cast<ValueChoice>().Select(item => item.Key));
        node.Condition = codeCondition.Code;
        node.HasButton = cboxButton.Checked;
        node.ButtonText = tbButtonText.Text;
        node.ButtonPosition = tbButtonPosition.Text;
        node.HasAutomation = cboxAutomation.Checked;
        node.Time = codeTime.Code;
        node.AutomationCondition = codeAutoCondition.Code;
        node.Mode = codeMode.Code;
        node.Repeat = codeRepeat.Code;
        node.End = codeEnd.Code;
        node.Variant = (cboVariant.SelectedItem as VariantChoice)?.Variant;
        node.AutomationFlags.Clear();
        node.AutomationFlags.AddRange(clbAutoFlags.CheckedItems.Cast<ValueChoice>().Select(item => item.Key));
        EnableNodeParts();
        if (tvGraphs.SelectedNode is { } treeNode)
            treeNode.Text = NodeText(node);
        Changed();
    }

    // zaskrtnutie sa do zoznamu zapise az po udalosti ItemCheck
    private static ValueChoice Flag(string key) => new(key, ValueTexts.NodeFlagText(key));

    // kluc priznaku v napovede pri mysi (v zozname su priznaky slovami)
    private void Flags_MouseMove(object? sender, MouseEventArgs e)
    {
        if (sender is not CheckedListBox list)
            return;
        var index = list.IndexFromPoint(e.Location);
        var text = index >= 0 && list.Items[index] is ValueChoice flag ? ValueTexts.NodeFlag(flag.Key) : "";
        if (_flagTip.GetToolTip(list) != text)
            _flagTip.SetToolTip(list, text);
    }

    private void Flags_ItemCheck(object? sender, ItemCheckEventArgs e)
    {
        if (!_loading)
            BeginInvoke(() => Node_Changed(sender, EventArgs.Empty));
    }

    private void Graph_Changed(object? sender, EventArgs e)
    {
        if (_loading || tvGraphs.SelectedNode?.Tag is not GraphDraft graph)
            return;
        graph.Description = tbGraphDescription.Text;
        tvGraphs.SelectedNode.Text = GraphText(graph);
        Changed();
    }

    private void Index_Changed(object? sender, EventArgs e)
    {
        if (_loading || _edit is null)
            return;
        _edit.GraphIndex = codeIndex.Code;
        Changed();
    }

    private void Changed()
    {
        Recheck();
        _check.Schedule();
        lCheck.Text = CodeWindow.Status(_check, _result);
    }

    private void tsbAddGraph_Click(object? sender, EventArgs e)
    {
        if (_edit is null)
            return;
        var graph = new GraphDraft { Description = Resources.Code_NewGraph };
        graph.Nodes.Add(new NodeDraft { Key = "#Start" });
        var index = SelectedGraph is { } selected ? _edit.Graphs.IndexOf(selected) + 1 : _edit.Graphs.Count;
        _edit.Graphs.Insert(index, graph);
        FillTree(graph);
        Changed();
    }

    private void tsbAddNode_Click(object? sender, EventArgs e)
    {
        if (SelectedGraph is not { } graph)
            return;
        var node = new NodeDraft { Key = Resources.Code_NewNode };
        var index = SelectedNode is { } selected ? graph.Nodes.IndexOf(selected) + 1 : graph.Nodes.Count;
        graph.Nodes.Insert(index, node);
        FillTree(node);
        Changed();
    }

    private void tsbDeleteItem_Click(object? sender, EventArgs e)
    {
        if (_edit is null)
            return;
        if (SelectedNode is { } node && SelectedGraph is { } graph)
        {
            var index = graph.Nodes.IndexOf(node);
            graph.Nodes.Remove(node);
            FillTree(graph.Nodes.Count > 0 ? graph.Nodes[Math.Min(index, graph.Nodes.Count - 1)] : graph);
        }
        else if (tvGraphs.SelectedNode?.Tag is GraphDraft selected)
        {
            var index = _edit.Graphs.IndexOf(selected);
            _edit.Graphs.Remove(selected);
            FillTree(_edit.Graphs.Count > 0 ? _edit.Graphs[Math.Min(index, _edit.Graphs.Count - 1)] : null);
        }

        Changed();
    }

    private void tsbItemUp_Click(object? sender, EventArgs e) => MoveItem(-1);

    private void tsbItemDown_Click(object? sender, EventArgs e) => MoveItem(1);

    // poradie stavov urcuje priebeh vlaku, poradie grafov cislo grafu
    private void MoveItem(int by)
    {
        if (_edit is null)
            return;
        if (SelectedNode is { } node && SelectedGraph is { } graph)
        {
            if (MoveIn(graph.Nodes, node, by))
                FillTree(node);
        }
        else if (tvGraphs.SelectedNode?.Tag is GraphDraft selected && MoveIn(_edit.Graphs, selected, by))
        {
            FillTree(selected);
        }

        Changed();
    }

    private static bool MoveIn<T>(List<T> list, T item, int by)
    {
        var index = list.IndexOf(item);
        var target = index + by;
        if (index < 0 || target < 0 || target >= list.Count)
            return false;
        list.RemoveAt(index);
        list.Insert(target, item);
        return true;
    }

    private void UpdateButtons()
    {
        tsbAddNode.Enabled = SelectedGraph is not null;
        tsbDeleteItem.Enabled = tsbItemUp.Enabled = tsbItemDown.Enabled = tvGraphs.SelectedNode is not null;
    }

    private string StationLabel(int index) => _workspace.StationName(_group.Stations[index].Station.Station);

    private List<SetupProblem> Rules() => [.. _edits.Values.SelectMany(edit => CodeRules.Check(edit, StationLabel(edit.Index), _workspace.Functions))];

    // pravidla a posledny vysledok prekladu v zozname zisteni
    private void Recheck()
    {
        var rules = Rules();
        var problems = CodeWindow.Of(rules).Select(problem => (problem.IsError, problem.Message, Target: (Action?)null)).ToList();
        foreach (var place in Places())
            problems.Add((!place.IsWarning, CodeWindow.Message(place, PlaceText(place)), () => GoTo(place)));

        var ordered = problems.OrderBy(problem => problem.IsError ? 0 : 1).ToList();
        _targets.Clear();
        _targets.AddRange(ordered.Select(problem => problem.Target));
        lbProblems.ShowProblems(ordered.Select(problem => (problem.IsError, problem.Message)));
        bOk.Enabled = !rules.Any(problem => problem.IsError);
    }

    // chyby prekladu stavovych diagramov tohto grafikonu
    private IEnumerable<CodeErrorPlace> Places() =>
        _result?.Errors.Where(place => place.IsStateDiagram && string.Equals(place.Group, _group.Group.Name, StringComparison.OrdinalIgnoreCase)) ?? [];

    private static string PlaceText(CodeErrorPlace place)
    {
        var where = string.Format(CultureInfo.CurrentCulture, Resources.Loc_Station, Resources.Page_StateDiagram, place.Station);
        if (place.Graph.Length > 0)
            where = string.Format(CultureInfo.CurrentCulture, Resources.Loc_Graph, where, place.Graph);
        if (place.Node.Length > 0)
            where = string.Format(CultureInfo.CurrentCulture, Resources.Loc_Node, where, place.Node);
        return string.Format(CultureInfo.CurrentCulture, Resources.Loc_Line, where, place.Line) + " (" + (place.Automation ? "a/" : "") + place.Field + ")";
    }

    private int StationIndex(CodeErrorPlace place) => _group.Stations.ToList()
        .FindIndex(station => string.Equals(station.Station.Directory, place.Station, StringComparison.OrdinalIgnoreCase));

    // stav podla popisu grafu a kluca stavu (prvy vyhovujuci, ako ich uvadza server)
    private static (GraphDraft? Graph, NodeDraft? Node) Find(StateDiagramEdit edit, CodeErrorPlace place)
    {
        var graph = edit.Graphs.FirstOrDefault(candidate => candidate.Description == place.Graph);
        return (graph, graph?.Nodes.FirstOrDefault(node => node.Key == place.Node));
    }

    private void GoTo(CodeErrorPlace place)
    {
        var index = StationIndex(place);
        if (index < 0)
            return;
        if ((cboStation.SelectedItem as StationChoice)?.Index != index)
            cboStation.SelectedIndex = index;
        if (_edit is null)
            return;
        var (graph, node) = Find(_edit, place);
        if (node is not null)
            FillTree(node);
        else if (graph is not null)
            FillTree(graph);
        var box = Editor(place);
        box?.Focus();
    }

    private CodeEditorBox? Editor(CodeErrorPlace place) => (place.Automation, place.Field) switch
    {
        (_, "indg") => codeIndex,
        (false, "c") => codeCondition,
        (true, "c") => codeAutoCondition,
        (_, "t") => codeTime,
        (_, "m") => codeMode,
        (_, "r") => codeRepeat,
        (_, "e") => codeEnd,
        _ => null
    };

    // podciarknutie chyb prekladu vo vyrazoch zobrazenej stanice a vybraneho stavu
    private void ShowIssues()
    {
        var issues = new Dictionary<CodeEditorBox, List<CodeIssue>>();
        var choice = cboStation.SelectedItem as StationChoice;
        foreach (var place in Places().Where(place => choice is not null && StationIndex(place) == choice.Index && _edit is not null))
        {
            var (_, node) = Find(_edit!, place);
            var showing = place.Field == "indg" || (node is not null && ReferenceEquals(node, SelectedNode));
            if (!showing || Editor(place) is not { } box)
                continue;
            var stored = place.Field == "c" && node is not null ? Stored(node, place.Automation) : box.Code;
            if (!issues.TryGetValue(box, out var list))
                issues[box] = list = [];
            list.Add(CodeWindow.Issue(box, stored, place.Line, place.Error.Number + " " + place.Error.Message, !place.IsWarning));
        }

        foreach (var box in new[] { codeIndex, codeCondition, codeTime, codeAutoCondition, codeMode, codeRepeat, codeEnd })
            box.SetIssues(issues.TryGetValue(box, out var list) ? list : []);
    }

    // podmienka, ako sa zapise do suboru (s obalom), urcuje posun riadkov chyby
    private static string Stored(NodeDraft node, bool automation)
    {
        var stored = node.To();
        return (automation ? stored.Automation?.Condition : stored.Condition) ?? "";
    }

    private void ShowResult(CodeCheckResult result)
    {
        _result = result;
        lCheck.Text = CodeWindow.Status(_check, result);
        Recheck();
        ShowIssues();
    }

    private void lbProblems_DoubleClick(object? sender, EventArgs e)
    {
        if (lbProblems.SelectedIndex >= 0 && lbProblems.SelectedIndex < _targets.Count)
            _targets[lbProblems.SelectedIndex]?.Invoke();
    }

    private void bOk_Click(object? sender, EventArgs e)
    {
        if (Rules().Any(problem => problem.IsError))
        {
            Utils.ShowWarning(Resources.Code_Errors);
            return;
        }

        if (Places().Any(place => !place.IsWarning) && Utils.ShowQuestion(Resources.Code_ConfirmCompile) != DialogResult.Yes)
            return;

        DialogResult = DialogResult.OK;
    }
}
