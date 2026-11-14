using System.Collections.Immutable;
using System.Globalization;
using GVXEditor.Domain.Stations.Drafts;
using GVXEditor.Domain.Views;
using GVXEditor.Properties;
using GVXEditor.UI.Controls;
using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.UI.EditTrain;

// vyber odkazov namiesto pisania klucov: stanica nazvom, kluce dodatkov, vyluk a odklonov s popisom, zoznamy stanic,
// jazykov a typov hlasenia v okne vyberu
internal partial class FEditTrain
{
    private const string AllLanguages = "1";
    private const string NoReport = "-";

    private StationChoices _stations = null!;

    private void InitPickers()
    {
        var names = _ctx.Workspace?.StationNames ?? ImmutableDictionary<int, string>.Empty;
        _stations = StationChoices.Of(names, _draft.Variants.SelectMany(variant => variant.Stops).Select(stop => stop.Station));
        _stations.Bind(dgvStops, cStationName);
        _stations.Bind(dgvCompositions, cCompEnd);

        // druh vlaku z TrTypes.xml a druhy, ktore uz vo vlaku su
        var types = (_ctx.Workspace?.TrainTypes ?? []).Concat(_draft.Variants.SelectMany(variant => variant.Stops).Select(stop => stop.TrainType)).Prepend("")
            .Distinct(StringComparer.Ordinal).Select(key => new ValueChoice(key, key)).ToList();
        Choices(cType, types);

        CellPicker.Attach(dgvAdditions, cAddReports, item => PickReports(((AdditionDraft)item).Reports), item => ReportsText(((AdditionDraft)item).Reports));
        CellPicker.Attach(dgvCompositions, cCompReports, item => PickReports(((CompositionDraft)item).Reports),
            item => ReportsText(((CompositionDraft)item).Reports));
        CellPicker.Attach(dgvCompositions, cCompSounds, item => PickSounds(((CompositionDraft)item).Sounds), item => SoundsText(((CompositionDraft)item).Sounds));
        CellPicker.Attach(dgvLanguages, cLangLanguages, item => PickLanguages(((LanguagesDraft)item).Languages),
            item => LanguagesText(((LanguagesDraft)item).Languages));
        foreach (var (column, value) in new (DataGridViewColumn, Func<ReportStationsDraft, string>)[]
                 {
                     (cRepShortFrom, draft => draft.ShortFrom), (cRepLongFrom, draft => draft.LongFrom), (cRepShortTo, draft => draft.ShortTo),
                     (cRepLongTo, draft => draft.LongTo)
                 })
        {
            CellPicker.Attach(dgvReportStations, column, item => PickStations(column.HeaderText, value((ReportStationsDraft)item)),
                item => StationsText(value((ReportStationsDraft)item)));
        }
    }

    // kluce dodatkov podla stanice: funkcie LogText s oznacenim Dodatek, hlasenia obsluhy a kluce zo suboru
    private void RefreshAdditionKeys()
    {
        var present = _station?.Additions.Select(addition => addition.Key) ?? [];
        Choices(cAddKey, _keyCatalog.Choices("Dodatek", present));
    }

    /// <summary>
    /// Skript hlaseni s dodatkami zalozenymi z okna; <see langword="null" />, ak sa skript nezmenil.
    /// </summary>
    public string? LogText => _logText;

    // novy dodatok v okne Hlasenia (nad skriptom s neulozenymi dodatkami okna) a zaznam s jeho klucom
    private void tsbAdditionsNew_Click(object? sender, EventArgs e)
    {
        if (_ctx.Workspace is not { } workspace || (_logText ?? workspace.History.Current.LogText) is not { } text)
            return;

        dgvAdditions.EndEdit();
        using var form = new UI.Code.FReports(_ctx, text, ToolsCore.Iniss2.Grafikon.Script.ScriptReportKind.Addition);
        if (form.ShowDialog(this) != DialogResult.OK)
            return;

        var updated = form.Result();
        if (updated != text)
        {
            _logText = updated;
            var lines = updated.Split('\n');
            _keyCatalog = new Domain.Stations.KeyCatalog(LogTextFile.Functions(lines), LogTextFile.Descriptions(lines), workspace.ConstReports);
            RefreshAdditionKeys();
        }

        if (form.CreatedName is not { } name || dgvAdditions.DataSource is not System.Collections.IList list)
            return;
        tsbRecordAdd_Click(tsbAdditionsAdd, EventArgs.Empty);
        if (list.Count > 0 && list[list.Count - 1] is AdditionDraft addition)
        {
            addition.Key = name;
            dgvAdditions.InvalidateRow(list.Count - 1);
            Recheck();
        }
    }

