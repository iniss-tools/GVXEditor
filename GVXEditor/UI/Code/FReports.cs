using System.ComponentModel;
using System.Globalization;
using GVXEditor.Domain;
using GVXEditor.Domain.Code;
using GVXEditor.Domain.Code.Drafts;
using GVXEditor.Domain.Rules;
using GVXEditor.Domain.Views;
using GVXEditor.Properties;
using GVXEditor.UI.Controls;
using Serilog;
using ToolsCore.Iniss2.Grafikon.Script;
using ToolsCore.Tools;

namespace GVXEditor.UI.Code;

/// <summary>
/// Okno Hlasenia: funkcie skriptu hlaseni (LogText.vb) ako udaje. Hlasenie, ktore editor uplne opise, sa upravuje vo
/// formulari a kazda platna zmena hned prepise len jeho funkciu v texte skriptu; rucny kod sa otvori v okne Skript hlaseni.
/// Server skript prelozi cez most chvilu po poslednej zmene.
/// </summary>
internal partial class FReports : Form
{
    private readonly EditorContext _ctx;
    private readonly Iniss2Workspace _workspace;
    private readonly ReportsEdit _edit;
    private readonly IReadOnlyDictionary<string, ReportReferences> _references;
    private readonly SoundBank _bank;
    private readonly StationChoices _stations;
    private readonly CodeCheck _check;
    private readonly BindingList<ValueChoice> _blockLanguages = [];
    private readonly List<ValueChoice> _languages = [];
    private readonly ContextMenuStrip _guardsMenu = new();

    // povodny nazov premenovanych hlaseni (aktualny nazov -> nazov pri otvoreni okna) - dalej k nemu vedu odkazy z dat
    private readonly Dictionary<string, string> _origins = new(StringComparer.OrdinalIgnoreCase);

    // ciele riadkov zoznamu zisteni: funkcia
    private readonly List<string?> _targets = [];
    private List<ReportRow> _rows = [];
    private ReportDraft? _draft;
    private string? _current;

    // okno otvorene na zalozenie noveho hlasenia (napr. dodatku z okna vlaku) a nazov zalozeneho hlasenia
    private readonly ScriptReportKind? _create;
    private string? _created;
    private CodeCheckResult? _result;
    private bool _loading;

    /// <param name="context">kontext editora (otvorena instalacia so skriptom hlaseni)</param>
    /// <param name="text">text skriptu (napr. s neulozenymi zmenami okna vlaku); <see langword="null" /> = skript instalacie</param>
    /// <param name="create">po otvoreni zalozi nove hlasenie tohto druhu; <see langword="null" /> = nic</param>
    public FReports(EditorContext context, string? text = null, ScriptReportKind? create = null)
    {
        ArgumentNullException.ThrowIfNull(context);
        _ctx = context;
        _workspace = context.Workspace ?? throw new InvalidOperationException("Instalacia nie je otvorena.");
        _edit = new ReportsEdit(text ?? _workspace.History.Current.LogText ?? throw new InvalidOperationException("Instalacia nema LogText.vb."));
        _create = create;
        _references = ReportReferences.Of(_workspace.History.Current);
        _bank = _workspace.Bank;
        _stations = StationChoices.Of(_workspace.StationNames, []);

        InitializeComponent();
        // polozky ponuk pred temou, aby dostali pismo a farby okna
        FillBuiltinMenu();
        FillConditionMenus();
        Disposed += (_, _) => _guardsMenu.Dispose();
        this.ApplyThemeAndFonts();
        _check = new CodeCheck(_workspace, this, () => _workspace.History.Current with { LogText = _edit.Result() }, ShowResult);
        Disposed += (_, _) => _check.Dispose();

        _loading = true;
        tbFilter.HintText = Resources.Reports_Filter;
        Bind(cboFilterKind, [new ValueChoice("", Resources.Reports_AllKinds), .. ReportTexts.Kinds()]);
        Bind(cboKind, ReportTexts.Kinds());
        Bind(cboExclusion, ReportTexts.Exclusions());
        _languages.Add(new ValueChoice("", Resources.Reports_AllLanguages));
        foreach (var language in _bank.Languages)
            _languages.Add(new ValueChoice(language.Key, string.Format(CultureInfo.CurrentCulture, Resources.Value_WithKey, language.Name, language.Key)));
        // v bloku len kluc jazyka (stlpec je uzky)
        foreach (var choice in _languages)
            _blockLanguages.Add(choice with { Text = choice.Key.Length == 0 ? Resources.Reports_AnyBlock : choice.Key });
        Bind(cboLanguage, [.. _languages]);

        dgvReports.AutoGenerateColumns = false;
        dgvSteps.AutoGenerateColumns = false;
        cStepKind.ValueMember = nameof(ValueChoice.Key);
        cStepKind.DisplayMember = nameof(ValueChoice.Text);
        cStepKind.DataSource = ReportTexts.StepKinds();
        cStepBlock.ValueMember = nameof(ValueChoice.Key);
        cStepBlock.DisplayMember = nameof(ValueChoice.Text);
        cStepBlock.DataSource = _blockLanguages;
        CellPicker.AttachEditable(dgvSteps, cStepValue, item => PickSound((StepDraft)item));
        _loading = false;

        FillList();
        Recheck();
        _check.Schedule();
    }

