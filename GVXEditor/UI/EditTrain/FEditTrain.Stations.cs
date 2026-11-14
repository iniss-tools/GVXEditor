using System.Collections;
using System.ComponentModel;
using System.Globalization;
using GVXEditor.Domain;
using GVXEditor.Domain.Editing;
using GVXEditor.Domain.Rules;
using GVXEditor.Domain.Stations;
using GVXEditor.Domain.Stations.Drafts;
using GVXEditor.Properties;
using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.UI.EditTrain;

// karty V staniciach a Vyluky a odklony: zaznamy vlaku v suboroch stanic podla cisla pri odchode
internal partial class FEditTrain
{
    private static readonly Dictionary<int, List<MosTrain>> NoMos = [];

    // upravy podla stanice a cisla - zachovaju sa aj po prepnuti stanice alebo zmene cisla vo vlaku
    private readonly Dictionary<TrainStationKey, StationEdit> _stationEdits = [];
    private readonly Dictionary<ToolStrip, DataGridView> _recordGrids = [];
    private List<TrainStationKey> _keys = [];
    private List<TrainStationKey> _originalKeys = [];
    private StationEdit? _station;
    private NoticesEdit? _notices;

    private void InitStations()
    {
        foreach (var (strip, grid) in new (ToolStrip, DataGridView)[]
                 {
                     (tsPositions, dgvPositions), (tsAdditions, dgvAdditions), (tsCompositions, dgvCompositions), (tsLanguages, dgvLanguages),
                     (tsReportStations, dgvReportStations), (tsTexts, dgvTexts), (tsMos, dgvMos), (tsLockouts, dgvLockouts),
                     (tsDiversions, dgvDiversions)
                 })
        {
            grid.AutoGenerateColumns = false;
            _recordGrids[strip] = grid;
        }

        cDivStation.DisplayMember = nameof(TrainStationKey.Label);
        cDivStation.ValueMember = nameof(TrainStationKey.Self);
        cboStation.DisplayMember = nameof(TrainStationKey.Label);
        _originalKeys = TryTrain() is { } train ? TrainStations.Keys(train, _group.Group) : [];
    }

    // vlak z konceptu, ak nema chybu - inak sa stanice a cisla neda urcit
    private GvTrain? TryTrain() =>
        TrainRules.Check(_draft, _interestStations, _stationName).Any(problem => problem.IsError) ? null : _draft.ToTrain(_interestStations);

