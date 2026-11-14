using System.ComponentModel;
using System.Globalization;
using GVXEditor.Domain.Setup;
using GVXEditor.Domain.Setup.Drafts;
using GVXEditor.Properties;

namespace GVXEditor.Domain.Rules;

/// <summary>
/// Kontrola nastavenia podla toho, ako subory cita server: zaznam, ktory server nenajde alebo pre ktory sa nespusti, je chyba,
/// zaznam, ktory nepouzije, upozornenie. Riadky sa cisluju od 1 ako v tabulke okna.
/// </summary>
internal static class SetupRules
{
    private static readonly HashSet<string> Categories = new(["V", "P", "K"], StringComparer.Ordinal);

    private static string Format(string format, params object[] args) => string.Format(CultureInfo.CurrentCulture, format, args);

    /// <summary>
    /// Nazvy stanic a predvolene stanice grafikonu. Pri dvoch rovnakych cislach v predvolenych staniciach server nenastartuje,
    /// v nazvoch prveho grafikonu s nazvami zlyha pri hladani nazvu (dalsie grafikony doplnaju len chybajuce cisla).
    /// </summary>
    public static List<SetupProblem> Check(GroupSetupEdit edit)
    {
        ArgumentNullException.ThrowIfNull(edit);
        var problems = new List<SetupProblem>();

        var names = Resources.Page_Names;
        var seen = new HashSet<int>();
        for (var i = 0; i < edit.Names.Count; i++)
        {
            var row = Format(Resources.Loc_Row, names, i + 1);
            var station = edit.Names[i];
            if (station.Number is not { } number)
                problems.Add(new SetupProblem(true, Format(Resources.Setup_BadStation, row, station.Station)));
            else if (!seen.Add(number))
                problems.Add(new SetupProblem(true, Format(Resources.Setup_DuplicateName, row, number)));
            else if (station.Name.Trim().Length == 0)
                problems.Add(new SetupProblem(false, Format(Resources.Setup_NoName, row, number)));
        }

        var defaults = Resources.Page_Defaults;
        seen.Clear();
        for (var i = 0; i < edit.DefaultStations.Count; i++)
        {
            var row = Format(Resources.Loc_Row, defaults, i + 1);
            var station = edit.DefaultStations[i];
            if (station.Number is not { } number)
                problems.Add(new SetupProblem(true, Format(Resources.Setup_BadStation, row, station.Station)));
            else if (!seen.Add(number))
                problems.Add(new SetupProblem(true, Format(Resources.Setup_DuplicateStation, row, number)));
        }

        return problems;
    }

    /// <summary>
    /// Nastavenie stanice: kolaje, typy hlaseni, vzory stanic v hlaseni a texty tabul.
    /// </summary>
    /// <param name="edit">koncepty stanice</param>
    /// <param name="station">nazov stanice pre text zisteni</param>
    public static List<SetupProblem> Check(StationSetupEdit edit, string station)
    {
        ArgumentNullException.ThrowIfNull(edit);
        var problems = new List<SetupProblem>();
        CheckTracks(edit, Format(Resources.Loc_Station, Resources.Page_Tracks, station), problems);
        CheckReportKeys(edit, Format(Resources.Loc_Station, Resources.Page_ReportKeys, station), problems);
        CheckPatterns(edit.ShortPatterns, Format(Resources.Loc_Station, Resources.Page_ShortPatterns, station), problems);
        CheckPatterns(edit.LongPatterns, Format(Resources.Loc_Station, Resources.Page_LongPatterns, station), problems);
        CheckRouteTexts(edit, Format(Resources.Loc_Station, Resources.Page_RouteTexts, station), problems);
        CheckStationTexts(edit, Format(Resources.Loc_Station, Resources.Page_StationTexts, station), problems);
        return problems;
    }

