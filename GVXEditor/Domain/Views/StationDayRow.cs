using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Views;

/// <summary>
/// Riadok vlakov stanice v dany den - co server v ten den v stanici ma.
/// </summary>
/// <param name="Arrival">prichod v den stanice</param>
/// <param name="Departure">odchod; pri odchode po polnoci s poctom dni</param>
/// <param name="ArrivalNumber">cislo vlaku pri prichode</param>
/// <param name="DepartureNumber">cislo vlaku pri odchode, podla neho server hlada kolaj</param>
/// <param name="Type">druh vlaku</param>
/// <param name="Name">nazov vlaku</param>
/// <param name="From">vychodzia stanica vlaku</param>
/// <param name="To">cielova stanica vlaku</param>
/// <param name="Track">kolaj pri prichode a odchode</param>
/// <param name="Variant">platna varianta vlaku od 1</param>
internal sealed record StationDayRow(string Arrival, string Departure, string ArrivalNumber, string DepartureNumber, string Type, string Name, string From,
    string To, string Track, int Variant)
{
    /// <summary>
    /// Riadky zastaveni stanice v den.
    /// </summary>
    /// <param name="calls">zastavenia z <see cref="StationTimetable.On" /></param>
    /// <param name="stationName">nazov stanice podla cisla</param>
    public static List<StationDayRow> Create(IEnumerable<StationDayCall> calls, Func<int, string> stationName)
    {
        ArgumentNullException.ThrowIfNull(calls);
        ArgumentNullException.ThrowIfNull(stationName);

        return calls.Select(item =>
        {
            var call = item.Call;
            var stop = call.Stop;
            var stops = call.Variant.Stops;

            // cas v den stanice: prichod bez dni, odchod s dnami po prichode
            var dayAtStation = call.IsFirst ? stop.DepartureDay : stop.Arrival.HasValue ? stop.ArrivalDay : stop.DepartureDay;
            return new StationDayRow(
                call.IsFirst ? "" : TimeText.Format(stop.Arrival),
                call.IsLast ? "" : TimeText.Format(stop.Departure, stop.DepartureDay - dayAtStation),
                call.IsFirst ? "" : call.ArrivalNumber,
                call.IsLast ? "" : call.DepartureNumber,
                call.TrainType,
                call.TrainName,
                stationName(stops[0].Station),
                stationName(stops[^1].Station),
                TrackText(item.Position),
                call.Train.Variants.ToList().IndexOf(call.Variant) + 1);
        }).ToList();
    }

    private static string TrackText(TrainPosition? position)
    {
        if (position is null)
            return "";

        return position.ArrivalTrack == position.DepartureTrack || position.ArrivalTrack.Length == 0
            ? position.DepartureTrack
            : $"{position.ArrivalTrack} / {position.DepartureTrack}";
    }
}