    private void RefreshNoticeKeys()
    {
        if (_notices is null)
            return;
        Choices(cLockKey, _keyCatalog.Choices("Vyluka", _notices.Lockouts.Select(lockout => lockout.Key)));
        Choices(cDivKey, _keyCatalog.Choices("Odklon", _notices.Diversions.Select(diversion => diversion.Key)));
    }

    private static void Choices(DataGridViewComboBoxColumn column, List<ValueChoice> choices)
    {
        column.DataSource = null;
        column.ValueMember = nameof(ValueChoice.Key);
        column.DisplayMember = nameof(ValueChoice.Text);
        column.DataSource = choices;
    }

    private static string[] Items(string text) => text.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    // --- stanice v hlaseni ---

    private string StationsText(string value) => string.Join(", ", Items(value).Select(_stations.Text));

    private string? PickStations(string title, string value) =>
        FPickOrdered.Pick(this, title, [.. _stations.Items.Where(choice => choice.Key.Length > 0)], Items(value)) is { } picked
            ? string.Join(" ", picked)
            : null;

    // --- typy hlasenia: pismena, prazdne = vsetky, pomlcka = ziadny ---

    private List<ValueChoice> ReportChoices() =>
    [
        .. _station?.Station.Setup?.ReportKeys.Select(key => new ValueChoice(key.Key, string.Format(CultureInfo.CurrentCulture, Resources.Value_WithKey, key.Name, key.Key))) ?? [],
        new ValueChoice(NoReport, Resources.Pick_NoReport)
    ];

    private string ReportsText(string value) => value switch
    {
        "" => Resources.Pick_AllReports,
        NoReport => Resources.Pick_NoReport,
        _ => string.Join(", ", value.Select(letter => letter.ToString())
            .Select(letter => _station?.Station.Setup?.ReportKeys.FirstOrDefault(key => key.Key == letter)?.Name ?? letter))
    };

    private string? PickReports(string value)
    {
        var selected = value == NoReport ? [NoReport] : value.Select(letter => letter.ToString());
        if (FPickChecked.Pick(this, Resources.Pick_ReportsTitle, Resources.Pick_ReportsHint, ReportChoices(), selected) is not { } picked)
            return null;
        // ziadny typ ma prednost pred pismenami
        return picked.Contains(NoReport) ? NoReport : string.Concat(picked);
    }

    // --- zvuky radenia: cesty skupina/zvuk (s jazykom jazyk/skupina/zvuk) v poradi prehravania ---

    private string SoundsText(string value) => _ctx.Workspace is { } workspace
        ? string.Join(", ", Items(value).Select(path => workspace.Bank.Find(path) is { Text.Length: > 0 } sound ? sound.Text : path))
        : value;

    private string? PickSounds(string value) =>
        _ctx.Workspace is { } workspace && FPickSound.PickList(this, workspace.Bank, Resources.Pick_SoundsTitle, Items(value)) is { } picked
            ? string.Join(" ", picked)
            : null;

    // --- jazyky: kluce banky v poradi, 1 = vsetky jazyky banky ---

    private string LanguagesText(string value) => string.Join(", ", Items(value).Select(key => key == AllLanguages
        ? Resources.Pick_AllLanguages
        : _ctx.Workspace?.Bank.Languages.FirstOrDefault(language => string.Equals(language.Key, key, StringComparison.OrdinalIgnoreCase))?.Name ?? key));

    private string? PickLanguages(string value)
    {
        List<ValueChoice> languages =
        [
            new(AllLanguages, Resources.Pick_AllLanguages),
            .. _ctx.Workspace?.Bank.Languages.Select(language => new ValueChoice(language.Key,
                string.Format(CultureInfo.CurrentCulture, Resources.Value_WithKey, language.Name, language.Key))) ?? []
        ];
        return FPickOrdered.Pick(this, Resources.Pick_LanguagesTitle, languages, Items(value)) is { } picked ? string.Join(" ", picked) : null;
    }
}
