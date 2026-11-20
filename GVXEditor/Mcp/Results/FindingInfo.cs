using System.ComponentModel;

namespace GVXEditor.Mcp.Results;

/// <summary>Zistenie kontroly.</summary>
internal sealed record FindingInfo(
    [property: Description("error, warning or info.")] string Severity,
    [property: Description("Where: group, station, train or file.")] string Place,
    [property: Description("Description in the language of the program.")] string Message,
    [property: Description("Timetable group; null = the whole installation.")] string? Group,
    [property: Description("Train index in GV.XML; null = not a train.")] int? Train);
