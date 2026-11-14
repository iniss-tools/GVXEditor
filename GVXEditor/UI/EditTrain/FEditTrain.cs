using System.ComponentModel;
using System.Globalization;
using GVXEditor.Domain;
using GVXEditor.Domain.Editing;
using GVXEditor.Domain.Rules;
using GVXEditor.Domain.Views;
using GVXEditor.Properties;
using ToolsCore.Forms.Calendar;
using ToolsCore.Iniss2.Grafikon;
using ToolsCore.Tools;

namespace GVXEditor.UI.EditTrain;

/// <summary>
/// Okno Upravit vlak: obdobie platnosti, varianty s datumovou poznamkou a trasa vybranej varianty s casmi, zaznamy vlaku
/// v suboroch stanic, vyluky a odklony. Kontrola pravidiel bezi pri kazdej zmene, vlak s chybou sa neda ulozit.
/// </summary>
internal partial class FEditTrain : Form
{
    private readonly EditorContext _ctx;
    private readonly TrainDraft _draft;
    private readonly GroupDocument _group;
    private readonly int _trainIndex;
    private Domain.Stations.KeyCatalog _keyCatalog;

    // skript hlaseni s dodatkami zalozenymi z okna; null = bez zmeny
    private string? _logText;
    private readonly IReadOnlySet<int> _interestStations;
    private readonly Func<int, string> _stationName;
    private readonly BindingList<VariantDraft> _variants;
    private readonly CheckBox[] _flagBoxes;
    private readonly GvVariantFlags[] _flagValues =
        [GvVariantFlags.D, GvVariantFlags.X, GvVariantFlags.N, GvVariantFlags.NP, GvVariantFlags.O, GvVariantFlags.M, GvVariantFlags.R, GvVariantFlags.L];

    // dni okna kalendara, ked plati prave dana varianta, a ci server jej poznamku rozvinie (po poslednej kontrole bez chyby)
    private List<(bool[] Runs, bool Invalid)> _active = [];
    private bool _loading;

    /// <param name="context">kontext editora</param>
    /// <param name="draft">koncept vlaku - okno ho meni</param>
    /// <param name="group">grafikon vlaku so subormi stanic</param>
    /// <param name="trainIndex">poradie vlaku v GV.XML; -1 = novy vlak</param>
    /// <param name="stationName">nazov stanice podla cisla</param>
    /// <param name="title">titulok okna</param>
    public FEditTrain(EditorContext context, TrainDraft draft, GroupDocument group, int trainIndex, Func<int, string> stationName, string title)
    {
        ArgumentNullException.ThrowIfNull(group);
        _ctx = context;
        _draft = draft;
        _group = group;
        _trainIndex = trainIndex;
        _interestStations = group.InterestStations;
        _keyCatalog = new Domain.Stations.KeyCatalog(context.Workspace?.Functions, context.Workspace?.FunctionDescriptions,
            context.Workspace?.ConstReports ?? []);
        _stationName = stationName;
        InitializeComponent();
        InitStations();
        InitPickers();
        _flagBoxes = [cboxFlagD, cboxFlagX, cboxFlagN, cboxFlagNP, cboxFlagO, cboxFlagM, cboxFlagR, cboxFlagL];
        // priznaky slovami s klucom a co robia na serveri
        var tips = new ToolTip(components ??= new Container());
        for (var i = 0; i < _flagBoxes.Length; i++)
        {
            _flagBoxes[i].Text = ValueTexts.VariantFlag(_flagValues[i]);
            tips.SetToolTip(_flagBoxes[i], ValueTexts.VariantFlagTip(_flagValues[i]));
        }

        foreach (var (column, flag) in new (DataGridViewColumn, GvStopFlags)[]
                 {
                     (cPassing, GvStopFlags.Passing), (cTransfer, GvStopFlags.Pr), (cOperating, GvStopFlags.OperatingPoint), (cRequest, GvStopFlags.OnRequest)
                 })
        {
            column.HeaderText = ValueTexts.StopFlag(flag);
            column.ToolTipText = ValueTexts.StopFlagTip(flag);
        }

        dgvVariants.AutoGenerateColumns = false;
        dgvStops.AutoGenerateColumns = false;
        this.ApplyThemeAndFonts();
        FitFlags();
        flpFlags.Resize += (_, _) => FitFlags();
        Text = title;

        _loading = true;
        dtpFrom.Value = draft.From.ToDateTime(TimeOnly.MinValue);
        dtpTo.Value = draft.To.ToDateTime(TimeOnly.MinValue);
        cboxNoEnd.Checked = draft.NoEnd;
        dtpTo.Enabled = !draft.NoEnd;
        _variants = new BindingList<VariantDraft>(draft.Variants);
        dgvVariants.DataSource = _variants;
        _loading = false;

        calendarStrip.RowLabel = row => row.Position.ToString(CultureInfo.CurrentCulture);
        calendarStrip.DayToolTip = DayToolTip;
        Recheck();
    }

