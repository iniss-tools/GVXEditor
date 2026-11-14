using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Stations.Texts;

/// <summary>
/// Stanice hlasenia vlaku zo smeru a do smeru tak, ako ich vyberie server (dekompilat IniSrv, <c>StaHlasVl.GenerujStanici</c>):
/// zaznam vlaku v StaHlasVl.xml, inak vzor StaHlasB.xml (kratke) alebo StaHlasC.xml (dlhe hlasenie) nad trasou bez bodov
/// s klucom <c>P</c>, inak do smeru predvolene stanice z Default.Stations.xml s cielovou a zo smeru len vychodzia stanica.
/// </summary>
internal static class ReportStationSelection
{
    /// <summary>
    /// Stanice hlasenia pre zastavenie vlaku v stanici.
    /// </summary>
    /// <param name="variant">varianta vlaku tak, ako ju vidi server</param>
    /// <param name="index">poradie zastavenia v stanici</param>
    /// <param name="number">cislo vlaku pri odchode zo stanice</param>
    /// <param name="source">zaznamy pri vlakoch, vzory a predvolene stanice</param>
    /// <param name="longReport">dlhe hlasenie, inak kratke</param>
    /// <returns>stanice zo smeru a do smeru bez tejto stanice; vlak, ktory tu zacina (konci), nema stanice zo smeru (do smeru)</returns>
    public static (IReadOnlyList<int> From, IReadOnlyList<int> To) Of(GvVariant variant, int index, string number, ReportStationSource source,
        bool longReport)
    {
        ArgumentNullException.ThrowIfNull(variant);
        ArgumentNullException.ThrowIfNull(source);
        var stops = variant.Stops;
        var station = stops[index].Station;
        var patterns = longReport ? source.LongPatterns : source.ShortPatterns;

        // zaznam s poznamkou plati len v niektore dni - prednost ma zaznam bez poznamky, zaznamy upravene na den sa vynechaju
        var record = source.Records.Where(item => item.Number == number && !item.EditedForDay).OrderBy(item => item.Validity.Length > 0)
            .FirstOrDefault();

        IReadOnlyList<int> from = [];
        if (index > 0)
        {
            // prazdny zoznam pri vlaku neznamena ziadnu stanicu - server hlada vzor
            var recorded = longReport ? record?.LongFrom : record?.ShortFrom;
            from = recorded is { Count: > 0 } ? recorded : Pattern(patterns, Route(stops, 0, index)) ?? [stops[0].Station];
        }

        IReadOnlyList<int> to = [];
        if (index < stops.Count - 1)
        {
            var route = Route(stops, index, stops.Count - 1);
            var recorded = longReport ? record?.LongTo : record?.ShortTo;
            to = recorded is { Count: > 0 } ? recorded : Pattern(patterns, route) ?? Defaults(route, source, longReport);
        }

        return ([.. from.Where(item => item != station)], [.. to.Where(item => item != station)]);
    }

    /// <summary>
    /// Stanice trasy od do (vratane) bez bodov, ktorymi vlak len prechadza - ako ich server porovnava so vzormi a trasami.
    /// </summary>
    public static List<int> Route(IReadOnlyList<GvStop> stops, int first, int last)
    {
        ArgumentNullException.ThrowIfNull(stops);
        return [.. stops.Skip(first).Take(last - first + 1).Where(stop => !stop.Flags.HasFlag(GvStopFlags.Passing)).Select(stop => stop.Station)];
    }

    // prvy vzor s rovnakymi stanicami v rovnakom poradi; vysledok su hlasene stanice vzoru (aj ziadna)
    private static List<int>? Pattern(IReadOnlyList<IReadOnlyList<ReportPatternStop>> patterns, List<int> route)
    {
        if (route.Count == 0)
            return null;
        var pattern = patterns.FirstOrDefault(item => item.Select(stop => stop.Station).SequenceEqual(route));
        return pattern is null ? null : [.. pattern.Where(stop => stop.Reported).Select(stop => stop.Station)];
    }

    // cielova stanica vzdy, ostatne za touto stanicou len zo suboru s danym hlasenim; bez suboru kratke len cielova, dlhe vsetky
    private static List<int> Defaults(List<int> route, ReportStationSource source, bool longReport)
    {
        if (!source.DefaultsFile)
            return longReport ? [.. route.Skip(1)] : route.Count > 1 ? [route[^1]] : [];

        var result = new List<int>();
        for (var i = 1; i < route.Count; i++)
        {
            if (i == route.Count - 1 || source.Defaults.Any(item => item.Station == route[i] && (longReport ? item.Long : item.Short)))
                result.Add(route[i]);
        }

        return result;
    }
}