    /// <summary>
    /// Text skriptu z okna; bez zmeny vrati povodny text (ten isty objekt).
    /// </summary>
    public string Result() => _edit.Result();

    /// <summary>Nazov hlasenia zalozeneho pri otvoreni okna (parameter <c>create</c>); <see langword="null" /> = ziadne.</summary>
    public string? CreatedName => _created;

    /// <summary>
    /// Premenovane hlasenia, na ktore odkazuju data: povodny nazov → novy nazov.
    /// </summary>
    public IReadOnlyDictionary<string, string> Renames =>
        _origins.Where(pair => !Same(pair.Key, pair.Value) && _references.TryGetValue(pair.Value, out var refs) && refs.Total > 0)
            .ToDictionary(pair => pair.Value, pair => pair.Key, StringComparer.OrdinalIgnoreCase);

    /// <summary>Obsluha potvrdila upravu odkazov v datach na nove nazvy (<see cref="Renames" />).</summary>
    public bool UpdateReferences { get; private set; }

    /// <inheritdoc />
    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        if (_create is { } kind)
            AddReport(NewReport(kind));
        else if (_rows.Count > 0)
            SelectReport(_rows[0].Name);
    }

    // --- zoznam hlaseni ---

    private ScriptReportKind? FilterKind =>
        cboFilterKind.SelectedValue is string key && key.Length > 0 && ScriptReportKinds.TryParse(key, out var kind) ? kind : null;

    private void FillList()
    {
        var kind = FilterKind;
        var filter = tbFilter.Text.Trim();
        _rows =
        [
            .. _edit.Reports.Select(result => new ReportRow(result, References(result.Member.Name)))
                .Where(row => (kind is null || row.Kind == kind)
                              && (filter.Length == 0 || row.Name.Contains(filter, StringComparison.CurrentCultureIgnoreCase)
                                                     || row.Description.Contains(filter, StringComparison.CurrentCultureIgnoreCase)))
        ];

        _loading = true;
        dgvReports.DataSource = _rows;
        _loading = false;
        if (_current is not null && _rows.FindIndex(row => Same(row.Name, _current)) is var index and >= 0)
            SelectRow(index);
        else
            dgvReports.ClearSelection();
    }

    // odkazy z dat vedu k nazvu pri otvoreni okna
    private ReportReferences? References(string name) =>
        _references.TryGetValue(_origins.GetValueOrDefault(name, name), out var refs) ? refs : null;

    private static bool Same(string? first, string? second) => string.Equals(first, second, StringComparison.OrdinalIgnoreCase);

    private void SelectRow(int index)
    {
        if (index < 0 || index >= dgvReports.Rows.Count)
            return;
        dgvReports.CurrentCell = dgvReports.Rows[index].Cells[cRepName.Index];
    }

    // vyberie hlasenie v zozname (filter, ktory ho skryva, sa zrusi)
    private void SelectReport(string name)
    {
        var index = _rows.FindIndex(row => Same(row.Name, name));
        if (index < 0)
        {
            _loading = true;
            cboFilterKind.SelectedIndex = 0;
            tbFilter.Text = "";
            _loading = false;
            FillList();
            index = _rows.FindIndex(row => Same(row.Name, name));
        }

        SelectRow(index);
        if (!Same(_current, name))
            ShowReport(name);
    }

    private void Filter_Changed(object? sender, EventArgs e)
    {
        if (!_loading)
            FillList();
    }

    private void dgvReports_SelectionChanged(object? sender, EventArgs e)
    {
        if (_loading || dgvReports.CurrentRow is not { Index: >= 0 } row || row.Index >= _rows.Count || Same(_rows[row.Index].Name, _current))
            return;

        // koncept s chybou sa do skriptu nezapisal - po prepnuti by sa jeho uprava stratila
        if (_draft is not null && CheckDraft().Any(problem => problem.IsError))
            Utils.ShowWarning(string.Format(CultureInfo.CurrentCulture, Resources.Reports_NotWritten, _draft.Name));
        ShowReport(_rows[row.Index].Name);
    }

    private void dgvReports_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex >= 0 && e.RowIndex < _rows.Count)
            dgvReports.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = _rows[e.RowIndex].ToolTip;
    }

    // --- hlasenie ---

    private void ShowReport(string name)
    {
        dgvSteps.CancelEdit();
        _current = name;
        var result = _edit.Find(name);
        _draft = result?.Report is { } report ? ReportDraft.Of(report) : null;
        pReport.Visible = _draft is not null;
        pManual.Visible = _draft is null && result is not null;
        if (_draft is not null)
            LoadDraft(_draft);
        else if (result is not null)
            lManual.Text = string.Format(CultureInfo.CurrentCulture, Resources.Reports_ManualText, result.Member.Name, ReportTexts.Manual(result.Reason),
                result.Line + 1);

        UpdateCommands();
        Recheck();
        ShowPreview();
    }

    private void LoadDraft(ReportDraft draft)
    {
        _loading = true;
        tbName.Text = draft.Name;
        cboKind.SelectedValue = ScriptReportKinds.AttributeName(draft.Kind);
        tbDescription.Text = draft.Description;
        cboExclusion.SelectedValue = Ensure(cboExclusion, draft.ExclusionType);
        cboLanguage.SelectedValue = Ensure(cboLanguage, draft.OnlyLanguage);
        tbGuards.Text = draft.Guards.ReplaceLineEndings(Environment.NewLine);
        cboxSection.Checked = draft.Section;
        foreach (var step in draft.Steps)
            EnsureBlock(step.Block);
        dgvSteps.DataSource = draft.Steps;
        ShowStations(draft);
        _loading = false;
    }

    private static void Bind(ComboBox combo, List<ValueChoice> choices)
    {
        combo.ValueMember = nameof(ValueChoice.Key);
        combo.DisplayMember = nameof(ValueChoice.Text);
        combo.DataSource = choices;
    }

    // hodnota zo suboru, ktoru zoznam nema, sa do zoznamu doplni
    private static string Ensure(ComboBox combo, string key)
    {
        if (combo.DataSource is List<ValueChoice> choices && !choices.Any(choice => choice.Key == key))
        {
            choices.Add(new ValueChoice(key, key));
            combo.DataSource = null;
            combo.DataSource = choices;
        }

        return key;
    }

    private void EnsureBlock(string key)
    {
        if (!_blockLanguages.Any(choice => choice.Key == key))
            _blockLanguages.Add(new ValueChoice(key, key));
    }

    // obmedzenie na stanice podla druhu: vyluka ma v prvom riadku stanice oznacenia, druhy bez obmedzenia nic
    private void ShowStations(ReportDraft draft)
    {
        var exclusion = draft.Kind == ScriptReportKind.Exclusion;
        var limit = ScriptReportKinds.HasStationLimit(draft.Kind);
        lForStations.Text = exclusion ? Resources.Reports_ExclusionStations : Resources.Reports_ForStations;
        tbForStations.Text = exclusion ? draft.ExclusionStations : draft.ForStations;
        tbNotStations.Text = draft.NotStations;
        tbForStations.Enabled = bForStations.Enabled = limit || exclusion;
        tbNotStations.Enabled = bNotStations.Enabled = limit;
        cboExclusion.Enabled = ReportDraft.IsExclusion(draft.Kind);
    }

    private void Report_Changed(object? sender, EventArgs e)
    {
        if (_loading || _draft is null)
            return;

        var kind = cboKind.SelectedValue is string key && ScriptReportKinds.TryParse(key, out var parsed) ? parsed : _draft.Kind;
        _draft.Name = tbName.Text;
        _draft.Description = tbDescription.Text;
        if (_draft.Kind == ScriptReportKind.Exclusion)
            _draft.ExclusionStations = tbForStations.Text;
        else
            _draft.ForStations = tbForStations.Text;
        _draft.NotStations = tbNotStations.Text;
        _draft.ExclusionType = cboExclusion.SelectedValue as string ?? "";
        _draft.OnlyLanguage = cboLanguage.SelectedValue as string ?? "";
        _draft.Guards = tbGuards.Text;
        _draft.Section = cboxSection.Checked;
        if (kind != _draft.Kind)
        {
            _draft.Kind = kind;
            if (ReportDraft.IsExclusion(kind) && _draft.ExclusionType.Length == 0)
                _draft.ExclusionType = ReportTexts.DefaultExclusionType;
            _loading = true;
            cboExclusion.SelectedValue = _draft.ExclusionType;
            ShowStations(_draft);
            _loading = false;
        }

        Apply();
    }

    private List<SetupProblem> CheckDraft() =>
        _draft is null
            ? []
            : ReportRules.Check(_draft, name => _edit.Script.Find(name) is { } member && !Same(member.Name, _current), name => _edit.Script.Find(name) is not null,
                _bank);

    // platny koncept sa hned zapise do textu skriptu (len jeho funkcia); koncept s chybou sa nezapise
    private void Apply()
    {
        if (_draft is not null && _current is not null && !CheckDraft().Any(problem => problem.IsError) && _draft.Build() is { } report
            && !report.Equals(_edit.Find(_current)?.Report))
        {
            var previous = _current;
            _edit.Write(report, previous);
            _current = report.Name;
            if (!Same(previous, report.Name))
            {
                var origin = _origins.Remove(previous, out var first) ? first : previous;
                if (!Same(origin, report.Name))
                    _origins[report.Name] = origin;
            }

            if (Same(previous, _created))
                _created = report.Name;
            UpdateRow(previous);
            _check.Schedule();
            lCheck.Text = CodeWindow.Status(_check, _result);
        }

        Recheck();
        ShowPreview();
    }

    // riadok zoznamu po zapise hlasenia (aj pri premenovani)
    private void UpdateRow(string previous)
    {
        var index = _rows.FindIndex(row => Same(row.Name, previous));
        if (index < 0 || _current is null || _edit.Find(_current) is not { } result)
            return;
        _rows[index] = new ReportRow(result, References(result.Member.Name));
        dgvReports.InvalidateRow(index);
    }

    private void ShowPreview()
    {
        var report = _draft?.Build() ?? (_current is null ? null : _edit.Find(_current)?.Report);
        tbPreview.Text = report is null
            ? ""
            : string.Join(Environment.NewLine, ReportPreview.Texts(report, _bank).Select(item => item.Language + ": " + (item.Text ?? Resources.Reports_NotInBank)));
    }

    // --- kroky ---

    private StepDraft? CurrentStep =>
        _draft is not null && dgvSteps.CurrentRow is { Index: >= 0 } row && row.Index < _draft.Steps.Count ? _draft.Steps[row.Index] : null;

    private void Steps_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
    {
        // vyber v bunke sa prejavi hned
        if (dgvSteps.IsCurrentCellDirty && dgvSteps.CurrentCell is DataGridViewComboBoxCell)
            dgvSteps.CommitEdit(DataGridViewDataErrorContexts.Commit);
    }

    private void Steps_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        if (_loading || e.RowIndex < 0)
            return;
        if (e.ColumnIndex == cStepKind.Index && _draft is not null && e.RowIndex < _draft.Steps.Count)
            NewKind(_draft.Steps[e.RowIndex]);
        dgvSteps.InvalidateRow(e.RowIndex);
        Apply();
    }

    // novy druh kroku: hodnota podla druhu (vstavana polozka dostane povinne argumenty, pauzy pri zvuku len zvuk)
    private void NewKind(StepDraft step)
    {
        if (step.Kind != StepDraft.SoundKind)
            step.Before = step.After = "";
        if (step.Builtin is { } builtin)
            step.Value = StepDraft.Template(builtin, ScriptReportKinds.IsPattern(_draft!.Kind) ? ReportDraft.DefaultStationsParameter : null);
        else if (step.Kind == StepDraft.PauseKind)
            step.Value = StepDraft.Pause(step.Value) is null || step.Value.Trim().Length == 0 ? "200" : step.Value;
        else if (step.Kind is StepDraft.SoundKind or StepDraft.CallKind)
            step.Value = "";
        _draft!.Steps.ResetItem(_draft.Steps.IndexOf(step));
    }

    // hodnota, ktoru tabulka nevie zobrazit, nie je chyba okna - ukaze ju kontrola
    private void Steps_DataError(object? sender, DataGridViewDataErrorEventArgs e) => e.ThrowException = false;

    // text zvuku z banky (zvuk bez jazyka v jazyku bloku alebo v prvom jazyku banky)
    private void Steps_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.ColumnIndex != cStepText.Index || _draft is null || e.RowIndex < 0 || e.RowIndex >= _draft.Steps.Count)
            return;
        var step = _draft.Steps[e.RowIndex];
        e.Value = step.Kind switch
        {
            StepDraft.SoundKind => SoundText(step),
            StepDraft.CallKind => CallText(step),
            StepDraft.PauseKind or StepDraft.OffKind => "",
            _ => step.Builtin is { } builtin ? ReportTexts.Parameters(builtin) : ""
        };
        e.FormattingApplied = true;
    }

    // volana funkcia: popis hlasenia, pomocna funkcia alebo funkcia, ktora v skripte nie je
    private string CallText(StepDraft step)
    {
        if (step.Build() is not CallStep call)
            return "";
        if (_edit.Find(call.Function) is { } report)
            return string.Format(CultureInfo.CurrentCulture, Resources.Reports_CallReport, new ReportRow(report, null).Description);
        return _edit.Script.Find(call.Function) is null ? Resources.Reports_CallMissing : Resources.Reports_CallHelper;
    }

    private string SoundText(StepDraft step)
    {
        var path = step.Value.Trim();
        if (path.Split('/').Length == 2)
        {
            var language = step.Block.Trim().Length > 0 ? step.Block.Trim() : _bank.Languages.Count > 0 ? _bank.Languages[0].Key : "";
            path = language + "/" + path;
        }

        return _bank.Find(path)?.Text ?? "";
    }

    // zvuk v bunke: vyber z banky (prvy vybrany zvuk), pri inom kroku sa nic nevybera
    private string? PickSound(StepDraft step)
    {
        if (step.Kind != StepDraft.SoundKind)
            return null;
        var paths = FPickSound.PickList(this, _bank, Resources.Reports_PickSounds, step.Value.Trim().Length > 0 ? [step.Value.Trim()] : []);
        return paths is { Count: > 0 } ? paths[0] : null;
    }

    private void InsertSteps(IEnumerable<StepDraft> steps)
    {
        if (_draft is null)
            return;
        dgvSteps.EndEdit();
        var index = CurrentStep is null ? _draft.Steps.Count : dgvSteps.CurrentRow!.Index + 1;
        var first = index;
        foreach (var step in steps)
            _draft.Steps.Insert(index++, step);
        if (index > first)
            dgvSteps.CurrentCell = dgvSteps.Rows[index - 1].Cells[cStepValue.Index];
        Apply();
    }

    private void tsbAddSounds_Click(object? sender, EventArgs e)
    {
        if (_draft is null || FPickSound.PickList(this, _bank, Resources.Reports_PickSounds, []) is not { Count: > 0 } paths)
            return;
        var block = CurrentStep?.Block ?? "";
        InsertSteps(paths.Select(path => new StepDraft { Block = block, Value = path }));
    }

    private void tsbAddPause_Click(object? sender, EventArgs e) =>
        InsertSteps([new StepDraft { Block = CurrentStep?.Block ?? "", Kind = StepDraft.PauseKind, Value = "200" }]);

    private void tsbAddCall_Click(object? sender, EventArgs e)
    {
        InsertSteps([new StepDraft { Block = CurrentStep?.Block ?? "", Kind = StepDraft.CallKind }]);
        dgvSteps.BeginEdit(true);
    }

    // logicke funkcie skriptu bez parametrov (kontroly), ktore sa daju pouzit v podmienke
    private List<string> CheckFunctions() =>
    [
        .. _edit.Script.Members
            .Where(member => member.Keyword.Equals("Function", StringComparison.OrdinalIgnoreCase) && string.IsNullOrEmpty(member.Parameters)
                             && member.Rest.Equals("As Boolean", StringComparison.OrdinalIgnoreCase))
            .Select(member => member.Name)
    ];

    // ponuky podmienok: podmienka vybranych krokov (panel krokov) a stráze hlasenia (tlacidlo … pri Nevyhlasit, ked)
    private void FillConditionMenus()
    {
        tsbCondition.DropDownItems.Add(Resources.Reports_NoCondition, null, (_, _) => SetCondition(_ => ""));
        tsbCondition.DropDownItems.Add(Resources.Reports_ElseBranch, null, (_, _) => SetCondition(ToggleElse));
        tsbCondition.DropDownItems.Add(new ToolStripSeparator());
        foreach (var choice in ReportTexts.Conditions())
        {
            tsbCondition.DropDownItems.Add(choice.Text, null, (_, _) => SetCondition(_ => choice.Key));
            _guardsMenu.Items.Add(choice.Text, null, (_, _) => AddGuard(choice.Key));
        }

        bGuards.ContextMenuStrip = _guardsMenu;
        var functions = CheckFunctions();
        if (functions.Count == 0)
            return;
        var conditionFunctions = new ToolStripMenuItem(Resources.Reports_ScriptFunctions);
        var guardFunctions = new ToolStripMenuItem(Resources.Reports_ScriptFunctions);
        foreach (var function in functions)
        {
            conditionFunctions.DropDownItems.Add(function + "()", null, (_, _) => SetCondition(_ => function + "()"));
            guardFunctions.DropDownItems.Add("Not " + function + "()", null, (_, _) => AddGuard("Not " + function + "()"));
        }

        tsbCondition.DropDownItems.Add(new ToolStripSeparator());
        tsbCondition.DropDownItems.Add(conditionFunctions);
        _guardsMenu.Items.Add(new ToolStripSeparator());
        _guardsMenu.Items.Add(guardFunctions);
    }

    private void bGuards_Click(object? sender, EventArgs e) => _guardsMenu.Show(bGuards, new Point(0, bGuards.Height));

    private void AddGuard(string expression)
    {
        var text = tbGuards.Text.TrimEnd();
        tbGuards.Text = text.Length == 0 ? expression : text + Environment.NewLine + expression;
    }

    // vetva Else poslednej podmienky kroku (druhy klik ju vrati)
    private static string ToggleElse(string condition)
    {
        if (StepDraft.Conditions(condition) is not { Count: > 0 } chain)
            return condition;
        chain[^1] = chain[^1] with { Else = !chain[^1].Else };
        return StepDraft.ConditionText(chain);
    }

    // podmienka vybranych krokov (riadky s vybranou bunkou alebo aktualny riadok)
    private void SetCondition(Func<string, string> change)
    {
        if (_draft is null)
            return;
        dgvSteps.EndEdit();
        var rows = dgvSteps.SelectedCells.Cast<DataGridViewCell>().Select(cell => cell.RowIndex)
            .Append(dgvSteps.CurrentRow?.Index ?? -1).Where(index => index >= 0 && index < _draft.Steps.Count).Distinct().ToList();
        foreach (var row in rows)
        {
            _draft.Steps[row].Condition = change(_draft.Steps[row].Condition);
            _draft.Steps.ResetItem(row);
        }

        if (rows.Count > 0)
            Apply();
    }

    // ponuka vstavanych poloziek: kazda vlozi krok s povinnymi argumentmi
    private void FillBuiltinMenu()
    {
        foreach (var builtin in ScriptBuiltins.All)
        {
            var item = new ToolStripMenuItem(string.Format(CultureInfo.CurrentCulture, Resources.Value_WithKey, ReportTexts.Builtin(builtin.Method), builtin.Method))
            {
                ToolTipText = ReportTexts.Parameters(builtin)
            };
            item.Click += (_, _) => InsertSteps([new StepDraft
            {
                Block = CurrentStep?.Block ?? "",
                Kind = builtin.Method,
                Value = StepDraft.Template(builtin, _draft is not null && ScriptReportKinds.IsPattern(_draft.Kind) ? ReportDraft.DefaultStationsParameter : null)
            }]);
            tsbAddBuiltin.DropDownItems.Add(item);
        }
    }

    private void tsbToggleStep_Click(object? sender, EventArgs e)
    {
        dgvSteps.EndEdit();
        if (CurrentStep is not { } step || !step.Toggle())
            return;
        _draft!.Steps.ResetItem(dgvSteps.CurrentRow!.Index);
        Apply();
    }

    private void tsbDeleteStep_Click(object? sender, EventArgs e)
    {
        if (_draft is null || CurrentStep is null)
            return;
        dgvSteps.CancelEdit();
        _draft.Steps.RemoveAt(dgvSteps.CurrentRow!.Index);
        Apply();
    }

    private void tsbStepUp_Click(object? sender, EventArgs e) => MoveStep(-1);

    private void tsbStepDown_Click(object? sender, EventArgs e) => MoveStep(1);

    private void MoveStep(int by)
    {
        if (_draft is null || CurrentStep is not { } step)
            return;
        var index = dgvSteps.CurrentRow!.Index;
        var target = index + by;
        if (target < 0 || target >= _draft.Steps.Count)
            return;
        dgvSteps.EndEdit();
        _draft.Steps.RemoveAt(index);
        _draft.Steps.Insert(target, step);
        dgvSteps.CurrentCell = dgvSteps.Rows[target].Cells[cStepValue.Index];
        Apply();
    }

    // --- stanice ---

    private void bForStations_Click(object? sender, EventArgs e) => PickStations(tbForStations);

    private void bNotStations_Click(object? sender, EventArgs e) => PickStations(tbNotStations);

    private void PickStations(TextBox box)
    {
        var selected = box.Text.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        foreach (var number in selected)
            _stations.Ensure(number);
        if (FPickOrdered.Pick(this, Resources.Pick_StationsTitle, [.. _stations.Items.Where(choice => choice.Key.Length > 0)], selected) is { } picked)
            box.Text = string.Join(",", picked);
    }

    // --- nove, kopia, zmazanie, rucny kod ---

    private void UpdateCommands()
    {
        tsbCopy.Enabled = _draft is not null;
        tsbDelete.Enabled = tsbOpenCode.Enabled = _current is not null && _edit.Find(_current) is not null;
    }

    private void tsbNew_Click(object? sender, EventArgs e) => AddReport(NewReport(FilterKind ?? ScriptReportKind.Other));

    // nove hlasenie druhu: dodatok dostane dalsie volne cislo D<n> ako v datach, ostatne nazov z popisu
    private ScriptReport NewReport(ScriptReportKind kind) =>
        new(kind == ScriptReportKind.Addition ? _edit.NextAdditionName() : _edit.UniqueName(Resources.Reports_NewName), kind)
        {
            Description = Resources.Reports_NewDescription,
            Arguments = ReportDraft.IsExclusion(kind) ? [ReportTexts.DefaultExclusionType] : [],
            StationsParameter = ScriptReportKinds.IsPattern(kind) ? ReportDraft.DefaultStationsParameter : null
        };

    private void tsbCopy_Click(object? sender, EventArgs e)
    {
        if (_current is null || _edit.Find(_current)?.Report is not { } report)
            return;
        AddReport(report with { Name = _edit.UniqueName(report.Name + Resources.Reports_CopySuffix) });
    }

    private void AddReport(ScriptReport report)
    {
        try
        {
            _edit.Write(report, null);
        }
        catch (FormatException exception)
        {
            Log.Warning(exception, "Hlasenie {Name} sa do skriptu instalacie {DataDir} nedalo pridat", report.Name, _workspace.DataDir);
            Utils.ShowError(Resources.Code_NoLogText);
            return;
        }

        if (_create is not null && _created is null)
            _created = report.Name;
        FillList();
        SelectReport(report.Name);
        _check.Schedule();
        tbDescription.Focus();
        tbDescription.SelectAll();
    }

    private void tsbDelete_Click(object? sender, EventArgs e)
    {
        if (_current is null || _edit.Find(_current) is not { } result)
            return;

        var name = result.Member.Name;
        var refs = References(name);
        var question = result.Report is null ? string.Format(CultureInfo.CurrentCulture, Resources.Reports_DeleteManual, name)
            : refs is { Total: > 0 } ? string.Format(CultureInfo.CurrentCulture, Resources.Reports_DeleteUsed, name, refs.Describe())
            : string.Format(CultureInfo.CurrentCulture, Resources.Reports_DeleteConfirm, name);
        if (Utils.ShowQuestion(question) != DialogResult.Yes)
            return;

        var index = _rows.FindIndex(row => Same(row.Name, name));
        _edit.Remove(name);
        _origins.Remove(name);
        if (Same(name, _created))
            _created = null;
        _current = null;
        _draft = null;
        FillList();
        if (_rows.Count > 0)
            SelectReport(_rows[Math.Clamp(index, 0, _rows.Count - 1)].Name);
        else
            ShowReport("");
        _check.Schedule();
    }

    private void tsbOpenCode_Click(object? sender, EventArgs e)
    {
        dgvSteps.EndEdit();
        var line = _current is not null && _edit.Script.Find(_current) is { } member ? member.Start + 1 : 0;
        using var form = new FLogText(_ctx, _edit.Text, line);
        if (form.ShowDialog(this) != DialogResult.OK || form.Result() == _edit.Text)
            return;

        _edit.Load(form.Result());
        var name = _current;
        _current = null;
        FillList();
        if (name is not null && _edit.Find(name) is not null)
            SelectReport(name);
        else if (_rows.Count > 0)
            SelectReport(_rows[0].Name);
        _check.Schedule();
    }

    // --- kontrola ---

    private void Recheck()
    {
        var problems = new List<(bool IsError, string Message, string? Target)>();
        problems.AddRange(CheckDraft().Select(problem => (problem.IsError, problem.Message, _current)));
        foreach (var (name, origin) in _origins)
        {
            if (_references.TryGetValue(origin, out var refs) && refs.Total > 0)
                problems.Add((false, string.Format(CultureInfo.CurrentCulture, Resources.Reports_RenamedUsed,
                    string.Format(CultureInfo.CurrentCulture, Resources.Reports_Where, name), origin, refs.Describe()), name));
        }

        foreach (var place in _result?.Errors.Where(place => place.IsLogText) ?? [])
        {
            var member = _edit.Script.Members.FirstOrDefault(item => place.Line - 1 >= item.Start && place.Line - 1 <= item.End);
            var where = member is null
                ? string.Format(CultureInfo.CurrentCulture, Resources.Loc_Line, Resources.Page_LogText, place.Line)
                : string.Format(CultureInfo.CurrentCulture, Resources.Loc_Line,
                    string.Format(CultureInfo.CurrentCulture, Resources.Reports_Where, member.Name), place.Line);
            problems.Add((!place.IsWarning, CodeWindow.Message(place, where), member?.Name));
        }

        var ordered = problems.OrderBy(problem => problem.IsError ? 0 : 1).ToList();
        _targets.Clear();
        _targets.AddRange(ordered.Select(problem => problem.Target));
        lbProblems.ShowProblems(ordered.Select(problem => (problem.IsError, problem.Message)));
    }

    private void ShowResult(CodeCheckResult result)
    {
        _result = result;
        lCheck.Text = CodeWindow.Status(_check, result);
        Recheck();
    }

    private void lbProblems_DoubleClick(object? sender, EventArgs e)
    {
        if (lbProblems.SelectedIndex < 0 || lbProblems.SelectedIndex >= _targets.Count || _targets[lbProblems.SelectedIndex] is not { } name)
            return;
        if (_edit.Find(name) is not null)
            SelectReport(name);
    }

    private void bOk_Click(object? sender, EventArgs e)
    {
        dgvSteps.EndEdit();
        if (CheckDraft().Any(problem => problem.IsError)
            && Utils.ShowQuestion(string.Format(CultureInfo.CurrentCulture, Resources.Reports_ConfirmNotWritten, _draft?.Name)) != DialogResult.Yes)
            return;

        if ((_result?.Errors.Any(place => place.IsLogText && !place.IsWarning) ?? false) && Utils.ShowQuestion(Resources.Code_ConfirmCompile) != DialogResult.Yes)
            return;

        // odkazy v datach upravi hlavne okno (pri zalozeni z okna vlaku nie)
        if (_create is null && Renames is { Count: > 0 } renames)
        {
            var list = string.Join(", ", renames.Select(pair => pair.Key + " → " + pair.Value));
            UpdateReferences = Utils.ShowQuestion(string.Format(CultureInfo.CurrentCulture, Resources.Reports_UpdateReferences, list)) == DialogResult.Yes;
        }

        DialogResult = DialogResult.OK;
    }
}