    /// <inheritdoc />
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        SelectVariant(0);
    }

    /// <summary>Vlak po potvrdeni okna; <see langword="null" />, ak ho obsluha zrusila.</summary>
    public GvTrain? Result { get; private set; }

    private VariantDraft? Selected =>
        (dgvVariants.SelectedRows.Count > 0 ? dgvVariants.SelectedRows[0] : dgvVariants.CurrentRow)?.DataBoundItem as VariantDraft;

    private int SelectedIndex => Selected is { } variant ? _variants.IndexOf(variant) : -1;

    // priznaky slovami sa podla sirky okna zalomia do viacerych riadkov: panel narastie podla obsahu a poznamka
    // s kalendarom sa posunu pod neho (pri pevnej vyske by posledny riadok priznakov nebolo vidno)
    private void FitFlags()
    {
        var delta = flpFlags.GetPreferredSize(new Size(flpFlags.Width, 0)).Height - flpFlags.Height;
        if (delta == 0)
            return;

        flpFlags.Height += delta;
        lNote.Top += delta;
        tbNote.Top += delta;
        calendarStrip.Top += delta;
    }

    // obdobie kalendara: vlak bez konca ukazuje prvy rok
    private DateOnly WindowEnd => _draft.NoEnd ? _draft.From.AddDays(364) : _draft.To;

    private void Period_Changed(object? sender, EventArgs e)
    {
        dtpTo.Enabled = !cboxNoEnd.Checked;
        if (_loading)
            return;

        _draft.From = DateOnly.FromDateTime(dtpFrom.Value);
        _draft.To = DateOnly.FromDateTime(dtpTo.Value);
        _draft.NoEnd = cboxNoEnd.Checked;
        Recheck();
    }

    private void dgvVariants_SelectionChanged(object? sender, EventArgs e) => ShowVariant();

    // udaje vybranej varianty do poli a jej trasa do tabulky
    private void ShowVariant()
    {
        var variant = Selected;
        _loading = true;
        pVariant.Enabled = variant is not null;
        tbValidity.Text = variant?.Validity ?? "";
        tbOperator.Text = variant?.Operator ?? "";
        tbNote.Text = variant?.Note ?? "";
        for (var i = 0; i < _flagBoxes.Length; i++)
            _flagBoxes[i].Checked = variant is not null && variant.Flags.HasFlag(_flagValues[i]);
        dgvStops.DataSource = variant?.Stops;
        _loading = false;
        UpdateButtons();
        UpdateStrip();
    }

    private void Variant_Changed(object? sender, EventArgs e)
    {
        if (_loading || Selected is not { } variant)
            return;

        variant.Validity = tbValidity.Text;
        variant.Operator = tbOperator.Text;
        variant.Note = tbNote.Text;
        var flags = GvVariantFlags.None;
        for (var i = 0; i < _flagBoxes.Length; i++)
        {
            if (_flagBoxes[i].Checked)
                flags |= _flagValues[i];
        }

        variant.Flags = flags;
        dgvVariants.InvalidateRow(SelectedIndex);
        Recheck();
    }

    private void bValidity_Click(object? sender, EventArgs e)
    {
        if (Selected is null)
            return;

        // bez konca platnosti textovu poznamku server nerozvinie - editor dostane rok od zaciatku
        var to = WindowEnd;
        var title = string.Create(CultureInfo.CurrentCulture, $"{Text} ({SelectedIndex + 1})");
        if (FDateLimitEdit.SetDateLimit(this, _ctx.UsingStyle, _draft.From.ToDateTime(TimeOnly.MinValue), to.ToDateTime(TimeOnly.MinValue), title,
                defaultValue: tbValidity.Text) is { } validity)
            tbValidity.Text = validity;
    }

    private void tsbVarAdd_Click(object? sender, EventArgs e)
    {
        // nova varianta ide na koniec - dostane dni, ktore skorsie varianty nemaju
        var variant = Selected?.Clone() ?? new VariantDraft { Stops = [new StopDraft(), new StopDraft()] };
        variant.Validity = "";
        _variants.Add(variant);
        SelectVariant(_variants.Count - 1);
    }

    private void tsbVarCopy_Click(object? sender, EventArgs e)
    {
        if (Selected is not { } variant)
            return;

        // kopia pred vybranou - v den plati prva varianta, ktora ide, takto sa cast dni oddeli
        var index = SelectedIndex;
        _variants.Insert(index, variant.Clone());
        SelectVariant(index);
    }

    private void tsbVarDelete_Click(object? sender, EventArgs e)
    {
        if (Selected is not { } variant || _variants.Count < 2)
            return;

        var index = SelectedIndex;
        _variants.Remove(variant);
        SelectVariant(Math.Min(index, _variants.Count - 1));
    }

    private void tsbVarUp_Click(object? sender, EventArgs e) => MoveVariant(-1);

    private void tsbVarDown_Click(object? sender, EventArgs e) => MoveVariant(1);

    private void MoveVariant(int by)
    {
        var index = SelectedIndex;
        var target = index + by;
        if (index < 0 || target < 0 || target >= _variants.Count)
            return;

        var variant = _variants[index];
        _variants.RemoveAt(index);
        _variants.Insert(target, variant);
        SelectVariant(target);
    }

    private void SelectVariant(int index)
    {
        if (index >= 0 && index < dgvVariants.Rows.Count)
        {
            dgvVariants.ClearSelection();
            dgvVariants.CurrentCell = dgvVariants.Rows[index].Cells[cVarValidity.Index];
            dgvVariants.Rows[index].Selected = true;
        }

        ShowVariant();
        Recheck();
    }

    private void tsbStopInsert_Click(object? sender, EventArgs e)
    {
        if (Selected is not { } variant)
            return;

        var row = dgvStops.CurrentRow?.Index ?? 0;
        variant.Stops.Insert(Math.Max(0, row), new StopDraft());
        Recheck();
    }

    private void tsbStopAppend_Click(object? sender, EventArgs e)
    {
        if (Selected is not { } variant)
            return;

        variant.Stops.Add(new StopDraft());
        if (dgvStops.Rows.Count > 0)
            dgvStops.CurrentCell = dgvStops.Rows[^1].Cells[0];
        Recheck();
    }

    private void tsbStopDelete_Click(object? sender, EventArgs e)
    {
        if (Selected is not { } variant || dgvStops.CurrentRow is not { Index: >= 0 } row || row.Index >= variant.Stops.Count)
            return;

        variant.Stops.RemoveAt(row.Index);
        Recheck();
    }

    // zaciarkavacie policko sa ma prejavit hned, nie az po opusteni bunky
    private void dgvStops_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
    {
        if (dgvStops.IsCurrentCellDirty && dgvStops.CurrentCell is DataGridViewCheckBoxCell or DataGridViewComboBoxCell)
            dgvStops.CommitEdit(DataGridViewDataErrorContexts.Commit);
    }

    private void dgvStops_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        if (_loading || e.RowIndex < 0)
            return;

        dgvStops.InvalidateRow(e.RowIndex);
        Recheck();
    }

    private void dgvVariants_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0)
            return;

        if (e.ColumnIndex == cVarIndex.Index)
        {
            e.Value = (e.RowIndex + 1).ToString(CultureInfo.CurrentCulture);
            e.FormattingApplied = true;
        }
        else if (e.ColumnIndex == cVarValidity.Index && e.Value is "")
        {
            e.Value = Resources.Validity_Daily;
            e.FormattingApplied = true;
        }
        else if (e.ColumnIndex == cVarDays.Index)
        {
            e.Value = e.RowIndex < _active.Count ? _active[e.RowIndex].Runs.Count(run => run).ToString(CultureInfo.CurrentCulture) : "";
            e.FormattingApplied = true;
        }
    }

    // pravidla, dni variant a pruh kalendara podla aktualneho konceptu
    private void Recheck()
    {
        var problems = TrainRules.Check(_draft, _interestStations, _stationName);
        if (!problems.Any(problem => problem.IsError))
            problems.AddRange(CheckRecords(_draft.ToTrain(_interestStations)));
        var errors = problems.Any(problem => problem.IsError);

        lbProblems.ShowProblems(problems.Select(problem => (problem.IsError, problem.Message)));

        bOk.Enabled = !errors;
        _active = TryTrain() is { } train ? ActiveDays(train) : [];
        dgvVariants.Invalidate();
        UpdateStrip();
        UpdateButtons();
    }

    // dni okna kalendara pre kazdu variantu - v INISS2 sa varianty neprekryvaju, plati prva, ktora ide
    private List<(bool[] Runs, bool Invalid)> ActiveDays(GvTrain train)
    {
        var count = Math.Max(0, WindowEnd.DayNumber - _draft.From.DayNumber + 1);
        return [.. train.Variants.Select(variant =>
        {
            var active = train.ActiveDaysOf(variant);
            var runs = new bool[count];
            for (var i = 0; i < runs.Length && i < active.Length; i++)
                runs[i] = active[i];
            return (runs, GvValidity.Check(train.From, train.To, variant.Validity) != GvValidityProblem.None);
        })];
    }

    private void UpdateStrip()
    {
        if (_active.Count == 0 || _active[0].Runs.Length == 0)
        {
            calendarStrip.SetCalendar(null);
            return;
        }

        var selected = SelectedIndex;
        var rows = _active.Select((variant, index) => new CalendarStripRow(index + 1, variant.Runs, variant.Invalid, index == selected)).ToList();
        calendarStrip.SetCalendar(new CalendarStripData(_draft.From.ToDateTime(TimeOnly.MinValue), WindowEnd.ToDateTime(TimeOnly.MinValue), rows));
        calendarStrip.SelectedPosition = selected + 1;
    }

    private string DayToolTip(DateTime date, IReadOnlyList<CalendarStripRow> running) =>
        running.Count == 0
            ? date.ToString("dddd d. M. yyyy", CultureInfo.CurrentCulture)
            : string.Create(CultureInfo.CurrentCulture, $"{date:dddd d. M. yyyy}: {string.Join(", ", running.Select(row => row.Position))}");

    private void UpdateButtons()
    {
        var index = SelectedIndex;
        tsbVarCopy.Enabled = index >= 0;
        tsbVarDelete.Enabled = index >= 0 && _variants.Count > 1;
        tsbVarUp.Enabled = index > 0;
        tsbVarDown.Enabled = index >= 0 && index < _variants.Count - 1;
        tsbStopInsert.Enabled = tsbStopAppend.Enabled = tsbStopDelete.Enabled = index >= 0;
    }

    private void bOk_Click(object? sender, EventArgs e)
    {
        dgvStops.EndEdit();
        EndRecordEdits();
        if (TryTrain() is not { } train || CheckRecords(train).Any(problem => problem.IsError))
        {
            Utils.ShowWarning(Resources.Edit_Errors);
            return;
        }

        Result = train;
        DialogResult = DialogResult.OK;
    }

    private void FEditTrain_FormClosing(object? sender, FormClosingEventArgs e)
    {
        // rozpisana bunka by sa pri zatvoreni stratila - pri OK sa zapisala uz v bOk_Click
        if (DialogResult != DialogResult.OK)
            dgvStops.CancelEdit();
    }
}
