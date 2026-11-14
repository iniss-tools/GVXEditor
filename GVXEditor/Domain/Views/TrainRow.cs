using System.Globalization;
using GVXEditor.Properties;
using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Views;

/// <summary>
/// Riadok zoznamu vlakov grafikonu. Trasu, casy a dopravcu ukazuje prva varianta vlaku.
/// </summary>
/// <param name="Train">vlak</param>
/// <param name="Numbers">cisla vlaku pozdlz trasy vsetkych variant</param>
/// <param name="Type">druh vlaku z prveho bodu s cislom</param>
/// <param name="Name">nazov vlaku</param>
/// <param name="From">vychodzia stanica</param>
/// <param name="Departure">odchod z vychodzej stanice</param>
/// <param name="To">cielova stanica</param>
/// <param name="Arrival">prichod do cielovej stanice</param>
/// <param name="Validity">obdobie platnosti</param>
/// <param name="Variants">pocet variant</param>
/// <param name="Operator">dopravca</param>
internal sealed record TrainRow(GvTrain Train, string Numbers, string Type, string Name, string From, string Departure, string To, string Arrival,
    string Validity, int Variants, string Operator)
{
    /// <summary>Poradie vlaku v GV.XML - plati aj v zoradenom zozname.</summary>
    public int Index { get; init; }

    /// <summary>Odchod z vychodzej stanice v minutach od polnoci prveho dna (na triedenie).</summary>
    public int? DepartureMinutes { get; init; }

    /// <summary>Prichod do cielovej stanice v minutach od polnoci prveho dna (na triedenie).</summary>
    public int? ArrivalMinutes { get; init; }

    /// <summary>
    /// Riadok vlaku.
    /// </summary>
    /// <param name="train">vlak</param>
    /// <param name="stationName">nazov stanice podla cisla</param>
    /// <param name="index">poradie vlaku v GV.XML</param>
    public static TrainRow Create(GvTrain train, Func<int, string> stationName, int index = 0)
    {
        ArgumentNullException.ThrowIfNull(train);
        ArgumentNullException.ThrowIfNull(stationName);

        var numbers = train.Variants.SelectMany(variant => variant.Stops).Select(stop => stop.Number).Where(number => number.Length > 0)
            .Distinct(StringComparer.Ordinal);
        var stops = train.Variants.Count > 0 ? train.Variants[0].Stops : [];
        var first = stops.Count > 0 ? stops[0] : null;
        var last = stops.Count > 0 ? stops[^1] : null;
        var source = stops.FirstOrDefault(stop => stop.Number.Length > 0);

        return new TrainRow(train,
            string.Join(" / ", numbers),
            source?.TrainType ?? "",
            stops.Select(stop => stop.TrainName).FirstOrDefault(name => name.Length > 0) ?? "",
            first is null ? "" : stationName(first.Station),
            first is null ? "" : TimeText.Format(first.Departure, first.DepartureDay),
            last is null ? "" : stationName(last.Station),
            last is null ? "" : TimeText.Format(last.Arrival, last.ArrivalDay),
            Period(train.From, train.To),
            train.Variants.Count,
            train.Variants.Count > 0 ? train.Variants[0].Operator : "")
        {
            Index = index,
            DepartureMinutes = first?.Departure + first?.DepartureDay * MinutesPerDay,
            ArrivalMinutes = last?.Arrival + last?.ArrivalDay * MinutesPerDay
        };
    }

    private const int MinutesPerDay = 24 * 60;

    /// <summary>
    /// Obdobie platnosti; vlak bez konca ma len zaciatok.
    /// </summary>
    public static string Period(DateOnly from, DateOnly to)
    {
        var start = from == DateOnly.MinValue ? "" : from.ToString("d", CultureInfo.CurrentCulture);
        return to == DateOnly.MaxValue
            ? string.Format(CultureInfo.CurrentCulture, Resources.Validity_From, start)
            : string.Format(CultureInfo.CurrentCulture, Resources.Validity_Range, start, to.ToString("d", CultureInfo.CurrentCulture));
    }
}
