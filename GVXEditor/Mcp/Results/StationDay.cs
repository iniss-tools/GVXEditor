using System.ComponentModel;

namespace GVXEditor.Mcp.Results;

/// <summary>Vysledok station_day.</summary>
internal sealed record StationDay(
    [property: Description("Timetable group.")] string Group,
    [property: Description("Station number SR70.")] int Station,
    [property: Description("Station name.")] string Name,
    [property: Description("Day.")] DateOnly Day,
    [property: Description("Stops of trains on that day by time.")] IReadOnlyList<StationDayItem> Trains);