    private void tcMain_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (tcMain.SelectedTab == tpStations)
            RefreshStations();
        else if (tcMain.SelectedTab == tpNotices)
            RefreshNotices();
    }

    private void RefreshStations()
    {
        var selected = cboStation.SelectedItem as TrainStationKey;
        if (TryTrain() is not { } train)
        {
            _keys = [];
            cboStation.DataSource = null;
            ShowStation(null, Resources.Edit_StationsNeedTrain);
            return;
        }

        _keys = TrainStations.Keys(train, _group.Group);
        cboStation.DataSource = _keys.ToList();
        if (_keys.Count == 0)
        {
            ShowStation(null, Resources.Edit_NoStations);
            return;
        }

        cboStation.SelectedItem = selected is not null && _keys.Contains(selected) ? selected : _keys[0];
        ShowStation(cboStation.SelectedItem as TrainStationKey, null);
    }

    private void cboStation_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (cboStation.SelectedItem is TrainStationKey key && !ReferenceEquals(key, _station?.Key))
            ShowStation(key, null);
    }

    private StationEdit EditOf(TrainStationKey key)
    {
        if (!_stationEdits.TryGetValue(key, out var edit))
            _stationEdits[key] = edit = StationEdit.Of(key, _group.Stations[key.StationIndex]);
        return edit;
    }

    private void ShowStation(TrainStationKey? key, string? info)
    {
        EndRecordEdits();
        _station = key is null ? null : EditOf(key);
        var station = _station;

        dgvPositions.DataSource = station?.Positions;
        RefreshAdditionKeys();
        dgvAdditions.DataSource = station?.Additions;
        dgvCompositions.DataSource = station?.Compositions;
        dgvLanguages.DataSource = station?.Languages;
        dgvReportStations.DataSource = station?.ReportStations;
        cTextSet.Items.Clear();
        foreach (var name in station?.TextSetNames ?? [])
            cTextSet.Items.Add(name);
        dgvTexts.DataSource = station?.Texts;
        dgvMos.DataSource = station?.Mos;

        var loaded = station?.Station.Files is not null;
        tcStation.Enabled = loaded;
        tsTexts.Enabled = loaded && station!.TextSetNames.Count > 0;
        lStationInfo.Text = info ?? (station!.Station.Error is { } error
            ? string.Format(CultureInfo.CurrentCulture, Resources.Edit_StationFailed, error)
            : string.Format(CultureInfo.CurrentCulture, Resources.Edit_StationInfo, station.Key.Number) +
              (station.TextSetNames.Count == 0 ? " " + Resources.Edit_NoTextSets : ""));
        Recheck();
    }

    private void RefreshNotices()
    {
        if (_notices is null)
        {
            if (TryTrain() is not { } train)
            {
                lLockouts.Text = Resources.Edit_StationsNeedTrain;
                lDiversions.Text = "";
                splitNotices.Enabled = false;
                return;
            }

            _notices = NoticesEdit.Of(TrainStations.LockoutNumber(train, _interestStations), TrainStations.Keys(train, _group.Group), _group);
            cDivStation.DataSource = _notices.Keys.ToList();
            RefreshNoticeKeys();
            dgvLockouts.DataSource = _notices.Lockouts;
            dgvDiversions.DataSource = _notices.Diversions;
        }

        var loaded = _group.NoticesError is null;
        splitNotices.Enabled = loaded;
        dgvLockouts.Enabled = tsLockouts.Enabled = loaded && _notices.LockoutNumber.Length > 0;
        dgvDiversions.Enabled = tsDiversions.Enabled = loaded && _notices.Keys.Count > 0;
        lLockouts.Text = !loaded ? string.Format(CultureInfo.CurrentCulture, Resources.Edit_NoticesFailed, _group.NoticesError)
            : _notices.LockoutNumber.Length > 0 ? string.Format(CultureInfo.CurrentCulture, Resources.Edit_Lockouts, _notices.LockoutNumber)
            : Resources.Edit_NoLockoutNumber;
        lDiversions.Text = Resources.Edit_Diversions;
        Recheck();
    }

    /// <summary>
    /// Zistenia zaznamov stanic, vyluk a odklonov. Poznamky zaznamov stanice sa rozvijaju do obdobia, ked vlak s cislom
    /// stanicou ide - pocita sa s upravovanym vlakom namiesto povodneho.
    /// </summary>
    private List<TrainProblem> CheckRecords(GvTrain train)
    {
        var problems = new List<TrainProblem>();
        var timetable = _group.Timetable is not { } file ? null
            : _trainIndex >= 0 ? TimetableEditing.Replace(file, _trainIndex, train) : TimetableEditing.Add(file, train);
        var server = timetable?.ForServer(_interestStations, NoMos);

        foreach (var edit in _stationEdits.Values)
        {
            var ranges = server?.TrainRanges(edit.Key.Station.Station);
            (DateOnly, DateOnly)? period = ranges is not null && ranges.TryGetValue(edit.Key.Number, out var range) ? range : null;
            problems.AddRange(RecordRules.Check(edit, period, _keyCatalog, StationLabel(edit.Key.Station)));
        }

        if (_notices is not null)
            problems.AddRange(RecordRules.Check(_notices, _keyCatalog));

        // zmena cisla v trase: zaznamy stanic ostali pri povodnom cisle
        var keys = TrainStations.Keys(train, _group.Group);
        foreach (var original in _originalKeys.Where(key => !keys.Contains(key)))
        {
            var current = keys.Where(key => key.StationIndex == original.StationIndex).Select(key => key.Number).ToList();
            if (current.Count > 0 && HasRecords(original))
            {
                problems.Add(new TrainProblem(false, -1, -1, string.Format(CultureInfo.CurrentCulture, Resources.Rule_NumberChanged,
                    StationLabel(original.Station), string.Join(", ", current), original.Number)));
            }
        }

        return problems;
    }

    private bool HasRecords(TrainStationKey key)
    {
        var edit = EditOf(key);
        return edit.Positions.Count + edit.Additions.Count + edit.Compositions.Count + edit.Languages.Count + edit.ReportStations.Count +
               edit.Texts.Count + edit.Mos.Count > 0;
    }

    private string StationLabel(DirListItem station) => _stationName(station.Station);

    /// <summary>
    /// Grafikon so zaznamami vlaku z kariet stanic, vyluk a odklonov. Nezmenene subory ostanu tie iste objekty.
    /// </summary>
    /// <param name="group">grafikon (uz s upravenymi vlakmi)</param>
    public GroupDocument ApplyRecords(GroupDocument group)
    {
        ArgumentNullException.ThrowIfNull(group);
        var result = group;
        foreach (var edit in _stationEdits.Values)
        {
            if (result.Stations[edit.Key.StationIndex].Files is not { } files)
                continue;
            var applied = edit.Apply(files);
            if (!ReferenceEquals(applied, files))
                result = result.WithStation(edit.Key.StationIndex, applied);
        }

        if (_notices is not null && result.Lockouts is { } lockouts && result.Diversions is { } diversions)
        {
            var newLockouts = _notices.ApplyLockouts(lockouts);
            var newDiversions = _notices.ApplyDiversions(diversions);
            if (!ReferenceEquals(newLockouts, lockouts) || !ReferenceEquals(newDiversions, diversions))
                result = result with { Lockouts = newLockouts, Diversions = newDiversions };
        }

        return result;
    }

    private void EndRecordEdits()
    {
        foreach (var grid in _recordGrids.Values)
            grid.EndEdit();
    }

    private DataGridView? RecordGrid(object? sender) =>
        sender is ToolStripItem { Owner: { } strip } && _recordGrids.TryGetValue(strip, out var grid) ? grid : null;

    private void tsbRecordAdd_Click(object? sender, EventArgs e)
    {
        if (RecordGrid(sender) is not { DataSource: IBindingList list } grid)
            return;

        grid.EndEdit();
        var item = list.AddNew();
        switch (item)
        {
            case TextDraft text when _station is { TextSetNames.Count: > 0 } station:
                text.Set = station.TextSetNames[0];
                break;
            case DiversionDraft diversion when _notices is { Keys.Count: > 0 } notices:
                diversion.Station = notices.Keys[0];
                break;
        }

        if (list is ICancelAddNew pending)
            pending.EndNew(list.Count - 1);
        SelectRecord(grid, list.Count - 1);
        Recheck();
    }

    private void tsbRecordDelete_Click(object? sender, EventArgs e)
    {
        if (RecordGrid(sender) is not { DataSource: IList list } grid || grid.CurrentRow is not { Index: >= 0 } row || row.Index >= list.Count)
            return;

        grid.CancelEdit();
        var index = row.Index;
        list.RemoveAt(index);
        SelectRecord(grid, Math.Min(index, list.Count - 1));
        Recheck();
    }

    private void tsbRecordUp_Click(object? sender, EventArgs e) => MoveRecord(sender, -1);

    private void tsbRecordDown_Click(object? sender, EventArgs e) => MoveRecord(sender, 1);

    // poradie zaznamov urcuje, ktory server najde prvy
    private void MoveRecord(object? sender, int by)
    {
        if (RecordGrid(sender) is not { DataSource: IList list } grid || grid.CurrentRow is not { Index: >= 0 } row)
            return;

        var index = row.Index;
        var target = index + by;
        if (target < 0 || target >= list.Count)
            return;

        grid.EndEdit();
        var item = list[index];
        list.RemoveAt(index);
        list.Insert(target, item);
        SelectRecord(grid, target);
        Recheck();
    }

    private static void SelectRecord(DataGridView grid, int index)
    {
        if (index < 0 || index >= grid.Rows.Count)
            return;
        var column = grid.Columns.GetFirstColumn(DataGridViewElementStates.Visible);
        if (column is not null)
            grid.CurrentCell = grid.Rows[index].Cells[column.Index];
    }

    private void Records_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
    {
        // zaciarkavacie policko a vyber sa maju prejavit hned
        if (sender is DataGridView { IsCurrentCellDirty: true, CurrentCell: DataGridViewCheckBoxCell or DataGridViewComboBoxCell } grid)
            grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
    }

    private void Records_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        if (_loading || e.RowIndex < 0 || sender is not DataGridView grid)
            return;

        grid.InvalidateRow(e.RowIndex);
        Recheck();
    }

    // hodnota mimo poloziek vyberu (napr. sada, ktoru stanica nema) nie je chyba tabulky - ukaze ju kontrola
    private void Records_DataError(object? sender, DataGridViewDataErrorEventArgs e) => e.ThrowException = false;

    private void Records_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || sender is not DataGridView grid || grid.Rows[e.RowIndex].DataBoundItem is not { } item)
            return;

        var column = grid.Columns[e.ColumnIndex];
        string? text = (column, item) switch
        {
            _ when column == cPosTracks && item is PositionDraft position => TrackNames(position),
            _ => null
        };

        if (text is not null)
        {
            e.Value = text;
            e.FormattingApplied = true;
        }
    }

    // nazvy kolaji z PositionsDef.xml pre obsluhu
    private string TrackNames(PositionDraft position)
    {
        if (_station is null || _station.Station.Tracks.Count == 0)
            return "";

        string Name(string key) => _station.Station.Tracks.FirstOrDefault(track => track.Key == key.Trim())?.Name ?? Resources.Track_Unknown;
        var arrival = position.Arrival.Trim();
        var departure = position.Departure.Trim();
        return departure.Length == 0 || departure == arrival ? Name(arrival) : $"{Name(arrival)} → {Name(departure)}";
    }
}
