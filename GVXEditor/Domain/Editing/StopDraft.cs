using System.Globalization;
using GVXEditor.Domain.Views;
using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Editing;

/// <summary>
/// Bod trasy v okne Upravit vlak - hodnoty ako text, ako ich obsluha pise do tabulky. Kluc <c>Z</c> a dni cez polnoc sa
/// dopocitaju pri prevode na vlak (<see cref="TrainDraft.ToTrain" />), ako v klientovi INISS2.
/// </summary>
internal sealed class StopDraft
{
    /// <summary>Cislo stanice SR70.</summary>
    public string Station { get; set; } = "";

    /// <summary>Prichod <c>HH:mm</c>; prazdny = bez prichodu.</summary>
    public string Arrival { get; set; } = "";

    /// <summary>Odchod <c>HH:mm</c>; prazdny = bez odchodu.</summary>
    public string Departure { get; set; } = "";

    /// <summary>Prejazd bez zastavenia (kluc P).</summary>
    public bool Passing { get; set; }

    /// <summary>Nutny prestup (kluc PR).</summary>
    public bool Transfer { get; set; }

    /// <summary>Dopravny bod (kluc DB).</summary>
    public bool OperatingPoint { get; set; }

    /// <summary>Zastavka na znamenie (kluc X).</summary>
    public bool OnRequest { get; set; }

    /// <summary>Cislo vlaku od bodu (<c>dcv</c>).</summary>
    public string Number { get; set; } = "";

    /// <summary>Hlasene cislo (<c>cv</c>).</summary>
    public string DisplayNumber { get; set; } = "";

    /// <summary>Druh vlaku od bodu.</summary>
    public string TrainType { get; set; } = "";

    /// <summary>Nazov vlaku od bodu.</summary>
    public string TrainName { get; set; } = "";

    /// <summary>Linka integrovaneho dopravneho systemu.</summary>
    public string Line { get; set; } = "";

    /// <summary>Linka nahradnej dopravy.</summary>
    public string ReplacementLine { get; set; } = "";

    /// <summary>
    /// Koncept bodu trasy.
    /// </summary>
    public static StopDraft Of(GvStop stop)
    {
        ArgumentNullException.ThrowIfNull(stop);
        return new StopDraft
        {
            Station = stop.Station.ToString(CultureInfo.InvariantCulture),
            Arrival = TimeText.Format(stop.Arrival),
            Departure = TimeText.Format(stop.Departure),
            Passing = stop.Flags.HasFlag(GvStopFlags.Passing),
            Transfer = stop.Flags.HasFlag(GvStopFlags.Pr),
            OperatingPoint = stop.Flags.HasFlag(GvStopFlags.OperatingPoint),
            OnRequest = stop.Flags.HasFlag(GvStopFlags.OnRequest),
            Number = stop.Number,
            DisplayNumber = stop.DisplayNumber,
            TrainType = stop.TrainType,
            TrainName = stop.TrainName,
            Line = stop.Line,
            ReplacementLine = stop.ReplacementLine
        };
    }

    /// <summary>Kopia bodu.</summary>
    public StopDraft Clone() => (StopDraft)MemberwiseClone();

    /// <summary>
    /// Cislo stanice; <see langword="null" />, ak nie je cislo vacsie ako 0.
    /// </summary>
    public int? StationNumber => int.TryParse(Station.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out var number) && number > 0 ? number : null;
}
