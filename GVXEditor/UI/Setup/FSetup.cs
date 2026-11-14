using System.Collections;
using System.ComponentModel;
using System.Globalization;
using GVXEditor.Domain;
using GVXEditor.Domain.Rules;
using GVXEditor.Domain.Setup;
using GVXEditor.Domain.Setup.Drafts;
using GVXEditor.Domain.Views;
using GVXEditor.Properties;
using GVXEditor.UI.Controls;
using ToolsCore.Tools;

namespace GVXEditor.UI.Setup;

/// <summary>
/// Okno Nastavenie grafikonu: nazvy stanic a predvolene stanice hlasenia grafikonu, pre kazdu stanicu kolaje, typy hlaseni,
/// vzory stanic v hlaseni a texty tabul podla trasy a stanice. Server tieto subory len cita. Kontrola bezi pri kazdej zmene,
/// nastavenie s chybou sa neda potvrdit.
/// </summary>
internal partial class FSetup : Form
{
    private readonly GroupSetupEdit _edit;
    private readonly Func<int, string> _stationName;
    private readonly Dictionary<ToolStrip, DataGridView> _grids = [];
    private readonly StationChoices _stations = new();
    private StationSetupEdit? _station;
    private bool _loading;

    /// <summary>Stanica v ponuke okna.</summary>
    /// <param name="Index">poradie stanice v grafikone</param>
    /// <param name="Label">text v ponuke</param>
    private sealed record StationChoice(int Index, string Label)
    {
        public override string ToString() => Label;
    }

    /// <param name="group">grafikon so stanicami</param>
    /// <param name="stationName">nazov stanice podla cisla</param>
    /// <param name="station">poradie stanice, ktora sa ma ukazat; -1 = nazvy stanic grafikonu</param>
    /// <param name="bank">zvukova banka instalacie na vyber zvukov kolaji; <see langword="null" /> = zvuky sa len pisu</param>
    public FSetup(GroupDocument group, Func<int, string> stationName, int station, SoundBank? bank = null)
    {
        ArgumentNullException.ThrowIfNull(group);
        _edit = GroupSetupEdit.Of(group);
        _stationName = stationName;
        InitializeComponent();
        this.ApplyThemeAndFonts();
        Text = string.Format(CultureInfo.CurrentCulture, Resources.Setup_Title, group.Group.Name);

        foreach (var (strip, grid) in new (ToolStrip, DataGridView)[]
                 {
                     (tsNames, dgvNames), (tsDefaults, dgvDefaults), (tsTracks, dgvTracks), (tsReportKeys, dgvReportKeys), (tsPatterns, dgvPatterns),
                     (tsPatternStops, dgvPatternStops), (tsRouteSets, dgvRouteSets), (tsRouteTargets, dgvRouteTargets), (tsRoutes, dgvRoutes),
                     (tsRouteStops, dgvRouteStops), (tsStationSets, dgvStationSets), (tsStationTargets, dgvStationTargets),
                     (tsStationTexts, dgvStationTexts)
                 })
        {
            grid.AutoGenerateColumns = false;
            _grids[strip] = grid;
        }

        // stanice nazvom: zoznam z nazvov stanic v okne, kategorie a ciel textov vyberom
        _stations.Bind(dgvDefaults, cDefStationName);
        _stations.Bind(dgvPatternStops, cPatStopName);
        _stations.Bind(dgvRouteStops, cRouteStopName);
        _stations.Bind(dgvStationTexts, cStationTextName);
        CellPicker.Attach(dgvReportKeys, cKeyCategories, item => PickCategories(((ReportKeyDraft)item).Categories),
            item => CategoriesText(((ReportKeyDraft)item).Categories));
        RefreshStationChoices();
        if (bank is not null)
        {
            foreach (var column in new DataGridViewColumn[] { cTrackSound, cTrackPlatformSound, cTrackSectorSound })
                CellPicker.AttachEditable(dgvTracks, column, item => FPickSound.PickKey(this, bank, column.HeaderText, SoundKey(column, (TrackDraft)item)));
        }

        dgvNames.DataSource = _edit.Names;
        dgvDefaults.DataSource = _edit.DefaultStations;
        if (group.Setup is null)
        {
            var error = string.Format(CultureInfo.CurrentCulture, Resources.Setup_GroupFailed, group.SetupError);
            lNames.Text = lDefaults.Text = error;
            dgvNames.Enabled = tsNames.Enabled = dgvDefaults.Enabled = tsDefaults.Enabled = false;
        }

        var choices = group.Stations.Select((document, index) => new StationChoice(index,
            string.Create(CultureInfo.CurrentCulture, $"{document.Station.Directory} ({stationName(document.Station.Station)})"))).ToList();
        _loading = true;
        cboStation.DataSource = choices;
        if (station >= 0 && station < choices.Count)
        {
            tcMain.SelectedTab = tpStations;
            cboStation.SelectedIndex = station;
        }

        _loading = false;
        ShowStation();
    }

