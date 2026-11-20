using System.Globalization;
using GVXEditor.Domain;
using GVXEditor.Domain.Stations.Texts;
using GVXEditor.Properties;
using ToolsCore.Tools;

namespace GVXEditor.UI.Boards;

/// <summary>
/// Generovanie textov vlakov do sady TTVlaky.xml stanice (cielova a vychodzia stanica, stanice do smeru a zo smeru) s nahladom,
/// co sa zapise. Zmena sa preberie cez <see cref="Apply" /> ako jeden krok historie.
/// </summary>
internal partial class FGenerateTexts : Form
{
    private readonly GroupDocument _group;
    private readonly Func<int, string> _stationName;
    private readonly bool _defaultsFile;
    private List<GeneratedText> _generated = [];
    private bool _loading;

    /// <param name="group">grafikon</param>
    /// <param name="station">poradie stanice vybranej v strome; -1 = prva stanica s textami</param>
    /// <param name="stationName">nazov stanice podla cisla</param>
    /// <param name="defaultsFile">grafikon ma Default.Stations.xml (predvolene stanice dlheho hlasenia)</param>
    public FGenerateTexts(GroupDocument group, int station, Func<int, string> stationName, bool defaultsFile)
    {
        ArgumentNullException.ThrowIfNull(group);
        _group = group;
        _stationName = stationName;
        _defaultsFile = defaultsFile;
        InitializeComponent();
        dgvPreview.AutoGenerateColumns = false;
        this.ApplyThemeAndFonts();
        Text = string.Format(CultureInfo.CurrentCulture, "{0} {1}", Text, group.Group.Name);

        _loading = true;
        cboKind.DisplayMember = nameof(Item<GeneratedTextKind>.Text);
        cboKind.DataSource = new List<Item<GeneratedTextKind>>
        {
            new(GeneratedTextKind.Destination, Resources.Texts_KindDestination),
            new(GeneratedTextKind.Origin, Resources.Texts_KindOrigin),
            new(GeneratedTextKind.StationsTo, Resources.Texts_KindTo),
            new(GeneratedTextKind.StationsFrom, Resources.Texts_KindFrom)
        };
        cboStation.DisplayMember = nameof(Item<int>.Text);
        var stations = group.Stations.Select((document, index) => (document, index)).Where(item => item.document.Files is not null)
            .Select(item => new Item<int>(item.index, item.document.Station.Directory)).ToList();
        cboStation.DataSource = stations;
        cboSet.DisplayMember = nameof(Item<TextSetChoice>.Text);
        if (stations.FindIndex(item => item.Value == station) is >= 0 and var selected)
            cboStation.SelectedIndex = selected;
        _loading = false;
        ShowSets();
    }

    /// <summary>Poradie stanice, do ktorej sa generuje; -1 = ziadna.</summary>
    public int StationIndex => cboStation.SelectedItem is Item<int> item ? item.Value : -1;

    private TextSetChoice? Set => cboSet.SelectedItem is Item<TextSetChoice> item ? item.Value : null;

    private GeneratedTextKind Kind => cboKind.SelectedItem is Item<GeneratedTextKind> item ? item.Value : GeneratedTextKind.StationsTo;

    /// <summary>
    /// Grafikon s vygenerovanymi textmi v sade stanice; ten isty, ak sa nic nezapise.
    /// </summary>
    /// <returns>grafikon, pocet zapisanych textov a nazov sady</returns>
    public (GroupDocument Group, int Written, string Set) Apply(GroupDocument group)
    {
        ArgumentNullException.ThrowIfNull(group);
        if (Set is not { } set || StationIndex < 0 || group.Stations[StationIndex] is not { Files: { } files } station)
            return (group, 0, "");

        var (sets, written) = TrainTextGenerating.Apply(files.TextSets, set, _generated, rbAll.Checked);
        if (ReferenceEquals(sets, files.TextSets))
            return (group, 0, set.Name);

        List<Domain.Stations.StationDocument> stations = [.. group.Stations];
        stations[StationIndex] = station with { Files = files with { TextSets = sets } };
        return (group with { Stations = stations }, written, set.Name);
    }

