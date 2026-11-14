using System.Collections;
using System.Globalization;
using GVXEditor.Domain.Rules;
using GVXEditor.Domain.Setup;
using GVXEditor.Domain.Setup.Drafts;
using GVXEditor.Domain.Stations;
using GVXEditor.Domain.Views;
using GVXEditor.Properties;
using GVXEditor.UI.Controls;
using ToolsCore.Iniss2.Grafikon;
using ToolsCore.Tools;

namespace GVXEditor.UI.Setup;

/// <summary>
/// Okno Hlasenia obsluhy: hlasenia z ConstReports.xml (vzor LogText so stanicami), na ktore sa odkazuju dodatky, vyluky
/// a odklony vlakov. Kontrola bezi pri kazdej zmene, hlasenia s chybou sa nedaju potvrdit.
/// </summary>
internal partial class FConstReports : Form
{
    private readonly ConstReportsEdit _edit;
    private readonly KeyCatalog _keys;
    private readonly IReadOnlySet<int> _used;
    private readonly StationChoices _stations;

    /// <param name="reports">hlasenia obsluhy</param>
    /// <param name="keys">funkcie LogText a ich popisy</param>
    /// <param name="used">cisla hlaseni, na ktore sa odkazuju zaznamy vlakov</param>
    /// <param name="stationNames">nazvy stanic instalacie podla cisla</param>
    public FConstReports(IReadOnlyList<ConstReport> reports, KeyCatalog keys, IReadOnlySet<int> used, IReadOnlyDictionary<int, string> stationNames)
    {
        ArgumentNullException.ThrowIfNull(keys);
        _edit = ConstReportsEdit.Of(reports);
        _keys = keys;
        _used = used;
        _stations = StationChoices.Of(stationNames, _edit.Reports.SelectMany(report => SetupText.Items(report.Stations)));
        InitializeComponent();
        this.ApplyThemeAndFonts();
        dgvReports.AutoGenerateColumns = false;
        // vzor hlasenia s popisom, stanice nazvami v okne vyberu
        cRepFunction.ValueMember = nameof(ValueChoice.Key);
        cRepFunction.DisplayMember = nameof(ValueChoice.Text);
        cRepFunction.DataSource = _keys.FunctionChoices(Patterns, _edit.Reports.Select(report => report.Function));
        CellPicker.Attach(dgvReports, cRepStations, item => PickStations(((ConstReportDraft)item).Stations),
            item => StationsText(((ConstReportDraft)item).Stations));
        dgvReports.DataSource = _edit.Reports;
        Recheck();
    }

    /// <summary>
    /// Hlasenia z okna; bez zmeny vrati povodny zoznam.
    /// </summary>
    public IReadOnlyList<ConstReport> Apply() => _edit.Apply();

    private void tsbAdd_Click(object? sender, EventArgs e)
    {
        dgvReports.EndEdit();
        var draft = new ConstReportDraft { Key = SetupText.Number(_edit.NextKey()) };
        _edit.Reports.Add(draft);
        Select(_edit.Reports.Count - 1);
        Recheck();
    }

    private void tsbDelete_Click(object? sender, EventArgs e)
    {
        if (dgvReports.CurrentRow is not { Index: >= 0 } row || row.Index >= _edit.Reports.Count)
            return;

        dgvReports.CancelEdit();
        var index = row.Index;
        _edit.Reports.RemoveAt(index);
        Select(Math.Min(index, _edit.Reports.Count - 1));
        Recheck();
    }

    private void tsbUp_Click(object? sender, EventArgs e) => MoveRow(-1);

    private void tsbDown_Click(object? sender, EventArgs e) => MoveRow(1);

    private void MoveRow(int by)
    {
        if (dgvReports.CurrentRow is not { Index: >= 0 } row)
            return;

        IList list = _edit.Reports;
        var index = row.Index;
        var target = index + by;
        if (target < 0 || target >= list.Count)
            return;

        dgvReports.EndEdit();
        var item = list[index];
        list.RemoveAt(index);
        list.Insert(target, item);
        Select(target);
        Recheck();
    }

    private void Select(int index)
    {
        if (index < 0 || index >= dgvReports.Rows.Count)
            return;

        var column = dgvReports.Columns.GetFirstColumn(DataGridViewElementStates.Visible);
        if (column is not null)
            dgvReports.CurrentCell = dgvReports.Rows[index].Cells[column.Index];
    }

    // tabulka nema zaciarkavacie policko - hodnota sa prejavi po opusteni bunky
    private void List_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
    {
    }

    private void List_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0)
            return;

        dgvReports.InvalidateRow(e.RowIndex);
        Recheck();
    }

    // hodnota, ktoru tabulka nevie zobrazit, nie je chyba okna - ukaze ju kontrola
    private void List_DataError(object? sender, DataGridViewDataErrorEventArgs e) => e.ThrowException = false;

    // vzory hlaseni obsluhy su funkcie LogText s tymito oznaceniami
    private static readonly string[] Patterns = ["VzorDodatku", "VzorOdklonu", "VzorVyluky"];

    private string StationsText(string value) => string.Join(", ", SetupText.Items(value).Select(_stations.Text));

    private string? PickStations(string value) =>
        FPickOrdered.Pick(this, Resources.Pick_StationsTitle, [.. _stations.Items.Where(choice => choice.Key.Length > 0)], SetupText.Items(value)) is { } picked
            ? string.Join(" ", picked)
            : null;

    // texty buniek stanic nastavuje vyber v bunke
    private void List_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
    }

    private List<SetupProblem> Check() => SetupRules.Check(_edit, _keys.Functions, _used);

    private void Recheck()
    {
        var problems = Check();
        lbProblems.ShowProblems(problems.Select(problem => (problem.IsError, problem.Message)));
        bOk.Enabled = !problems.Any(problem => problem.IsError);
    }

    private void bOk_Click(object? sender, EventArgs e)
    {
        dgvReports.EndEdit();
        if (Check().Any(problem => problem.IsError))
        {
            Utils.ShowWarning(Resources.ConstReports_Errors);
            return;
        }

        DialogResult = DialogResult.OK;
    }

    private void FConstReports_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (DialogResult != DialogResult.OK)
            dgvReports.CancelEdit();
    }

    /// <inheritdoc />
    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        if (_edit.Reports.Count > 0)
            Select(0);
    }
}
