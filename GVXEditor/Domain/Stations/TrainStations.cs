using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Stations;

/// <summary>
/// Cisla vlaku v staniciach grafikonu tak, ako ich hlada server v suboroch stanic, vo vylukach a odklonoch.
/// </summary>
internal static class TrainStations
{
    private static readonly Dictionary<int, List<MosTrain>> NoMos = [];

    /// <summary>
    /// Vlak tak, ako ho vidi server: kluc <c>Z</c> len v staniciach grafikonu, cislo z <c>cv</c>, ak <c>dcv</c> chyba.
    /// </summary>
    public static GvTrain ServerView(GvTrain train, IReadOnlySet<int> interestStations) =>
        new GvFile([], [train]).ForServer(interestStations, NoMos).Trains[0];

    /// <summary>
    /// Stanice grafikonu, v ktorych vlak zastavuje s klucom <c>Z</c> a casom, s cislom pri odchode - v poradi trasy, kazda
    /// dvojica stanica a cislo raz. Zaznam s inym cislom server pre tento vlak nepouzije.
    /// </summary>
    /// <param name="train">vlak tak, ako je v subore</param>
    /// <param name="group">grafikon z DirList.xml</param>
    public static List<TrainStationKey> Keys(GvTrain train, DirListGroup group)
    {
        ArgumentNullException.ThrowIfNull(train);
        ArgumentNullException.ThrowIfNull(group);

        var server = ServerView(train, group.Items.Select(item => item.Station).ToHashSet());
        var keys = new List<TrainStationKey>();
        foreach (var variant in server.Variants)
        {
            for (var i = 0; i < variant.Stops.Count; i++)
            {
                var stop = variant.Stops[i];
                // ako obdobie cisla v stanici: kluc Z (aj prejazd) a cas
                if (!stop.Flags.HasFlag(GvStopFlags.Interest) || !stop.HasTime || variant.NumberSource(i, false)?.Number is not { Length: > 0 } number)
                    continue;

                var index = IndexOf(group, stop.Station);
                if (index >= 0 && !keys.Any(key => key.StationIndex == index && key.Number == number))
                    keys.Add(new TrainStationKey(index, group.Items[index], number));
            }
        }

        return keys;
    }

    /// <summary>
    /// Cislo, pod ktorym ma vlak vyluky: v prvej variante so zastavenim v stanici grafikonu cislo pri odchode z prveho
    /// takeho zastavenia; prazdne, ak vlak stanicou grafikonu nejde.
    /// </summary>
    /// <param name="train">vlak tak, ako je v subore</param>
    /// <param name="interestStations">stanice grafikonu</param>
    public static string LockoutNumber(GvTrain train, IReadOnlySet<int> interestStations)
    {
        ArgumentNullException.ThrowIfNull(train);
        foreach (var variant in ServerView(train, interestStations).Variants)
        {
            for (var i = 0; i < variant.Stops.Count; i++)
            {
                if (variant.Stops[i].IsInterest)
                    return variant.NumberSource(i, false)?.Number ?? "";
            }
        }

        return "";
    }

    private static int IndexOf(DirListGroup group, int station)
    {
        for (var i = 0; i < group.Items.Count; i++)
        {
            if (group.Items[i].Station == station)
                return i;
        }

        return -1;
    }
}