    private void Input_Changed(object? sender, EventArgs e)
    {
        if (_loading)
            return;
        if (sender == cboStation)
            ShowSets();
        else if (sender == cboSet)
            SelectKind();
        else
            ShowPreview();
    }

    // sady stanice: existujuce zo suborov textov, potom nove pre udaje katalogu
    private void ShowSets()
    {
        _loading = true;
        var station = StationIndex >= 0 ? _group.Stations[StationIndex] : null;
        var choices = station is null ? [] : TrainTextGenerating.Choices(station);
        cboSet.DataSource = choices.Select(choice => new Item<TextSetChoice>(choice, Label(choice))).ToList();
        // predvolene prva sada, pre ktoru sa text da generovat
        if (choices.FindIndex(choice => choice.Kind is not null) is >= 0 and var first)
            cboSet.SelectedIndex = first;
        _loading = false;
        SelectKind();
    }

    private static string Label(TextSetChoice choice)
    {
        var targets = string.Join(", ", choice.Targets.Select(target => target.Catalog + " / " + target.Column));
        return string.Format(CultureInfo.CurrentCulture, choice.IsNew ? Resources.Texts_NewSet : Resources.Texts_Set, choice.Name, targets);
    }

    // obsah udaja sady urci, co sa generuje; pri udaji bez podporovaneho obsahu ostane volba pouzivatela
    private void SelectKind()
    {
        if (Set?.Kind is { } kind)
        {
            _loading = true;
            cboKind.SelectedIndex = (int)kind;
            _loading = false;
        }

        ShowPreview();
    }

    private void ShowPreview()
    {
        var via = Kind is GeneratedTextKind.StationsTo or GeneratedTextKind.StationsFrom;
        lReport.Enabled = pReport.Enabled = via;

        if (Set is not { } set || StationIndex < 0)
        {
            _generated = [];
            dgvPreview.DataSource = null;
            lSummary.Text = Resources.Texts_NoSets;
            bOk.Enabled = false;
            return;
        }

        // porovnanie s textom servera len pri udaji, ktoreho obsah zodpoveda generovanemu textu
        var target = set.Kind == Kind ? set.Target : null;
        _generated = TrainTextGenerating.Generate(_group, StationIndex, Kind, target, rbLong.Checked, _defaultsFile, _stationName);
        var lines = TrainTextGenerating.Preview(_generated, _group.Stations[StationIndex].Files?.TextSets ?? [], set.Name, rbAll.Checked);
        dgvPreview.DataSource = lines
            .Select(line => new TextPreviewRow(line.Text.Number, line.Text.Validity, line.Text.Text, line.Current, ChangeText(line.Change)))
            .ToList();

        var changed = lines.Count(line => line.Changes);
        lSummary.Text = string.Format(CultureInfo.CurrentCulture, Resources.Texts_Summary, changed);
        bOk.Enabled = changed > 0;
    }

    private static string ChangeText(GeneratedTextChange change) => change switch
    {
        GeneratedTextChange.Add => Resources.Texts_ChangeAdd,
        GeneratedTextChange.Replace => Resources.Texts_ChangeReplace,
        GeneratedTextChange.RouteOrStationText => Resources.Texts_ChangeFileText,
        GeneratedTextChange.SameWithout => Resources.Texts_ChangeSame,
        GeneratedTextChange.Empty => Resources.Texts_ChangeEmpty,
        _ => Resources.Texts_ChangeKeep
    };

    private void bOk_Click(object? sender, EventArgs e) => DialogResult = DialogResult.OK;

    /// <summary>Polozka vyberu s textom.</summary>
    private sealed record Item<T>(T Value, string Text);
}
