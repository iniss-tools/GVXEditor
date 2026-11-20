using System.ComponentModel;

namespace GVXEditor.Mcp.Inputs;

/// <summary>Bod trasy varianty (set_train).</summary>
internal sealed record StopInput(
    [property: Description("Station number SR70.")] int Station,
    [property: Description("Arrival as 8:15, 08:15 or 815; empty = no arrival. A day suffix like +1 is ignored, days after midnight are computed from the times.")] string Arrival = "",
    [property: Description("Departure; empty = no departure.")] string Departure = "",
    [property: Description("Point flags separated by spaces: P passing, PR transfer, DB operating point, X request stop. Z (station controlled by INISS) is computed.")] string Flags = "",
    [property: Description("Train number from this point; empty = as before. The first point needs one.")] string Number = "",
    [property: Description("Announced number from this point.")] string ReportedNumber = "",
    [property: Description("Train type key from this point, e.g. Os, R (TrTypes.xml).")] string Type = "",
    [property: Description("Train name from this point.")] string TrainName = "",
    [property: Description("IDS line.")] string IdsLine = "",
    [property: Description("Replacement bus line.")] string NadLine = "");
