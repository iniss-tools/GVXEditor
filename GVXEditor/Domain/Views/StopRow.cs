using System.Globalization;
using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Views;

/// <summary>
/// Riadok trasy varianty - bod trasy so vsetkymi udajmi zo suboru.
/// </summary>
/// <param name="Station">nazov stanice</param>
/// <param name="Code">cislo stanice SR70</param>
/// <param name="Arrival">prichod</param>
/// <param name="Departure">odchod</param>
/// <param name="Flags">priznaky klucmi ako v subore</param>
/// <param name="Number">cislo vlaku od bodu (<c>dcv</c>)</param>
/// <param name="DisplayNumber">hlasene cislo (<c>cv</c>)</param>
/// <param name="Type">druh vlaku od bodu</param>
/// <param name="Name">nazov vlaku od bodu</param>
/// <param name="Line">linka integrovaneho dopravneho systemu</param>
/// <param name="ReplacementLine">linka nahradnej dopravy</param>
internal sealed record StopRow(string Station, string Code, string Arrival, string Departure, string Flags, string Number, string DisplayNumber, string Type,
    string Name, string Line, string ReplacementLine)
{
    /// <summary>
    /// Riadky trasy varianty v poradi jazdy.
    /// </summary>
    /// <param name="variant">varianta</param>
    /// <param name="stationName">nazov stanice podla cisla</param>
    public static List<StopRow> Create(GvVariant variant, Func<int, string> stationName)
    {
        ArgumentNullException.ThrowIfNull(variant);
        ArgumentNullException.ThrowIfNull(stationName);

        return variant.Stops.Select(stop => new StopRow(stationName(stop.Station), stop.Station.ToString(CultureInfo.InvariantCulture),
            TimeText.Format(stop.Arrival, stop.ArrivalDay), TimeText.Format(stop.Departure, stop.DepartureDay), ValueTexts.StopFlags(stop.Flags), stop.Number,
            stop.DisplayNumber, stop.TrainType, stop.TrainName, stop.Line, stop.ReplacementLine)).ToList();
    }
}
