using System.ComponentModel;

namespace GVXEditor.Mcp.Results;

/// <summary>Grafikon instalacie.</summary>
internal sealed record GroupInfo(
    [property: Description("Group name (folder), the group parameter of other tools.")] string Name,
    [property: Description("Number of trains in GV.XML.")] int Trains,
    [property: Description("Why GV.XML could not be read; null = read.")] string? Error,
    [property: Description("Stations of the group in DirList.xml order.")] IReadOnlyList<StationInfo> Stations);
