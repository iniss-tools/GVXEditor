using System.ComponentModel;
using GVXEditor.Domain.Setup.Drafts;
using GVXEditor.Domain.Stations;

namespace GVXEditor.Domain.Setup;

/// <summary>
/// Nastavenie jednej stanice v okne Nastavenie grafikonu: koncepty zoznamov, ktore okno meni.
/// </summary>
internal sealed class StationSetupEdit
{
    private StationSetupEdit(int index, StationDocument station, StationSetup setup)
    {
        Index = index;
        Station = station;
        Setup = setup;
    }

    /// <summary>Poradie stanice v grafikone.</summary>
    public int Index { get; }

    /// <summary>Stanica pred upravou.</summary>
    public StationDocument Station { get; }

    /// <summary>Nastavenie pred upravou.</summary>
    public StationSetup Setup { get; }

    /// <summary>Kolaje.</summary>
    public BindingList<TrackDraft> Tracks { get; } = [];

    /// <summary>Typy hlaseni.</summary>
    public BindingList<ReportKeyDraft> ReportKeys { get; } = [];

    /// <summary>Vzory stanic pre kratke hlasenie.</summary>
    public BindingList<PatternDraft> ShortPatterns { get; } = [];

    /// <summary>Vzory stanic pre dlhe hlasenie.</summary>
    public BindingList<PatternDraft> LongPatterns { get; } = [];

    /// <summary>Sady textov podla trasy.</summary>
    public BindingList<RouteSetDraft> RouteSets { get; } = [];

    /// <summary>Sady textov podla stanice.</summary>
    public BindingList<StationSetDraft> StationSets { get; } = [];

    /// <summary>
    /// Koncepty nastavenia stanice; <see langword="null" />, ak sa nastavenie stanice nedalo nacitat.
    /// </summary>
    /// <param name="index">poradie stanice v grafikone</param>
    /// <param name="station">stanica</param>
    public static StationSetupEdit? Of(int index, StationDocument station)
    {
        ArgumentNullException.ThrowIfNull(station);
        if (station.Setup is not { } setup)
            return null;

        var edit = new StationSetupEdit(index, station, setup);
        Fill(edit.Tracks, setup.Tracks.Select(TrackDraft.Of));
        Fill(edit.ReportKeys, setup.ReportKeys.Select(ReportKeyDraft.Of));
        Fill(edit.ShortPatterns, setup.ShortPatterns.Select(PatternDraft.Of));
        Fill(edit.LongPatterns, setup.LongPatterns.Select(PatternDraft.Of));
        Fill(edit.RouteSets, setup.RouteTexts.Select(RouteSetDraft.Of));
        Fill(edit.StationSets, setup.StationTexts.Select(StationSetDraft.Of));
        return edit;
    }

    private static void Fill<T>(BindingList<T> list, IEnumerable<T> items)
    {
        foreach (var item in items)
            list.Add(item);
    }

    /// <summary>
    /// Nastavenie z konceptov. Nezmenene zoznamy ostanu tie iste objekty, bez zmeny vrati <see cref="Setup" />.
    /// </summary>
    public StationSetup Apply()
    {
        var setup = Setup;
        var tracks = SetupEquality.Keep(setup.Tracks, [.. Tracks.Select(track => track.To())], SetupEquality.Equal);
        var keys = SetupEquality.Keep(setup.ReportKeys, [.. ReportKeys.Select(key => key.To())], SetupEquality.Equal);
        var shortPatterns = SetupEquality.Keep(setup.ShortPatterns, [.. ShortPatterns.Select(pattern => pattern.To())], SetupEquality.Same);
        var longPatterns = SetupEquality.Keep(setup.LongPatterns, [.. LongPatterns.Select(pattern => pattern.To())], SetupEquality.Same);
        var routes = SetupEquality.Keep(setup.RouteTexts, [.. RouteSets.Select(set => set.To())], SetupEquality.Same);
        var texts = SetupEquality.Keep(setup.StationTexts, [.. StationSets.Select(set => set.To())], SetupEquality.Same);

        return ReferenceEquals(tracks, setup.Tracks) && ReferenceEquals(keys, setup.ReportKeys) && ReferenceEquals(shortPatterns, setup.ShortPatterns) &&
               ReferenceEquals(longPatterns, setup.LongPatterns) && ReferenceEquals(routes, setup.RouteTexts) && ReferenceEquals(texts, setup.StationTexts)
            ? setup
            : new StationSetup(tracks, keys, shortPatterns, longPatterns, routes, texts);
    }
}
