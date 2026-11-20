using System.ComponentModel;

namespace GVXEditor.Mcp.Results;

/// <summary>Zastavenie vlaku v stanici v den.</summary>
internal sealed record StationDayItem(
    [property: Description("Arrival on the station day.")] string Arrival,
    [property: Description("Departure; after midnight with the day count.")] string Departure,
    [property: Description("Train number on arrival.")] string ArrivalNumber,
    [property: Description("Train number on departure; station records are looked up by it.")] string DepartureNumber,
    [property: Description("Train type.")] string Type,
    [property: Description("Train name.")] string Name,
    [property: Description("Origin station.")] string From,
    [property: Description("Destination station.")] string To,
    [property: Description("Track the server uses that day (arrival and departure).")] string Track,
    [property: Description("Variant that applies that day, from 1.")] int Variant);
