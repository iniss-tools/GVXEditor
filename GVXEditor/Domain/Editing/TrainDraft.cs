using GVXEditor.Domain.Views;
using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Editing;

/// <summary>
/// Vlak v okne Upravit vlak: obdobie platnosti a varianty s trasou. Na vlak sa prevedie az po kontrole
/// (<see cref="Rules.TrainRules" />).
/// </summary>
internal sealed class TrainDraft
{
    /// <summary>Prvy den platnosti.</summary>
    public DateOnly From { get; set; }

    /// <summary>Posledny den platnosti; pri <see cref="NoEnd" /> sa nepouzije.</summary>
    public DateOnly To { get; set; }

    /// <summary>Vlak nema koniec platnosti (v subore bez atributu <c>d</c>).</summary>
    public bool NoEnd { get; set; }

    /// <summary>Varianty v poradi - v den plati prva, ktora ide.</summary>
    public List<VariantDraft> Variants { get; init; } = [];

    /// <summary>Posledny den platnosti, ako ho pozna vlak.</summary>
    public DateOnly End => NoEnd ? DateOnly.MaxValue : To;

    /// <summary>
    /// Koncept vlaku.
    /// </summary>
    public static TrainDraft Of(GvTrain train)
    {
        ArgumentNullException.ThrowIfNull(train);
        return new TrainDraft
        {
            From = train.From,
            To = train.To == DateOnly.MaxValue ? train.From.AddDays(364) : train.To,
            NoEnd = train.To == DateOnly.MaxValue,
            Variants = [.. train.Variants.Select(VariantDraft.Of)]
        };
    }

    /// <summary>
    /// Novy vlak s jednou variantou o dvoch bodoch v obdobi <paramref name="from" /> az <paramref name="to" />.
    /// </summary>
    public static TrainDraft New(DateOnly from, DateOnly to) => new()
    {
        From = from,
        To = to,
        Variants = [new VariantDraft { Stops = [new StopDraft(), new StopDraft()] }]
    };

    /// <summary>
    /// Vlak z konceptu. Kluc <c>Z</c> dostane kazdy bod v stanici grafikonu a dni cez polnoc sa dopocitaju z casov: cas mensi ako
    /// predchadzajuci je dalsi den. Predpoklada platny koncept (casy a cisla stanic).
    /// </summary>
    /// <param name="interestStations">stanice grafikonu z DirList.xml</param>
    /// <exception cref="FormatException">cas alebo cislo stanice sa neda precitat</exception>
    public GvTrain ToTrain(IReadOnlySet<int> interestStations)
    {
        ArgumentNullException.ThrowIfNull(interestStations);
        return new GvTrain(From, End, [.. Variants.Select(variant => ToVariant(variant, interestStations))]);
    }

    private static GvVariant ToVariant(VariantDraft variant, IReadOnlySet<int> interestStations)
    {
        var day = variant.FirstDay;
        int? previous = null;
        var stops = new List<GvStop>();

        foreach (var stop in variant.Stops)
        {
            var station = stop.StationNumber ?? throw new FormatException($"Station {stop.Station}");
            var arrival = Time(stop.Arrival);
            var departure = Time(stop.Departure);

            var arrivalDay = 0;
            if (arrival is { } a)
            {
                if (previous is { } p && a < p)
                    day++;
                arrivalDay = day;
                previous = a;
            }

            var departureDay = 0;
            if (departure is { } d)
            {
                if (previous is { } p && d < p)
                    day++;
                departureDay = day;
                previous = d;
            }

            var flags = (stop.Passing ? GvStopFlags.Passing : 0) | (interestStations.Contains(station) ? GvStopFlags.Interest : 0) |
                        (stop.Transfer ? GvStopFlags.Pr : 0) | (stop.OperatingPoint ? GvStopFlags.OperatingPoint : 0) |
                        (stop.OnRequest ? GvStopFlags.OnRequest : 0);

            stops.Add(new GvStop
            {
                Station = station,
                Arrival = arrival,
                ArrivalDay = arrivalDay,
                Departure = departure,
                DepartureDay = departureDay,
                Flags = flags,
                Number = stop.Number.Trim(),
                DisplayNumber = stop.DisplayNumber.Trim(),
                TrainType = stop.TrainType.Trim(),
                TrainName = stop.TrainName.Trim(),
                Line = stop.Line.Trim(),
                ReplacementLine = stop.ReplacementLine.Trim()
            });
        }

        return new GvVariant
        {
            Validity = variant.Validity.Trim(),
            Operator = variant.Operator.Trim(),
            Flags = variant.Flags,
            Note = variant.Note,
            Stops = stops
        };
    }

    private static int? Time(string text) => TimeText.TryParse(text, out var minutes) ? minutes : throw new FormatException($"Time {text}");
}
