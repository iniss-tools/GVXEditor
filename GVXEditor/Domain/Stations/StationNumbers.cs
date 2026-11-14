using ToolsCore.Iniss2.Grafikon;
using ToolsCore.Iniss2.Tables;

namespace GVXEditor.Domain.Stations;

/// <summary>
/// Cislo vlaku v zaznamoch suborov stanic, vyluk a odklonov: premenovanie, kopia pod novym cislom a zmazanie. Zoznam bez
/// zaznamu s cislom ostane ten isty objekt.
/// </summary>
internal static class StationNumbers
{
    /// <summary>
    /// Zaznamy s cislom <paramref name="old" /> dostanu cislo <paramref name="number" />; pri <paramref name="copy" /> ostanu
    /// a kopia s novym cislom ide hned za ne.
    /// </summary>
    public static StationFiles Rename(StationFiles files, string old, string number, bool copy)
    {
        ArgumentNullException.ThrowIfNull(files);
        var result = files with
        {
            Positions = Map(files.Positions, record => record.Train == old, record => record with { Train = number }, copy),
            Additions = Map(files.Additions, record => record.Number == old, record => record with { Number = number }, copy),
            Compositions = Map(files.Compositions, record => record.Number == old, record => record with { Number = number }, copy),
            Languages = Map(files.Languages, record => record.Number == old, record => record with { Number = number }, copy),
            ReportStations = Map(files.ReportStations, record => record.Number == old, record => record with { Number = number }, copy),
            TextSets = MapSets(files.TextSets, set => Map(set.Trains, text => text.Number == old, text => text with { Number = number }, copy)),
            Mos = Map(files.Mos, record => record.Number == old, record => record with { Number = number }, copy)
        };
        return Same(result, files) ? files : result;
    }

    /// <summary>
    /// Subory stanice bez zaznamov s cislom <paramref name="number" />.
    /// </summary>
    public static StationFiles Remove(StationFiles files, string number)
    {
        ArgumentNullException.ThrowIfNull(files);
        var result = files with
        {
            Positions = Without(files.Positions, record => record.Train == number),
            Additions = Without(files.Additions, record => record.Number == number),
            Compositions = Without(files.Compositions, record => record.Number == number),
            Languages = Without(files.Languages, record => record.Number == number),
            ReportStations = Without(files.ReportStations, record => record.Number == number),
            TextSets = MapSets(files.TextSets, set => Without(set.Trains, text => text.Number == number)),
            Mos = Without(files.Mos, record => record.Number == number)
        };
        return Same(result, files) ? files : result;
    }

    /// <summary>
    /// Pocet zaznamov s cislom v suboroch stanice.
    /// </summary>
    public static int Count(StationFiles files, string number)
    {
        ArgumentNullException.ThrowIfNull(files);
        return files.Positions.Count(record => record.Train == number) + files.Additions.Count(record => record.Number == number) +
               files.Compositions.Count(record => record.Number == number) + files.Languages.Count(record => record.Number == number) +
               files.ReportStations.Count(record => record.Number == number) +
               files.TextSets.Sum(set => set.Trains.Count(text => text.Number == number)) + files.Mos.Count(record => record.Number == number);
    }

    /// <summary>
    /// Zoznam, v ktorom zaznamy splnajuce <paramref name="match" /> dostanu nove cislo (alebo kopiu za seba).
    /// </summary>
    public static IReadOnlyList<T> Map<T>(IReadOnlyList<T> records, Func<T, bool> match, Func<T, T> renamed, bool copy)
    {
        ArgumentNullException.ThrowIfNull(records);
        if (!records.Any(match))
            return records;

        var result = new List<T>(records.Count + 4);
        foreach (var record in records)
        {
            if (!match(record))
            {
                result.Add(record);
                continue;
            }

            if (copy)
                result.Add(record);
            result.Add(renamed(record));
        }

        return result;
    }

    /// <summary>
    /// Zoznam bez zaznamov splnajucich <paramref name="match" />.
    /// </summary>
    public static IReadOnlyList<T> Without<T>(IReadOnlyList<T> records, Func<T, bool> match)
    {
        ArgumentNullException.ThrowIfNull(records);
        return records.Any(match) ? [.. records.Where(record => !match(record))] : records;
    }

    private static IReadOnlyList<TrainTextSet> MapSets(IReadOnlyList<TrainTextSet> sets, Func<TrainTextSet, IReadOnlyList<TrainText>> map)
    {
        var changed = false;
        var result = sets.Select(set =>
        {
            var trains = map(set);
            if (ReferenceEquals(trains, set.Trains))
                return set;
            changed = true;
            return set with { Trains = trains };
        }).ToList();
        return changed ? result : sets;
    }

    private static bool Same(StationFiles a, StationFiles b) =>
        ReferenceEquals(a.Positions, b.Positions) && ReferenceEquals(a.Additions, b.Additions) && ReferenceEquals(a.Compositions, b.Compositions) &&
        ReferenceEquals(a.Languages, b.Languages) && ReferenceEquals(a.ReportStations, b.ReportStations) &&
        ReferenceEquals(a.TextSets, b.TextSets) && ReferenceEquals(a.Mos, b.Mos);
}