    // server hlada kolaj podla kluca, plati prva
    private static void CheckTracks(StationSetupEdit edit, string where, List<SetupProblem> problems)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        for (var i = 0; i < edit.Tracks.Count; i++)
        {
            var track = edit.Tracks[i];
            if (track.Separator)
                continue;

            var row = Format(Resources.Loc_Row, where, i + 1);
            if (track.Key.Length == 0)
                problems.Add(new SetupProblem(true, Format(Resources.Setup_NoTrackKey, row)));
            else if (!seen.Add(track.Key))
                problems.Add(new SetupProblem(true, Format(Resources.Setup_DuplicateTrack, row, track.Key)));
        }
    }

    // server hlada typ hlasenia podla nazvu, plati prvy; z pismena berie prvy znak, neznama kategoria je chyba citania
    private static void CheckReportKeys(StationSetupEdit edit, string where, List<SetupProblem> problems)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        for (var i = 0; i < edit.ReportKeys.Count; i++)
        {
            var key = edit.ReportKeys[i];
            var row = Format(Resources.Loc_Row, where, i + 1);
            if (key.Name.Length == 0 || key.Key.Length == 0)
            {
                problems.Add(new SetupProblem(true, Format(Resources.Setup_ReportKeyIncomplete, row)));
            }
            else
            {
                if (key.Key.Length > 1)
                    problems.Add(new SetupProblem(false, Format(Resources.Setup_ReportKeyLetter, row, key.Key)));
                if (!seen.Add(key.Name))
                    problems.Add(new SetupProblem(true, Format(Resources.Setup_DuplicateReport, row, key.Name)));
            }

            if (SetupText.Items(key.Categories).Any(category => !Categories.Contains(category)))
                problems.Add(new SetupProblem(true, Format(Resources.Setup_Categories, row)));
        }
    }

    // server pouzije prvy vzor, ktoreho stanice sa zhoduju s trasou vlaku poctom aj poradim
    private static void CheckPatterns(BindingList<PatternDraft> patterns, string where, List<SetupProblem> problems)
    {
        var seen = new Dictionary<string, int>(StringComparer.Ordinal);
        for (var i = 0; i < patterns.Count; i++)
        {
            var at = Format(Resources.Loc_Pattern, where, i + 1);
            var stops = patterns[i].Stops;
            if (stops.Count == 0)
            {
                problems.Add(new SetupProblem(false, Format(Resources.Setup_NoStations, at)));
                continue;
            }

            var bad = stops.FirstOrDefault(stop => stop.Number is null);
            if (bad is not null)
            {
                problems.Add(new SetupProblem(true, Format(Resources.Setup_BadStation, at, bad.Station)));
                continue;
            }

            var key = SetupText.Stations(stops.Select(stop => stop.Number!.Value));
            if (seen.TryGetValue(key, out var first))
                problems.Add(new SetupProblem(false, Format(Resources.Setup_DuplicatePattern, at, first)));
            else
                seen[key] = i + 1;
        }
    }

    private static void CheckSet(TextSetDraft set, string at, List<SetupProblem> problems)
    {
        if (set.Name.Trim().Length == 0)
            problems.Add(new SetupProblem(false, Format(Resources.Setup_NoSetName, at)));
        if (set.Targets.Any(target => target.Catalog.Trim().Length == 0 || target.Column.Trim().Length == 0))
            problems.Add(new SetupProblem(false, Format(Resources.Setup_TargetIncomplete, at)));
    }

    private static void CheckRouteTexts(StationSetupEdit edit, string where, List<SetupProblem> problems)
    {
        for (var s = 0; s < edit.RouteSets.Count; s++)
        {
            var set = edit.RouteSets[s];
            var at = Format(Resources.Loc_Set, where, set.Name.Trim().Length > 0 ? set.Name : (s + 1).ToString(CultureInfo.CurrentCulture));
            CheckSet(set, at, problems);
            for (var i = 0; i < set.Routes.Count; i++)
            {
                var route = set.Routes[i];
                var text = Format(Resources.Loc_Text, at, i + 1);
                if (SetupText.Font(route.Font) is null)
                    problems.Add(new SetupProblem(true, Format(Resources.Setup_BadFont, text, route.Font)));
                if (route.Stations.FirstOrDefault(station => station.Number is null) is { } bad)
                    problems.Add(new SetupProblem(true, Format(Resources.Setup_BadStation, text, bad.Station)));
                else if (route.Stations.Count == 0)
                    problems.Add(new SetupProblem(false, Format(Resources.Setup_NoStations, text)));
            }
        }
    }

    // server pouzije prvy text so stanicou
    private static void CheckStationTexts(StationSetupEdit edit, string where, List<SetupProblem> problems)
    {
        for (var s = 0; s < edit.StationSets.Count; s++)
        {
            var set = edit.StationSets[s];
            var at = Format(Resources.Loc_Set, where, set.Name.Trim().Length > 0 ? set.Name : (s + 1).ToString(CultureInfo.CurrentCulture));
            CheckSet(set, at, problems);
            var seen = new HashSet<int>();
            for (var i = 0; i < set.Texts.Count; i++)
            {
                var text = set.Texts[i];
                var row = Format(Resources.Loc_Row, at, i + 1);
                if (SetupText.Font(text.Font) is null)
                    problems.Add(new SetupProblem(true, Format(Resources.Setup_BadFont, row, text.Font)));
                if (text.Number is not { } number)
                    problems.Add(new SetupProblem(true, Format(Resources.Setup_BadStation, row, text.Station)));
                else if (!seen.Add(number))
                    problems.Add(new SetupProblem(false, Format(Resources.Setup_DuplicateStationText, row, number)));
            }
        }
    }

    /// <summary>
    /// Hlasenia obsluhy: server hlada hlasenie podla cisla, plati prve.
    /// </summary>
    /// <param name="edit">koncepty hlaseni</param>
    /// <param name="functions">funkcie LogText.vb; <see langword="null" />, ak subor chyba</param>
    /// <param name="used">cisla, na ktore sa odkazuju zaznamy vlakov (dodatky, vyluky, odklony)</param>
    public static List<SetupProblem> Check(ConstReportsEdit edit, IReadOnlyDictionary<string, HashSet<string>>? functions, IReadOnlySet<int> used)
    {
        ArgumentNullException.ThrowIfNull(used);
        ArgumentNullException.ThrowIfNull(edit);
        var problems = new List<SetupProblem>();
        var seen = new HashSet<int>();
        for (var i = 0; i < edit.Reports.Count; i++)
        {
            var report = edit.Reports[i];
            var row = Format(Resources.Loc_Row, Resources.Page_ConstReports, i + 1);
            if (report.Number is not { } number || number <= 0)
                problems.Add(new SetupProblem(true, Format(Resources.Setup_BadReportKey, row, report.Key)));
            else if (!seen.Add(number))
                problems.Add(new SetupProblem(true, Format(Resources.Setup_DuplicateReportKey, row, number)));

            if (report.Function.Trim().Length == 0)
                problems.Add(new SetupProblem(true, Format(Resources.Setup_NoFunction, row)));
            else if (functions is not null && !functions.ContainsKey(report.Function))
                problems.Add(new SetupProblem(false, Format(Resources.Setup_UnknownFunction, row, report.Function)));

            if (report.StationNumbers is null)
                problems.Add(new SetupProblem(true, Format(Resources.Setup_BadStations, row)));
        }

        // zmazane alebo precislovane hlasenie, na ktore sa odkazuju zaznamy vlakov
        foreach (var key in edit.Original.Select(report => report.Key).Where(key => used.Contains(key) && !seen.Contains(key)).Distinct())
            problems.Add(new SetupProblem(false, Format(Resources.Setup_ReportStillUsed, key)));

        return problems;
    }
}
