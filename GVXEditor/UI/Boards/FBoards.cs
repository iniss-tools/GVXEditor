using System.Collections;
using System.ComponentModel;
using System.Globalization;
using GVXEditor.Domain;
using GVXEditor.Domain.Boards;
using GVXEditor.Domain.Boards.Drafts;
using GVXEditor.Domain.Code;
using GVXEditor.Domain.Rules;
using GVXEditor.Domain.Views;
using GVXEditor.UI.Controls;
using GVXEditor.Properties;
using GVXEditor.UI.Code;
using ToolsCore.CodeEditor;
using ToolsCore.Iniss2.Tables;
using ToolsCore.Tools;

namespace GVXEditor.UI.Boards;

/// <summary>
/// Okno Tabule: tabule stanic s castami (Tables.xml) a katalogove predlohy (TCatalog.xml) jedneho grafikonu. Pravidla sa
/// kontroluju pri kazdej zmene, podmienky zobrazenia prelozi server cez most chvilu po poslednej zmene.
/// </summary>
internal partial class FBoards : Form
{

    private readonly EditorContext _ctx;
    private readonly Iniss2Workspace _workspace;
    private readonly GroupDocument _group;
    private readonly int _groupIndex;
    private readonly Dictionary<int, BoardsEdit> _edits = [];
    private readonly CodeCheck _check;
    private readonly List<Action?> _targets = [];
    private CodeCheckResult? _result;
    private BoardsEdit? _edit;
    private bool _loading;

    /// <summary>Stanica v ponuke okna.</summary>
    private sealed record StationChoice(int Index, string Label)
    {
        public override string ToString() => Label;
    }

    /// <param name="context">kontext editora (otvorena instalacia)</param>
    /// <param name="groupIndex">poradie grafikonu</param>
    /// <param name="station">poradie stanice, ktorej tabule sa maju ukazat</param>
    public FBoards(EditorContext context, int groupIndex, int station)
    {
        ArgumentNullException.ThrowIfNull(context);
        _ctx = context;
        _workspace = context.Workspace ?? throw new InvalidOperationException("Instalacia nie je otvorena.");
        _groupIndex = groupIndex;
        _group = _workspace.Groups[groupIndex];
        _loading = true;
        InitializeComponent();
        this.ApplyThemeAndFonts();
        Text = string.Format(CultureInfo.CurrentCulture, "{0} {1}", Resources.Page_Boards, _group.Group.Name);
        dgvTracks.AutoGenerateColumns = false;
        dgvColumns.AutoGenerateColumns = false;
        CodeWindow.Prepare(context, this, CodeWindow.BoardParameters, codeCondition);
        // prazdne pole jasu znamena hodnotu z linky
        foreach (var box in new[] { tbLightTime0, tbLightTime1, tbLight0, tbLight1, tbLightOff0, tbLightOff1, tbLightOnTime })
            box.HintText = Resources.Value_FromLineHint;
        cColumnContent.DropDownWidth = 340;
        cboLine.Items.AddRange([.. _workspace.Lines.Select(line => line.Key)]);

        _check = new CodeCheck(_workspace, this, () => _workspace.History.Current.With(_groupIndex, Apply(_group)), ShowResult);
        Disposed += (_, _) => _check.Dispose();

        cboStation.DataSource = _group.Stations.Select((document, index) =>
            new StationChoice(index, string.Create(CultureInfo.CurrentCulture, $"{document.Station.Directory} ({_workspace.StationName(document.Station.Station)})"))).ToList();
        if (station >= 0 && station < _group.Stations.Count)
            cboStation.SelectedIndex = station;
        _loading = false;
        ShowStation();
        _check.Schedule();
    }

