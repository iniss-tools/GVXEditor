using System.Collections;
using System.ComponentModel;
using System.Globalization;
using GVXEditor.Domain;
using GVXEditor.Domain.Code;
using GVXEditor.Domain.Code.Drafts;
using GVXEditor.Domain.Rules;
using GVXEditor.Properties;
using ToolsCore.CodeEditor;
using ToolsCore.Iniss2.Tables;
using ToolsCore.Tools;

namespace GVXEditor.UI.Code;

/// <summary>
/// Okno Tabulky TabTab: spolocne tabulky grafikonu (TabTabGlobal.xml) a tabulky stanic (TabTab.xml) s vyrazom VB.NET
/// a slovnikom. Pravidla sa kontroluju pri kazdej zmene, vyrazy prelozi server cez most chvilu po poslednej zmene.
/// </summary>
internal partial class FTabTabs : Form
{
    private readonly EditorContext _ctx;
    private readonly Iniss2Workspace _workspace;
    private readonly GroupDocument _group;
    private readonly int _groupIndex;
    private readonly Dictionary<int, TabTabEdit> _edits = [];
    private readonly CodeCheck _check;
    private readonly List<Action?> _targets = [];
    private CodeCheckResult? _result;
    private TabTabEdit? _edit;
    private bool _loading;

    /// <summary>Subor tabuliek v ponuke okna.</summary>
    /// <param name="Station">poradie stanice; -1 = spolocne tabulky grafikonu</param>
    /// <param name="Label">text v ponuke</param>
    private sealed record FileChoice(int Station, string Label)
    {
        public override string ToString() => Label;
    }

    /// <param name="context">kontext editora (otvorena instalacia)</param>
    /// <param name="groupIndex">poradie grafikonu</param>
    /// <param name="station">poradie stanice, ktorej tabulky sa maju ukazat; -1 = spolocne tabulky</param>
    public FTabTabs(EditorContext context, int groupIndex, int station)
    {
        ArgumentNullException.ThrowIfNull(context);
        _ctx = context;
        _workspace = context.Workspace ?? throw new InvalidOperationException("Instalacia nie je otvorena.");
        _groupIndex = groupIndex;
        _group = _workspace.Groups[groupIndex];
        InitializeComponent();
        this.ApplyThemeAndFonts();
        Text = string.Format(CultureInfo.CurrentCulture, "{0} {1}", Resources.Page_TabTab, _group.Group.Name);
        dgvEntries.AutoGenerateColumns = false;
        CodeWindow.Prepare(context, this, CodeWindow.TabTabParameters, codeExpression);

        _check = new CodeCheck(_workspace, this, () => _workspace.History.Current.With(_groupIndex, Apply(_group)), ShowResult);
        Disposed += (_, _) => _check.Dispose();

        var choices = new List<FileChoice> { new(-1, Resources.Code_GlobalTables) };
        choices.AddRange(_group.Stations.Select((document, index) => new FileChoice(index,
            string.Format(CultureInfo.CurrentCulture, Resources.Code_StationTables, $"{document.Station.Directory} ({_workspace.StationName(document.Station.Station)})"))));
        _loading = true;
        cboFile.DataSource = choices;
        cboFile.SelectedIndex = station >= 0 && station < _group.Stations.Count ? station + 1 : 0;
        _loading = false;
        ShowFile();
        _check.Schedule();
    }

    /// <summary>
    /// Grafikon s tabulkami z okna; bez zmeny vrati <paramref name="group" />.
    /// </summary>
    public GroupDocument Apply(GroupDocument group)
    {
        ArgumentNullException.ThrowIfNull(group);
        var result = group;
        foreach (var edit in _edits.Values)
        {
            var tables = edit.Apply();
            if (ReferenceEquals(tables, edit.Original))
                continue;
            if (edit.Station < 0)
                result = result with { GlobalTabTabs = tables };
            else if (group.Stations[edit.Station].Code is { } code)
                result = result.WithStationCode(edit.Station, code with { TabTabs = tables });
        }

        return result;
    }

    // tabulky suboru zo stavu grafikonu; null, ak sa subor nedal nacitat
    private IReadOnlyList<TabTabTable>? Original(int station) => station < 0 ? _group.GlobalTabTabs : _group.Stations[station].Code?.TabTabs;

    private TabTabEdit? EditOf(int station)
    {
        if (_edits.TryGetValue(station, out var edit))
            return edit;
        if (Original(station) is not { } tables)
            return null;
        return _edits[station] = TabTabEdit.Of(station, tables);
    }

