using System.Globalization;
using GVXEditor.Domain.Editing;
using GVXEditor.Domain.Views;
using GVXEditor.Properties;
using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Rules;

/// <summary>
/// Kontrola vlaku pred ulozenim: chyby, pre ktore sa vlak neda zapisat, a upozornenia na to, co by server s vlakom urobil inak,
/// nez obsluha cakala.
/// </summary>
internal static class TrainRules
{
    /// <summary>
    /// Zistenia konceptu vlaku. Pri chybe casu alebo stanice sa dalsie kontroly nerobia.
    /// </summary>
    /// <param name="draft">koncept</param>
    /// <param name="interestStations">stanice grafikonu z DirList.xml</param>
    /// <param name="stationName">nazov stanice podla cisla</param>
    public static List<TrainProblem> Check(TrainDraft draft, IReadOnlySet<int> interestStations, Func<int, string> stationName)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(stationName);

        var problems = new List<TrainProblem>();
        if (!draft.NoEnd && draft.To < draft.From)
            problems.Add(new TrainProblem(true, -1, -1, Resources.Rule_PeriodReversed));
        if (draft.Variants.Count == 0)
            problems.Add(new TrainProblem(true, -1, -1, Resources.Rule_NoVariant));

        for (var v = 0; v < draft.Variants.Count; v++)
        {
            var stops = draft.Variants[v].Stops;
            if (stops.Count < 2)
                problems.Add(new TrainProblem(true, v, -1, Text(Resources.Rule_FewStops, v + 1)));
            else if (stops[0].Number.Trim().Length == 0 && stops[0].DisplayNumber.Trim().Length == 0)
                problems.Add(new TrainProblem(true, v, 0, Text(Resources.Rule_NoNumber, v + 1)));

            for (var s = 0; s < stops.Count; s++)
            {
                var stop = stops[s];
                if (stop.StationNumber is null)
                    problems.Add(new TrainProblem(true, v, s, Text(Resources.Rule_BadStation, v + 1, s + 1, stop.Station)));
                foreach (var time in new[] { stop.Arrival, stop.Departure })
                {
                    if (!TimeText.TryParse(time, out _))
                        problems.Add(new TrainProblem(true, v, s, Text(Resources.Rule_BadTime, v + 1, s + 1, time)));
                }
            }
        }

        if (problems.Any(problem => problem.IsError))
            return problems;

        var train = draft.ToTrain(interestStations);
        var controlled = false;
        for (var v = 0; v < train.Variants.Count; v++)
        {
            var variant = train.Variants[v];
            switch (GvValidity.Check(train.From, train.To, variant.Validity))
            {
                case GvValidityProblem.PeriodTooLong:
                    problems.Add(new TrainProblem(false, v, -1, Text(Resources.Rule_ValidityTooLong, v + 1)));
                    break;
                case GvValidityProblem.Invalid:
                    problems.Add(new TrainProblem(false, v, -1, Text(Resources.Rule_ValidityInvalid, v + 1, variant.Validity)));
                    break;
                default:
                    if (!train.ActiveDaysOf(variant).HasAnySet())
                        problems.Add(new TrainProblem(false, v, -1, Text(Resources.Rule_VariantNeverActive, v + 1)));
                    break;
            }

            for (var s = 0; s < variant.Stops.Count; s++)
            {
                var stop = variant.Stops[s];
                if (!stop.IsInterest)
                    continue;
                if (stop.HasTime)
                    controlled = true;
                else
                    problems.Add(new TrainProblem(false, v, s, Text(Resources.Rule_InterestWithoutTime, v + 1, stationName(stop.Station))));
            }
        }

        if (!controlled)
            problems.Add(new TrainProblem(false, -1, -1, Resources.Rule_NoInterestStop));
        return problems;
    }

    private static string Text(string format, params object[] args) => string.Format(CultureInfo.CurrentCulture, format, args);
}