    /// <summary>
    /// Grafikon s tabulami z okna; bez zmeny vrati <paramref name="group" />.
    /// </summary>
    public GroupDocument Apply(GroupDocument group)
    {
        ArgumentNullException.ThrowIfNull(group);
        var result = group;
        foreach (var edit in _edits.Values)
        {
            var boards = edit.Apply();
            if (!ReferenceEquals(boards, edit.Original))
                result = result.WithStationBoards(edit.Station, boards);
        }

        return result;
    }

    private BoardsEdit? EditOf(int index)
    {
        if (_edits.TryGetValue(index, out var edit))
            return edit;
        if (_group.Stations[index].Boards is not { } boards)
            return null;
        return _edits[index] = BoardsEdit.Of(index, boards);
    }

    private int StationIndex => cboStation.SelectedItem is StationChoice choice ? choice.Index : -1;

    private void cboStation_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (!_loading)
            ShowStation();
    }

    private void ShowStation()
    {
        EndEdits();
        var index = StationIndex;
        _edit = index < 0 ? null : EditOf(index);
        var document = index < 0 ? null : _group.Stations[index];
        lStationInfo.Text = document switch
        {
            null => "",
            { Boards: null } => string.Format(CultureInfo.CurrentCulture, Resources.Boards_LoadFailed, document.BoardsError),
            _ => BoardContext.FirstGroupOf(_workspace, document.Station.Station) is { } first && first != _groupIndex
                ? string.Format(CultureInfo.CurrentCulture, Resources.Boards_NotFirst, _workspace.Groups[first].Group.Name)
                : ""
        };
        tcMain.Enabled = _edit is not null;

        _loading = true;
        var tracks = document?.Tracks.Select(track => track.Key) ?? [];
        Choices(cTrackKey, tracks.ToList(), _edit?.Tables.SelectMany(table => table.Parts).SelectMany(part => part.Tracks).Select(track => track.Key) ?? []);
        var columns = _edit?.Templates.SelectMany(template => template.Columns).ToList() ?? [];
        Choices(cColumnContent, ValueTexts.Choices(BoardValues.Contents, ValueTexts.Content, CatalogColumn.NoContent, columns.Select(column => column.Content)));
        Choices(cColumnAlign, ValueTexts.Choices(BoardValues.Aligns, ValueTexts.Align, BoardValues.Aligns[0], columns.Select(column => column.Align)));
        Choices(cColumnTranslation, ValueTexts.Choices(BoardValues.Translations, ValueTexts.TranslationText, null, columns.Select(column => column.Translation)));
        // tabulky TabTab stanice a spolocne, ako ich hlada server, a nazvy zo suboru
        var tabTabs = index >= 0 ? BoardContext.Of(_workspace, _group, index).TabTabs ?? new HashSet<string>() : new HashSet<string>();
        var tables = tabTabs.Order(StringComparer.Ordinal).Concat(columns.SelectMany(column => new[] { column.Table1, column.Table2 })).Prepend("")
            .Distinct(StringComparer.Ordinal).Select(name => new ValueChoice(name, name)).ToList();
        Choices(cColumnTable1, tables);
        Choices(cColumnTable2, [.. tables]);
        lbTemplates.DataSource = _edit?.Templates;
        _loading = false;

        FillTree(null);
        ShowTemplate();
        Recheck();
    }

    // hodnoty vyberu v tabulke: zname, prazdna a tie, co su v datach (inak by tabulka hlasila chybu hodnoty)
    private static void Choices(DataGridViewComboBoxColumn column, IReadOnlyList<string> known, IEnumerable<string> present)
    {
        column.Items.Clear();
        column.Items.AddRange([.. BoardValues.Choices(known, present)]);
    }

    // vyber hodnot slovami: v tabulke je text, do suboru ide kluc
    private static void Choices(DataGridViewComboBoxColumn column, List<ValueChoice> choices)
    {
        column.DataSource = null;
        column.Items.Clear();
        column.ValueMember = nameof(ValueChoice.Key);
        column.DisplayMember = nameof(ValueChoice.Text);
        column.DataSource = choices;
    }

    private static void Choices(ComboBox combo, List<ValueChoice> choices, string key)
    {
        combo.DisplayMember = nameof(ValueChoice.Text);
        combo.ValueMember = nameof(ValueChoice.Key);
        combo.DataSource = choices;
        combo.SelectedValue = key;
    }

    private static void Fill(ComboBox combo, IEnumerable<string> values)
    {
        combo.Items.Clear();
        combo.Items.AddRange([.. values.Where(value => value.Length > 0).Distinct(StringComparer.Ordinal)]);
    }

    // --- tabule ---

    private void FillTree(object? select)
    {
        _loading = true;
        tvBoards.BeginUpdate();
        tvBoards.Nodes.Clear();
        TreeNode? selected = null;
        foreach (var table in _edit?.Tables ?? [])
        {
            var node = tvBoards.Nodes.Add(table.ToString());
            node.Tag = table;
            if (ReferenceEquals(table, select))
                selected = node;
            foreach (var part in table.Parts)
            {
                var child = node.Nodes.Add(part.ToString());
                child.Tag = part;
                if (ReferenceEquals(part, select))
                    selected = child;
            }
        }

        tvBoards.ExpandAll();
        tvBoards.EndUpdate();
        tvBoards.SelectedNode = selected ?? (tvBoards.Nodes.Count > 0 ? tvBoards.Nodes[0] : null);
        _loading = false;
        ShowItem();
    }

    private BoardDraft? SelectedTable => tvBoards.SelectedNode?.Tag switch
    {
        BoardDraft table => table,
        PartDraft => tvBoards.SelectedNode.Parent?.Tag as BoardDraft,
        _ => null
    };

    private PartDraft? SelectedPart => tvBoards.SelectedNode?.Tag as PartDraft;

    private void tvBoards_AfterSelect(object? sender, TreeViewEventArgs e)
    {
        if (!_loading)
            ShowItem();
    }

    private void ShowItem()
    {
        EndEdits();
        var part = SelectedPart;
        var table = part is null ? SelectedTable : null;
        pTable.Visible = table is not null;
        pPart.Visible = part is not null;
        _loading = true;
        if (table is not null)
            ShowTable(table);
        if (part is not null)
            ShowPart(part);
        _loading = false;
        ShowIssues();
    }

    private void ShowTable(BoardDraft table)
    {
        tbKey.Text = table.Key;
        tbName.Text = table.Name;
        cboLine.Text = table.Line;
        var stationLine = StationIndex >= 0 ? BoardContext.StationLineOf(_group, StationIndex) : "";
        lLineInfo.Text = stationLine.Length > 0 ? string.Format(CultureInfo.CurrentCulture, Resources.Boards_StationLine, stationLine) : "";
        tbSubLine.Text = table.SubLine;
        tbAddress.Text = table.Address;
        tbSave.Text = table.SaveFile;
        tbArrows.Text = table.ReverseArrows;
        cboxNoScroll.Checked = table.NoScroll;
        tbLightTime0.Text = table.LightTime0;
        tbLightTime1.Text = table.LightTime1;
        tbLight0.Text = table.Light0;
        tbLight1.Text = table.Light1;
        tbLightOff0.Text = table.LightOff0;
        tbLightOff1.Text = table.LightOff1;
        Choices(cboLightIfEmpty0, LightIfEmptyChoices(table.LightIfEmpty0), table.LightIfEmpty0);
        Choices(cboLightIfEmpty1, LightIfEmptyChoices(table.LightIfEmpty1), table.LightIfEmpty1);
        tbLightOnTime.Text = table.LightOnTime;
    }

    // svieti tabula bez vlakov plnym jasom: podla linky, ano, nie a hodnota zo suboru, aj ked je ina
    private static List<ValueChoice> LightIfEmptyChoices(string value)
    {
        List<ValueChoice> choices = [new("", Resources.Value_FromLine), new("true", Resources.Value_Yes), new("false", Resources.Value_No)];
        if (!choices.Any(choice => choice.Key == value))
            choices.Add(new ValueChoice(value, value));
        return choices;
    }

    private void ShowPart(PartDraft part)
    {
        tbPartName.Text = part.Name;
        Fill(cboTemplate, _edit?.Templates.Select(template => template.Key) ?? []);
        cboTemplate.Text = part.CatalogKey;
        // chybajuci druh je NODEF
        Choices(cboKind, ValueTexts.Choices(BoardValues.PartKinds, ValueTexts.PartKind, null, [part.Kind]), part.Kind.Length > 0 ? part.Kind : BoardValues.PartKinds[0]);
        tbRecords.Text = part.Records;
        tbStations.Text = part.Stations;
        ShowStationNames(part.Stations);
        Fill(cboPrevious, _edit?.Tables.Select(table => table.Key) ?? []);
        cboPrevious.Text = part.Previous;
        tbWebSource.Text = part.WebSource;
        tbWindows.Text = part.WindowDefinition;
        codeCondition.Code = part.Condition;
        dgvTracks.DataSource = part.Tracks;
    }

    private void Board_Changed(object? sender, EventArgs e)
    {
        if (_loading || SelectedPart is not null || SelectedTable is not { } table)
            return;
        table.Key = tbKey.Text;
        table.Name = tbName.Text;
        table.Line = cboLine.Text;
        table.SubLine = tbSubLine.Text;
        table.Address = tbAddress.Text;
        table.SaveFile = tbSave.Text;
        table.ReverseArrows = tbArrows.Text;
        table.NoScroll = cboxNoScroll.Checked;
        table.LightTime0 = tbLightTime0.Text;
        table.LightTime1 = tbLightTime1.Text;
        table.Light0 = tbLight0.Text;
        table.Light1 = tbLight1.Text;
        table.LightOff0 = tbLightOff0.Text;
        table.LightOff1 = tbLightOff1.Text;
        table.LightIfEmpty0 = (cboLightIfEmpty0.SelectedItem as ValueChoice)?.Key ?? "";
        table.LightIfEmpty1 = (cboLightIfEmpty1.SelectedItem as ValueChoice)?.Key ?? "";
        table.LightOnTime = tbLightOnTime.Text;
        if (tvBoards.SelectedNode is { } node)
            node.Text = table.ToString();
        Changed(false);
    }

    private void Part_Changed(object? sender, EventArgs e)
    {
        if (_loading || SelectedPart is not { } part)
            return;
        var condition = part.Condition != codeCondition.Code;
        part.Name = tbPartName.Text;
        part.CatalogKey = cboTemplate.Text;
        // chybajuci druh (NODEF) ostane chybajuci, kym ho obsluha nezmeni
        var kind = (cboKind.SelectedItem as ValueChoice)?.Key ?? "";
        part.Kind = kind == BoardValues.PartKinds[0] && part.Kind.Length == 0 ? "" : kind;
        part.Records = tbRecords.Text;
        part.Stations = tbStations.Text;
        ShowStationNames(part.Stations);
        part.Previous = cboPrevious.Text;
        part.WebSource = tbWebSource.Text;
        part.WindowDefinition = tbWindows.Text;
        part.Condition = codeCondition.Code;
        if (tvBoards.SelectedNode is { } node)
            node.Text = part.ToString();
        Changed(condition);
    }

    private void Tracks_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        if (!_loading && e.RowIndex >= 0)
            Changed(false);
    }

    private void Grid_DataError(object? sender, DataGridViewDataErrorEventArgs e) => e.ThrowException = false;

    private static string[] StationItems(string text) => text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

    // nazvy stanic casti vedla pola s cislami
    private void ShowStationNames(string stations)
    {
        var names = StationItems(stations).Select(item => int.TryParse(item, NumberStyles.None, CultureInfo.InvariantCulture, out var number)
            ? _workspace.StationName(number)
            : item).ToList();
        lStationsHint.Text = names.Count == 0 ? Resources.Boards_StationsDefault : string.Join(", ", names);
    }

    private void bStations_Click(object? sender, EventArgs e)
    {
        if (SelectedPart is not { } part)
            return;
        var stations = StationChoices.Of(_workspace.StationNames, StationItems(part.Stations));
        if (FPickOrdered.Pick(this, Resources.Pick_StationsTitle, [.. stations.Items.Where(choice => choice.Key.Length > 0)], StationItems(part.Stations)) is { } picked)
            tbStations.Text = string.Join(" ", picked);
    }

    // vyber zo zoznamu sa ma prejavit hned, nie az po opusteni bunky
    private void Grid_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
    {
        if (sender is DataGridView { IsCurrentCellDirty: true, CurrentCell: DataGridViewComboBoxCell } grid)
            grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
    }

    private void tsbAddTable_Click(object? sender, EventArgs e)
    {
        EndEdits();
        if (_edit is null)
            return;
        var table = new BoardDraft { Key = UniqueKey(Resources.Boards_NewTable, _edit.Tables.Select(item => item.Key)) };
        table.Parts.Add(new PartDraft { CatalogKey = _edit.Templates.FirstOrDefault()?.Key ?? "" });
        var current = SelectedTable is { } selected ? _edit.Tables.IndexOf(selected) : -1;
        _edit.Tables.Insert(current >= 0 ? current + 1 : _edit.Tables.Count, table);
        FillTree(table);
        Changed(true);
    }

    private void tsbAddPart_Click(object? sender, EventArgs e)
    {
        EndEdits();
        if (SelectedTable is not { } table)
            return;
        var part = new PartDraft { CatalogKey = SelectedPart?.CatalogKey ?? table.Parts.FirstOrDefault()?.CatalogKey ?? _edit?.Templates.FirstOrDefault()?.Key ?? "" };
        var current = SelectedPart is { } selected ? table.Parts.IndexOf(selected) : -1;
        table.Parts.Insert(current >= 0 ? current + 1 : table.Parts.Count, part);
        FillTree(part);
        Changed(true);
    }

    private void tsbDeleteItem_Click(object? sender, EventArgs e)
    {
        EndEdits();
        if (_edit is null || SelectedTable is not { } table)
            return;
        if (SelectedPart is { } part)
        {
            var index = table.Parts.IndexOf(part);
            table.Parts.Remove(part);
            FillTree(table.Parts.Count > 0 ? table.Parts[Math.Min(index, table.Parts.Count - 1)] : table);
        }
        else
        {
            var index = _edit.Tables.IndexOf(table);
            _edit.Tables.Remove(table);
            FillTree(_edit.Tables.Count > 0 ? _edit.Tables[Math.Min(index, _edit.Tables.Count - 1)] : null);
        }

        Changed(true);
    }

    private void tsbItemUp_Click(object? sender, EventArgs e) => MoveItem(-1);

    private void tsbItemDown_Click(object? sender, EventArgs e) => MoveItem(1);

    private void MoveItem(int by)
    {
        EndEdits();
        if (_edit is null || SelectedTable is not { } table)
            return;
        object item = SelectedPart is { } part ? part : table;
        IList list = SelectedPart is not null ? table.Parts : _edit.Tables;
        var index = list.IndexOf(item);
        var target = index + by;
        if (index < 0 || target < 0 || target >= list.Count)
            return;
        list.RemoveAt(index);
        list.Insert(target, item);
        FillTree(item);
        Changed(false);
    }

    private static string UniqueKey(string name, IEnumerable<string> keys)
    {
        var used = keys.ToHashSet(StringComparer.Ordinal);
        var key = name;
        for (var i = 2; used.Contains(key); i++)
            key = string.Create(CultureInfo.InvariantCulture, $"{name} {i}");
        return key;
    }

    // --- predlohy ---

    private TemplateDraft? Template => lbTemplates.SelectedItem as TemplateDraft;

    private void lbTemplates_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (!_loading)
            ShowTemplate();
    }

    private void ShowTemplate()
    {
        EndEdits();
        var template = Template;
        _loading = true;
        tbTemplateKey.Text = template?.Key ?? "";
        tbTemplateName.Text = template?.Name ?? "";
        var classes = ValueTexts.Choices(BoardValues.Classes, ValueTexts.DeviceClass, null, template is null ? [] : [template.Class]);
        if (template is { Class.Length: 0 })
            classes.Insert(0, new ValueChoice("", ""));
        Choices(cboClass, classes, template?.Class ?? "");
        tbFontBank.Text = template?.FontBank ?? "";
        dgvColumns.DataSource = template?.Columns;
        _loading = false;
        pTemplate.Enabled = template is not null;
        dgvColumns.Enabled = template is not null;
        tsColumns.Enabled = template is not null;
    }

    private void Template_Changed(object? sender, EventArgs e)
    {
        if (_loading || Template is not { } template || _edit is null)
            return;
        var renamed = template.Key != tbTemplateKey.Text || template.Name != tbTemplateName.Text;
        template.Key = tbTemplateKey.Text;
        template.Name = tbTemplateName.Text;
        template.Class = (cboClass.SelectedItem as ValueChoice)?.Key ?? template.Class;
        template.FontBank = tbFontBank.Text;
        if (renamed)
        {
            _loading = true;
            _edit.Templates.ResetItem(_edit.Templates.IndexOf(template));
            _loading = false;
        }

        Changed(false);
    }

    private void Columns_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        if (!_loading && e.RowIndex >= 0)
            Changed(false);
    }

    // zoznam podla panela tlacidiel: predlohy, udaje predlohy alebo kolaje casti
    private IList? ListOf(object? sender) => sender is ToolStripItem { Owner: { } strip }
        ? strip == tsTemplates ? _edit?.Templates : strip == tsColumns ? Template?.Columns : SelectedPart?.Tracks
        : null;

    private void tsbAdd_Click(object? sender, EventArgs e)
    {
        EndEdits();
        if (ListOf(sender) is not IBindingList list)
            return;
        object item = list == _edit?.Templates
            ? new TemplateDraft { Key = UniqueKey(Resources.Boards_NewTemplate, _edit.Templates.Select(template => template.Key)) }
            : list == Template?.Columns
                ? new ColumnDraft()
                : new TrackRefDraft();
        var current = Position(list);
        var index = current >= 0 ? current + 1 : list.Count;
        list.Insert(index, item);
        Select(list, index);
        Changed(false);
    }

    private void tsbDelete_Click(object? sender, EventArgs e)
    {
        EndEdits();
        if (ListOf(sender) is not IList list)
            return;
        var index = Position(list);
        if (index < 0 || index >= list.Count)
            return;
        list.RemoveAt(index);
        Select(list, Math.Min(index, list.Count - 1));
        Changed(false);
    }

    private void tsbUp_Click(object? sender, EventArgs e) => MoveRow(sender, -1);

    private void tsbDown_Click(object? sender, EventArgs e) => MoveRow(sender, 1);

    private void MoveRow(object? sender, int by)
    {
        EndEdits();
        if (ListOf(sender) is not IList list)
            return;
        var index = Position(list);
        var target = index + by;
        if (index < 0 || target < 0 || target >= list.Count)
            return;
        var item = list[index];
        list.RemoveAt(index);
        list.Insert(target, item);
        Select(list, target);
        Changed(false);
    }

    private int Position(IList list) => list == _edit?.Templates ? lbTemplates.SelectedIndex
        : list == Template?.Columns ? dgvColumns.CurrentRow?.Index ?? -1
        : dgvTracks.CurrentRow?.Index ?? -1;

    private void Select(IList list, int index)
    {
        if (index < 0)
            return;
        if (list == _edit?.Templates)
        {
            if (index < lbTemplates.Items.Count)
                lbTemplates.SelectedIndex = index;
            ShowTemplate();
            return;
        }

        var grid = list == Template?.Columns ? dgvColumns : dgvTracks;
        if (index < grid.Rows.Count)
            grid.CurrentCell = grid.Rows[index].Cells[0];
    }

    // --- kontrola ---

    private void Changed(bool condition)
    {
        Recheck();
        if (condition)
            _check.Schedule();
        lCheck.Text = CodeWindow.Status(_check, _result);
    }

    private void EndEdits()
    {
        dgvTracks.EndEdit();
        dgvColumns.EndEdit();
    }

    private List<SetupProblem> Rules() => [.. _edits.Values.SelectMany(edit => BoardRules.Check(edit, BoardContext.Of(_workspace, _group, edit.Station)))];

    private void Recheck()
    {
        var rules = Rules();
        var problems = CodeWindow.Of(rules).Select(problem => (problem.IsError, problem.Message, Target: (Action?)null)).ToList();
        foreach (var (place, station, part) in Places())
        {
            var where = string.Format(CultureInfo.CurrentCulture, Resources.Loc_Station, Resources.Page_Boards,
                _workspace.StationName(_group.Stations[station].Station.Station));
            var at = string.Format(CultureInfo.CurrentCulture, Resources.Loc_Part, where, part);
            var line = string.Format(CultureInfo.CurrentCulture, Resources.Loc_Line, string.Format(CultureInfo.CurrentCulture, Resources.Loc_Condition, at),
                place.Line);
            problems.Add((!place.IsWarning, CodeWindow.Message(place, line), () => GoTo(station, part)));
        }

        var ordered = problems.OrderBy(problem => problem.IsError ? 0 : 1).ToList();
        _targets.Clear();
        _targets.AddRange(ordered.Select(problem => problem.Target));
        lbProblems.ShowProblems(ordered.Select(problem => (problem.IsError, problem.Message)));
        bOk.Enabled = !rules.Any(problem => problem.IsError);
    }

    // chyby prekladu podmienok casti tabul tohto grafikonu: server neuvadza miesto, chyba patri kazdej casti s rovnakym kodom
    private IEnumerable<(CodeErrorPlace Place, int Station, PartDraft Part)> Places()
    {
        if (_result is null)
            yield break;
        foreach (var place in _result.Errors.Where(place => place.IsTables))
        {
            foreach (var edit in _edits.Values)
            {
                foreach (var part in edit.Tables.SelectMany(table => table.Parts).Where(part => place.MatchesCondition(part.Condition)))
                    yield return (place, edit.Station, part);
            }
        }
    }

    private void GoTo(int station, PartDraft part)
    {
        if (StationIndex != station)
            cboStation.SelectedIndex = station;
        tcMain.SelectedTab = tpTables;
        FillTree(part);
        codeCondition.Focus();
    }

    private void ShowIssues()
    {
        var issues = new List<CodeIssue>();
        if (SelectedPart is { } part)
        {
            foreach (var (place, _, _) in Places().Where(found => ReferenceEquals(found.Part, part)))
                issues.Add(CodeWindow.Issue(codeCondition, part.Condition, place.Line, place.Error.Number + " " + place.Error.Message, !place.IsWarning, false));
        }

        codeCondition.SetIssues(issues);
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
        EndEdits();
        if (Rules().Any(problem => problem.IsError))
        {
            Utils.ShowWarning(Resources.Boards_Errors);
            return;
        }

        if (Places().Any(found => !found.Place.IsWarning) && Utils.ShowQuestion(Resources.Boards_ConfirmCompile) != DialogResult.Yes)
            return;

        DialogResult = DialogResult.OK;
    }
}
