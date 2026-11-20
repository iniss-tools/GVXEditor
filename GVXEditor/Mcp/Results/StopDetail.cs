using System.ComponentModel;

namespace GVXEditor.Mcp.Results;

/// <summary>Bod trasy varianty.</summary>
internal sealed record StopDetail(
    [property: Description("Station number SR70.")] int Station,
    [property: Description("Station name.")] string Name,
    [property: Description("Arrival; +1 = next day.")] string Arrival,
    [property: Description("Departure; +1 = next day.")] string Departure,
    [property: Description("Point flags as keys of the file: P passing, Z station controlled by INISS (computed), PR transfer, DB operating point, X request stop.")] string Flags,
    [property: Description("Train number from this point; empty = as before.")] string Number,
    [property: Description("Announced number from this point.")] string ReportedNumber,
    [property: Description("Train type from this point.")] string Type,
    [property: Description("Train name from this point.")] string TrainName,
    [property: Description("IDS line.")] string IdsLine,
    [property: Description("Replacement bus line.")] string NadLine);
