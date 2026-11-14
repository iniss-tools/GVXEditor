using System.Globalization;
using GVXEditor.Config;
using GVXEditor.Domain.Views;
using GVXEditor.Properties;

namespace GVXEditor.UI.Main;

// zoznam vlakov grafikonu: triedenie podla stlpca, vyber viacerych vlakov (riadok drzi poradie vlaku v GV.XML, nie cislo
// riadka zoznamu) a farby druhov vlakov
internal partial class FMain
{
    // triedenie zoznamu vlakov: stlpec a smer, null = poradie GV.XML; plati aj pri prechode na iny grafikon
    private DataGridViewColumn? _sortColumn;
    private bool _sortDescending;

    // riadok, ktoreho varianty a trasu okno ukazuje
    private TrainRow? _shownTrain;
    private Font? _boldTypeFont;

    /// <summary>
    /// Poradie vlaku pod kurzorom v GV.XML (Upravit, Premenovat cislo, varianty a trasa); -1 = ziadny.
    /// </summary>
    private int CurrentTrainIndex => ShownGroup is not null && dgvTrains.CurrentRow?.DataBoundItem is TrainRow row ? row.Index : -1;

    /// <summary>
    /// Poradie vybranych vlakov v GV.XML vzostupne (Kopirovat, Zmazat, Posunut platnost); bez vyberu vlak pod kurzorom.
    /// </summary>
    private List<int> SelectedTrainIndexes
    {
        get
        {
            if (ShownGroup is null)
                return [];
            var selected = dgvTrains.SelectedRows.Cast<DataGridViewRow>().Select(row => row.DataBoundItem).OfType<TrainRow>()
                .Select(row => row.Index).Order().ToList();
            return selected.Count > 0 || CurrentTrainIndex < 0 ? selected : [CurrentTrainIndex];
        }
    }

    private void FillTrains(IReadOnlyList<TrainRow> rows)
    {
        var property = _sortColumn?.DataPropertyName;
        _shownTrain = null;
        dgvTrains.DataSource = TrainRowOrder.Sort(rows, property, _sortDescending);
        foreach (DataGridViewColumn column in dgvTrains.Columns)
            column.HeaderCell.SortGlyphDirection = column != _sortColumn ? SortOrder.None
                : _sortDescending ? SortOrder.Descending : SortOrder.Ascending;
    }

    // prvy klik triedi vzostupne, druhy zostupne, treti vrati poradie GV.XML
    private void dgvTrains_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left || e.ColumnIndex < 0 || dgvTrains.DataSource is not List<TrainRow> rows)
            return;
        var column = dgvTrains.Columns[e.ColumnIndex];
        if (column.SortMode != DataGridViewColumnSortMode.Programmatic || !TrainRowOrder.CanSort(column.DataPropertyName))
            return;

        if (_sortColumn != column)
        {
            _sortColumn = column;
            _sortDescending = false;
        }
        else if (!_sortDescending)
        {
            _sortDescending = true;
        }
        else
        {
            _sortColumn = null;
        }

        var current = CurrentTrainIndex;
        var selected = SelectedTrainIndexes;
        FillTrains(rows);
        SelectTrains(current, selected);
    }

    /// <summary>
    /// Vyberie vlaky podla poradia v GV.XML; kurzor ostane na <paramref name="current" /> (alebo na prvom vybranom).
    /// </summary>
    private void SelectTrains(int current, IReadOnlyCollection<int> selected)
    {
        var column = dgvTrains.Columns.GetFirstColumn(DataGridViewElementStates.Visible);
        if (column is null)
            return;

        var rows = dgvTrains.Rows.Cast<DataGridViewRow>().Where(row => row.DataBoundItem is TrainRow).ToList();
        var target = rows.FirstOrDefault(row => ((TrainRow)row.DataBoundItem!).Index == current)
                     ?? rows.FirstOrDefault(row => selected.Contains(((TrainRow)row.DataBoundItem!).Index));
        if (target is null)
            return;

        dgvTrains.CurrentCell = target.Cells[column.Index];
        target.Selected = true;
        if (selected.Count <= 1)
            return;
        foreach (var row in rows.Where(row => selected.Contains(((TrainRow)row.DataBoundItem!).Index)))
            row.Selected = true;
    }

    private void SelectTrain(int index) => SelectTrains(index, [index]);

    // varianty a trasa vlaku pod kurzorom; pocas SelectionChanged este CurrentRow ukazuje na predchadzajuci riadok,
    // CurrentCellChanged uz ma novy
    private void dgvTrains_CurrentCellChanged(object? sender, EventArgs e)
    {
        var row = dgvTrains.CurrentRow?.DataBoundItem as TrainRow;
        if (ReferenceEquals(row, _shownTrain))
            return;
        _shownTrain = row;
        ShowVariants();
    }

    private void dgvTrains_SelectionChanged(object sender, EventArgs e)
    {
        UpdateTrainCount();
        UpdateCommandStates();
    }

    private void UpdateTrainCount()
    {
        if (ShownGroup is not { Timetable: { } timetable })
            return;
        var selected = dgvTrains.SelectedRows.Count;
        tsslCount.Text = selected > 1
            ? string.Format(CultureInfo.CurrentCulture, Resources.Status_TrainsSelected, timetable.Trains.Count, selected)
            : string.Format(CultureInfo.CurrentCulture, Resources.Status_Trains, timetable.Trains.Count);
    }

    // pravy klik na vlak mimo vyberu vyberie len jeho, na vybrany vlak posunie kurzor a vyber ponecha (ako Prieskumnik)
    private void dgvTrains_CellMouseDown(object? sender, DataGridViewCellMouseEventArgs e)
    {
        if (e.Button != MouseButtons.Right || e.RowIndex < 0 || e.ColumnIndex < 0)
            return;

        var row = dgvTrains.Rows[e.RowIndex];
        var keep = row.Selected ? dgvTrains.SelectedRows.Cast<DataGridViewRow>().ToList() : [];
        dgvTrains.CurrentCell = row.Cells[e.ColumnIndex];
        foreach (var selected in keep)
            selected.Selected = true;
    }

    // farby stlpca Druh podla skupiny druhu vlaku (farebna schema, kategoria Druh vlaku)
    private void dgvTrains_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex != cType.Index || e.CellStyle is null || _ctx.Workspace is not { } workspace ||
            dgvTrains.Rows[e.RowIndex].DataBoundItem is not TrainRow { Type.Length: > 0 } row)
            return;

        var setting = GvxColors.For(_ctx.UsingStyle.TrainTypeScheme, TrainTypeGroups.Of(row.Type, workspace.TrainTypeDefinitions));
        if (setting.ForeColor.A > 0)
            e.CellStyle.ForeColor = setting.ForeColor;
        if (setting.BackColor.A > 0)
            e.CellStyle.BackColor = setting.BackColor;
        if (setting.Bold && e.CellStyle.Font is { } font)
            e.CellStyle.Font = BoldTypeFont(font);
    }

    // tucne pismo tabulky (aj po zmene pisma) - jedno pre vsetky bunky
    private Font BoldTypeFont(Font font)
    {
        if (_boldTypeFont is { } bold && bold.FontFamily.Equals(font.FontFamily) && bold.Size.Equals(font.Size))
            return bold;
        _boldTypeFont?.Dispose();
        return _boldTypeFont = new Font(font, font.Style | FontStyle.Bold);
    }
}
