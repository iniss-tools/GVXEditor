using GVXEditor.Domain.Stations;
using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Editing;

/// <summary>
/// Cislo vlaku napriec subormi grafikonu: premenovanie v GV.XML, suboroch stanic, vylukach a odklonoch a zmazanie zaznamov
/// cisla, ktore po zmazani vlaku uz ziadny vlak nepouziva.
/// </summary>
internal static class TrainNumbers
{
    /// <summary>
    /// Cisla vlaku v poradi trasy vsetkych variant (cislo od bodu aj hlasene cislo).
    /// </summary>
    public static List<string> Of(GvTrain train)
    {
        ArgumentNullException.ThrowIfNull(train);
        return [.. train.Variants.SelectMany(variant => variant.Stops).SelectMany(stop => new[] { stop.Number, stop.DisplayNumber })
            .Where(number => number.Length > 0).Distinct(StringComparer.Ordinal)];
    }

    /// <summary>
    /// Poradia ostatnych vlakov grafikonu, ktore cislo pouzivaju.
    /// </summary>
    public static List<int> OtherTrains(GvFile timetable, int index, string number)
    {
        ArgumentNullException.ThrowIfNull(timetable);
        var others = new List<int>();
        for (var i = 0; i < timetable.Trains.Count; i++)
        {
            if (i != index && Uses(timetable.Trains[i], number))
                others.Add(i);
        }

        return others;
    }

    private static bool Uses(GvTrain train, string number) =>
        train.Variants.Any(variant => variant.Stops.Any(stop => stop.Number == number || stop.DisplayNumber == number));

    private static GvTrain Rename(GvTrain train, string old, string number) => train with
    {
        Variants = [.. train.Variants.Select(variant => variant with
        {
            Stops = [.. variant.Stops.Select(stop => stop with
            {
                Number = stop.Number == old ? number : stop.Number,
                DisplayNumber = stop.DisplayNumber == old ? number : stop.DisplayNumber
            })]
        })]
    };

    /// <summary>
    /// Premenuje cislo vlaku. Ak ho maju aj ine vlaky a nepremenuvaju sa (<paramref name="allTrains" /> je
    /// <see langword="false" />), zaznamy stanic ostanu pre ne a vlak dostane ich kopie pod novym cislom - len v staniciach,
    /// kde vlak s tym cislom zastavuje.
    /// </summary>
    /// <param name="group">grafikon</param>
    /// <param name="index">poradie vlaku v GV.XML</param>
    /// <param name="old">povodne cislo</param>
    /// <param name="number">nove cislo</param>
    /// <param name="allTrains">premenovat aj ostatne vlaky grafikonu s tymto cislom</param>
    public static GroupDocument Rename(GroupDocument group, int index, string old, string number, bool allTrains)
    {
        ArgumentNullException.ThrowIfNull(group);
        var timetable = group.Timetable ?? throw new InvalidOperationException("Grafikon bez vlakov");
        var original = timetable.Trains[index];
        var others = OtherTrains(timetable, index, old);

        var trains = timetable.Trains.ToList();
        trains[index] = Rename(original, old, number);
        if (allTrains)
        {
            foreach (var other in others)
                trains[other] = Rename(trains[other], old, number);
        }

        var copy = !allTrains && others.Count > 0;
        var stations = copy
            ? TrainStations.Keys(original, group.Group).Where(key => key.Number == old).Select(key => key.StationIndex).ToHashSet()
            : Enumerable.Range(0, group.Stations.Count).ToHashSet();

        var result = group with { Timetable = timetable with { Trains = trains } };
        foreach (var station in stations)
        {
            if (result.Stations[station].Files is { } files && StationNumbers.Rename(files, old, number, copy) is var renamed &&
                !ReferenceEquals(renamed, files))
                result = result.WithStation(station, renamed);
        }

        var lockouts = result.Lockouts;
        if (lockouts is not null && (!copy || TrainStations.LockoutNumber(original, group.InterestStations) == old))
            lockouts = StationNumbers.Map(lockouts, record => record.Number == old, record => record with { Number = number }, copy);

        var stationNumbers = stations.Select(station => group.Stations[station].Station.Station).ToHashSet();
        var diversions = result.Diversions is { } list
            ? StationNumbers.Map(list, record => record.Number == old && stationNumbers.Contains(record.Station), record => record with { Number = number }, copy)
            : null;

        return result with { Lockouts = lockouts, Diversions = diversions };
    }

    /// <summary>
    /// Pocet zaznamov cisla v suboroch stanic, vylukach a odklonoch grafikonu.
    /// </summary>
    public static int RecordCount(GroupDocument group, string number)
    {
        ArgumentNullException.ThrowIfNull(group);
        return group.Stations.Sum(station => station.Files is { } files ? StationNumbers.Count(files, number) : 0) +
               (group.Lockouts?.Count(record => record.Number == number) ?? 0) + (group.Diversions?.Count(record => record.Number == number) ?? 0);
    }

    /// <summary>
    /// Po zmazani vlaku odstrani jeho zaznamy v staniciach, ktore ziadny iny vlak s rovnakym cislom v stanici nepouziva, a
    /// vyluky cisla, pod ktorym ich uz ziadny vlak nema.
    /// </summary>
    /// <param name="group">grafikon uz bez vlaku</param>
    /// <param name="deleted">zmazany vlak</param>
    /// <returns>grafikon bez zaznamov a pocet odstranenych zaznamov</returns>
    public static (GroupDocument Group, int Removed) RemoveUnused(GroupDocument group, GvTrain deleted)
    {
        ArgumentNullException.ThrowIfNull(group);
        ArgumentNullException.ThrowIfNull(deleted);

        var trains = group.Timetable?.Trains ?? [];
        var used = trains.SelectMany(train => TrainStations.Keys(train, group.Group)).ToHashSet();
        var usedLockouts = trains.Select(train => TrainStations.LockoutNumber(train, group.InterestStations)).ToHashSet(StringComparer.Ordinal);

        var result = group;
        var removed = 0;
        foreach (var key in TrainStations.Keys(deleted, group.Group).Where(key => !used.Contains(key)))
        {
            if (result.Stations[key.StationIndex].Files is { } files)
            {
                removed += StationNumbers.Count(files, key.Number);
                var cleaned = StationNumbers.Remove(files, key.Number);
                if (!ReferenceEquals(cleaned, files))
                    result = result.WithStation(key.StationIndex, cleaned);
            }

            if (result.Diversions is { } diversions)
            {
                bool Mine(TrainDiversion record) => record.Station == key.Station.Station && record.Number == key.Number;
                removed += diversions.Count(Mine);
                result = result with { Diversions = StationNumbers.Without(diversions, Mine) };
            }
        }

        var lockoutNumber = TrainStations.LockoutNumber(deleted, group.InterestStations);
        if (lockoutNumber.Length > 0 && !usedLockouts.Contains(lockoutNumber) && result.Lockouts is { } lockouts)
        {
            removed += lockouts.Count(record => record.Number == lockoutNumber);
            result = result with { Lockouts = StationNumbers.Without(lockouts, record => record.Number == lockoutNumber) };
        }

        return (result, removed);
    }
}