    private void cboFile_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (!_loading)
            ShowFile();
    }

    private void ShowFile()
    {
        EndEdits();
        var choice = cboFile.SelectedItem as FileChoice;
        _edit = choice is null ? null : EditOf(choice.Station);
        var error = choice is null ? null : choice.Station < 0 ? _group.GlobalTabTabsError : _group.Stations[choice.Station].CodeError;
        lFileInfo.Text = _edit is null && error is not null ? string.Format(CultureInfo.CurrentCulture, Resources.Code_LoadFailed, error) : "";
        splitMain.Enabled = _edit is not null;
        _loading = true;
        lbTables.DataSource = _edit?.Tables;
        lbTables.DisplayMember = nameof(TableDraft.Name);
        _loading = false;
        ShowTable();
        Recheck();
    }

    private TableDraft? Table => lbTables.SelectedItem as TableDraft;

    private void lbTables_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (!_loading)
            ShowTable();
    }

    private void ShowTable()
    {
        EndEdits();
        var table = Table;
        _loading = true;
        tbTableName.Text = table?.Name ?? "";
        tbTableDescription.Text = table?.Description ?? "";
        cboxViewValues.Checked = table?.ViewValues ?? false;
        cboxIgnoreCase.Checked = table?.IgnoreCase ?? false;
        codeExpression.Code = table?.Expression ?? "";
        dgvEntries.DataSource = table?.Entries;
        _loading = false;
        splitTable.Enabled = table is not null;
        tsEntries.Enabled = table is not null;
        ShowIssues();
    }

    private void Table_Changed(object? sender, EventArgs e)
    {
        if (_loading || Table is not { } table || _edit is null)
            return;
        var renamed = table.Name != tbTableName.Text;
        table.Name = tbTableName.Text;
        table.Description = tbTableDescription.Text;
        table.ViewValues = cboxViewValues.Checked;
        table.IgnoreCase = cboxIgnoreCase.Checked;
        table.Expression = codeExpression.Code;
        if (renamed)
        {
            _loading = true;
            _edit.Tables.ResetItem(_edit.Tables.IndexOf(table));
            _loading = false;
        }

        Changed();
    }

    private void Entries_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        if (!_loading && e.RowIndex >= 0)
            Changed();
    }

    private void Changed()
    {
        Recheck();
        _check.Schedule();
        lCheck.Text = CodeWindow.Status(_check, _result);
    }

    private void EndEdits() => dgvEntries.EndEdit();

    // zoznam podla panela tlacidiel: tabulky alebo slovnik vybranej tabulky
    private IList? ListOf(object? sender) => sender is ToolStripItem { Owner: { } strip } ? strip == tsTables ? _edit?.Tables : Table?.Entries : null;

    private void tsbAdd_Click(object? sender, EventArgs e)
    {
        EndEdits();
        if (ListOf(sender) is not IBindingList list)
            return;
        var tables = list == _edit?.Tables;
        object item = tables ? new TableDraft { Name = Resources.Code_NewTable } : new EntryDraft();
        var current = tables ? lbTables.SelectedIndex : dgvEntries.CurrentRow?.Index ?? -1;
        var index = current >= 0 ? current + 1 : list.Count;
        list.Insert(index, item);
        Select(tables, index);
        Changed();
    }

    private void tsbDelete_Click(object? sender, EventArgs e)
    {
        EndEdits();
        if (ListOf(sender) is not IList list)
            return;
        var tables = list == _edit?.Tables;
        var index = tables ? lbTables.SelectedIndex : dgvEntries.CurrentRow?.Index ?? -1;
        if (index < 0 || index >= list.Count)
            return;
        list.RemoveAt(index);
        Select(tables, Math.Min(index, list.Count - 1));
        Changed();
    }

    private void tsbUp_Click(object? sender, EventArgs e) => MoveItem(sender, -1);

    private void tsbDown_Click(object? sender, EventArgs e) => MoveItem(sender, 1);

    private void MoveItem(object? sender, int by)
    {
        EndEdits();
        if (ListOf(sender) is not IList list)
            return;
        var tables = list == _edit?.Tables;
        var index = tables ? lbTables.SelectedIndex : dgvEntries.CurrentRow?.Index ?? -1;
        var target = index + by;
        if (index < 0 || target < 0 || target >= list.Count)
            return;
        var item = list[index];
        list.RemoveAt(index);
        list.Insert(target, item);
        Select(tables, target);
        Changed();
    }

    private void Select(bool tables, int index)
    {
        if (index < 0)
            return;
        if (tables)
        {
            if (index < lbTables.Items.Count)
                lbTables.SelectedIndex = index;
            ShowTable();
        }
        else if (index < dgvEntries.Rows.Count)
        {
            dgvEntries.CurrentCell = dgvEntries.Rows[index].Cells[0];
        }
    }

    // nazvy spolocnych tabuliek, ako ich pouzije server: prvy grafikon v poradi, ktory ich ma
    private HashSet<string> GlobalNames()
    {
        for (var i = 0; i < _workspace.Groups.Count; i++)
        {
            var tables = i == _groupIndex && _edits.TryGetValue(-1, out var edit)
                ? edit.Tables.Select(table => table.Name).ToList()
                : _workspace.Groups[i].GlobalTabTabs?.Select(table => table.Name).ToList() ?? [];
            if (tables.Count > 0)
                return [.. tables];
        }

        return [];
    }

    private string FileLabel(int station) =>
        station < 0 ? Resources.Code_GlobalTables : string.Format(CultureInfo.CurrentCulture, Resources.Code_StationTables, _workspace.StationName(_group.Stations[station].Station.Station));

    private List<SetupProblem> Rules()
    {
        var global = GlobalNames();
        return [.. _edits.Values.SelectMany(edit => CodeRules.Check(edit, FileLabel(edit.Station), edit.Station < 0 ? [] : global))];
    }

    private void Recheck()
    {
        var rules = Rules();
        var problems = CodeWindow.Of(rules).Select(problem => (problem.IsError, problem.Message, Target: (Action?)null)).ToList();
        foreach (var place in Places())
        {
            var where = string.Format(CultureInfo.CurrentCulture, Resources.Loc_Table, Resources.Page_TabTab, place.Table);
            problems.Add((!place.IsWarning, CodeWindow.Message(place, string.Format(CultureInfo.CurrentCulture, Resources.Loc_Line, where, place.Line)),
                () => GoTo(place)));
        }

        var ordered = problems.OrderBy(problem => problem.IsError ? 0 : 1).ToList();
        _targets.Clear();
        _targets.AddRange(ordered.Select(problem => problem.Target));
        lbProblems.ShowProblems(ordered.Select(problem => (problem.IsError, problem.Message)));
        bOk.Enabled = !rules.Any(problem => problem.IsError);
    }

    // chyby prekladu tabuliek tohto grafikonu (spolocnych alebo jeho stanic)
    private IEnumerable<CodeErrorPlace> Places() => _result?.Errors.Where(place => place.IsTabTab && Locate(place) is not null) ?? [];

    // subor a tabulka chyby: tabulka s nazvom chyby, pri viacerych ta s rovnakym kodom
    private (int Station, TableDraft Table)? Locate(CodeErrorPlace place)
    {
        var candidates = new List<(int Station, TableDraft Table)>();
        foreach (var station in new[] { -1 }.Concat(Enumerable.Range(0, _group.Stations.Count)))
        {
            var tables = _edits.TryGetValue(station, out var edit)
                ? edit.Tables.ToList()
                : Original(station)?.Where(table => table.Name == place.Table).Select(TableDraft.Of).ToList() ?? [];
            candidates.AddRange(tables.Where(table => table.Name == place.Table).Select(table => (station, table)));
        }

        return candidates.Count switch
        {
            0 => null,
            1 => candidates[0],
            _ => candidates.FirstOrDefault(candidate => candidate.Table.To().Expression == place.Error.Code) is { Table: not null } match ? match : candidates[0]
        };
    }

    private void GoTo(CodeErrorPlace place)
    {
        if (Locate(place) is not { } located)
            return;
        cboFile.SelectedIndex = located.Station + 1;
        if (_edit is null)
            return;
        var table = _edit.Tables.FirstOrDefault(candidate => candidate.Name == place.Table && candidate.To().Expression == place.Error.Code) ??
                    _edit.Tables.FirstOrDefault(candidate => candidate.Name == place.Table);
        if (table is not null)
            Select(true, _edit.Tables.IndexOf(table));
        codeExpression.Focus();
    }

    private void ShowIssues()
    {
        var issues = new List<CodeIssue>();
        if (Table is { } table && _edit is not null)
        {
            var stored = table.To().Expression;
            foreach (var place in Places().Where(place => place.Table == table.Name && place.Error.Code == stored))
                issues.Add(CodeWindow.Issue(codeExpression, stored, place.Line, place.Error.Number + " " + place.Error.Message, !place.IsWarning));
        }

        codeExpression.SetIssues(issues);
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
            Utils.ShowWarning(Resources.Code_Errors);
            return;
        }

        if (Places().Any(place => !place.IsWarning) && Utils.ShowQuestion(Resources.Code_ConfirmCompile) != DialogResult.Yes)
            return;

        DialogResult = DialogResult.OK;
    }
}