    /// <summary>
    /// Grafikon s nastavenim z okna; bez zmeny vrati <paramref name="group" />.
    /// </summary>
    /// <param name="group">aktualny grafikon (ten isty, z ktoreho okno vzniklo)</param>
    public GroupDocument Apply(GroupDocument group) => _edit.Apply(group);

    // nazov stanice podla nazvov v okne (aj neulozenych), inak podla instalacie
    private string StationName(int number) =>
        _edit.Names.FirstOrDefault(name => name.Number == number && name.Name.Length > 0)?.Name ?? _stationName(number);

    private string StationName(string station) => SetupText.Station(station) is { } number ? StationName(number) : "";

    private string StationNames(IEnumerable<StationRefDraft> stations) => string.Join(" – ", stations.Select(station => StationName(station.Station)));

    // kluc zvuku kolaje v stlpci
    private string SoundKey(DataGridViewColumn column, TrackDraft track) =>
        column == cTrackSound ? track.TrackSound : column == cTrackPlatformSound ? track.PlatformSound : track.SectorSound;

    private void cboStation_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (!_loading)
            ShowStation();
    }

    private void ShowStation()
    {
        EndEdits();
        _station = cboStation.SelectedItem is StationChoice choice ? _edit.Station(choice.Index) : null;
        var station = _station;
        RefreshStationChoices();
        RefreshTargetChoices(cboStation.SelectedItem is StationChoice selectedStation ? selectedStation.Index : -1);

        _loading = true;
        dgvTracks.DataSource = station?.Tracks;
        dgvReportKeys.DataSource = station?.ReportKeys;
        dgvRouteSets.DataSource = station?.RouteSets;
        dgvStationSets.DataSource = station?.StationSets;
        _loading = false;
        ShowPatterns();
        ShowDetails(dgvRouteSets);
        ShowDetails(dgvStationSets);

        tcStation.Enabled = station is not null;
        if (cboStation.SelectedItem is StationChoice selected)
        {
            var document = _edit.Group.Stations[selected.Index];
            lStationInfo.Text = document.Setup is null
                ? string.Format(CultureInfo.CurrentCulture, Resources.Setup_StationFailed, document.SetupError)
                : string.Format(CultureInfo.CurrentCulture, Resources.Setup_StationInfo, document.Station.Directory, document.Station.Station);
        }
        else
        {
            lStationInfo.Text = "";
        }

        Recheck();
    }

    private void rbPatterns_CheckedChanged(object? sender, EventArgs e)
    {
        if (sender is RadioButton { Checked: true })
            ShowPatterns();
    }

    private void ShowPatterns()
    {
        EndEdits();
        dgvPatterns.DataSource = _station is null ? null : rbLongPatterns.Checked ? _station.LongPatterns : _station.ShortPatterns;
        ShowDetails(dgvPatterns);
    }

    // podrobnosti vybraneho riadku zoznamu (stanice vzoru, udaje a texty sady, stanice trasy)
    private void ShowDetails(DataGridView master)
    {
        var current = master.CurrentRow?.DataBoundItem;
        if (master == dgvPatterns)
        {
            Bind(dgvPatternStops, (current as PatternDraft)?.Stops);
        }
        else if (master == dgvRouteSets)
        {
            var set = current as RouteSetDraft;
            Bind(dgvRouteTargets, set?.Targets);
            Bind(dgvRoutes, set?.Routes);
            ShowDetails(dgvRoutes);
        }
        else if (master == dgvRoutes)
        {
            Bind(dgvRouteStops, (current as RouteDraft)?.Stations);
        }
        else if (master == dgvStationSets)
        {
            var set = current as StationSetDraft;
            Bind(dgvStationTargets, set?.Targets);
            Bind(dgvStationTexts, set?.Texts);
        }
    }

    private void Bind(DataGridView grid, IBindingList? list)
    {
        if (ReferenceEquals(grid.DataSource, list))
            return;

        grid.EndEdit();
        grid.DataSource = list;
        foreach (var (strip, owner) in _grids)
        {
            if (owner == grid)
                strip.Enabled = list is not null;
        }
    }

    private void Master_CurrentCellChanged(object? sender, EventArgs e)
    {
        if (!_loading && sender is DataGridView grid)
            ShowDetails(grid);
    }

    private void EndEdits()
    {
        foreach (var grid in _grids.Values)
            grid.EndEdit();
    }

    private DataGridView? GridOf(object? sender) =>
        sender is ToolStripItem { Owner: { } strip } && _grids.TryGetValue(strip, out var grid) ? grid : null;

    private void tsbAdd_Click(object? sender, EventArgs e)
    {
        if (GridOf(sender) is not { DataSource: IBindingList list } grid)
            return;

        grid.EndEdit();
        // novy riadok za vybrany - poradie urcuje, co server najde prvy
        var target = grid.CurrentRow is { Index: >= 0 } row ? row.Index + 1 : list.Count;
        var item = list.AddNew();
        if (list is ICancelAddNew pending)
            pending.EndNew(list.Count - 1);
        if (target < list.Count - 1)
        {
            list.RemoveAt(list.Count - 1);
            list.Insert(target, item);
        }

        Select(grid, target);
        Changed(grid);
    }

    private void tsbDelete_Click(object? sender, EventArgs e)
    {
        if (GridOf(sender) is not { DataSource: IList list } grid || grid.CurrentRow is not { Index: >= 0 } row || row.Index >= list.Count)
            return;

        grid.CancelEdit();
        var index = row.Index;
        list.RemoveAt(index);
        Select(grid, Math.Min(index, list.Count - 1));
        Changed(grid);
    }

    private void tsbUp_Click(object? sender, EventArgs e) => MoveRow(sender, -1);

    private void tsbDown_Click(object? sender, EventArgs e) => MoveRow(sender, 1);

    private void MoveRow(object? sender, int by)
    {
        if (GridOf(sender) is not { DataSource: IList list } grid || grid.CurrentRow is not { Index: >= 0 } row)
            return;

        var index = row.Index;
        var target = index + by;
        if (target < 0 || target >= list.Count)
            return;

        grid.EndEdit();
        var item = list[index];
        list.RemoveAt(index);
        list.Insert(target, item);
        Select(grid, target);
        Changed(grid);
    }

    private static void Select(DataGridView grid, int index)
    {
        if (index < 0 || index >= grid.Rows.Count)
            return;

        var column = grid.Columns.GetFirstColumn(DataGridViewElementStates.Visible);
        if (column is not null)
            grid.CurrentCell = grid.Rows[index].Cells[column.Index];
    }

    // zmena zoznamu: suhrny nadradeneho zoznamu (stanice vzoru a trasy) a kontrola
    private void Changed(DataGridView grid)
    {
        if (grid == dgvPatternStops)
            dgvPatterns.Invalidate();
        else if (grid == dgvRouteStops)
            dgvRoutes.Invalidate();
        else if (grid == dgvNames)
            InvalidateNames();
        Recheck();
    }

    // nazvy stanic sa ukazuju vo viacerych tabulkach
    private void InvalidateNames()
    {
        RefreshStationChoices();
        foreach (var grid in new[] { dgvDefaults, dgvPatterns, dgvPatternStops, dgvRoutes, dgvRouteStops, dgvStationTexts })
            grid.Invalidate();
    }

    private void List_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
    {
        // zaciarkavacie policko sa ma prejavit hned
        if (sender is DataGridView { IsCurrentCellDirty: true, CurrentCell: DataGridViewCheckBoxCell or DataGridViewComboBoxCell } grid)
            grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
    }

    private void List_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        if (_loading || e.RowIndex < 0 || sender is not DataGridView grid)
            return;

        grid.InvalidateRow(e.RowIndex);
        Changed(grid);
    }

    // hodnota, ktoru tabulka nevie zobrazit, nie je chyba okna - ukaze ju kontrola
    private void List_DataError(object? sender, DataGridViewDataErrorEventArgs e) => e.ThrowException = false;

    private void List_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || sender is not DataGridView grid || grid.Rows[e.RowIndex].DataBoundItem is not { } item)
            return;

        var column = grid.Columns[e.ColumnIndex];
        string? text = (column, item) switch
        {
            _ when column == cPatIndex => (e.RowIndex + 1).ToString(CultureInfo.CurrentCulture),
            _ when column == cPatStations && item is PatternDraft pattern => StationNames(pattern.Stops),
            _ when column == cRouteStations && item is RouteDraft route => StationNames(route.Stations),
            _ => null
        };

        if (text is not null)
        {
            e.Value = text;
            e.FormattingApplied = true;
        }
    }

    private List<SetupProblem> Check()
    {
        var problems = SetupRules.Check(_edit);
        foreach (var station in _edit.Stations)
            problems.AddRange(SetupRules.Check(station, StationName(station.Station.Station.Station)));
        return problems;
    }

    private void Recheck()
    {
        var problems = Check();
        lbProblems.ShowProblems(problems.Select(problem => (problem.IsError, problem.Message)));
        bOk.Enabled = !problems.Any(problem => problem.IsError);
    }

    private void bOk_Click(object? sender, EventArgs e)
    {
        EndEdits();
        if (Check().Any(problem => problem.IsError))
        {
            Utils.ShowWarning(Resources.Setup_Errors);
            return;
        }

        DialogResult = DialogResult.OK;
    }

    private void FSetup_FormClosing(object? sender, FormClosingEventArgs e)
    {
        // rozpisana bunka by sa pri zatvoreni stratila - pri OK sa zapisala uz v bOk_Click
        if (DialogResult != DialogResult.OK)
        {
            foreach (var grid in _grids.Values)
                grid.CancelEdit();
        }
    }
}
