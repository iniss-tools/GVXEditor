using System.ComponentModel;

namespace GVXEditor.Mcp.Results;

/// <summary>Stanica grafikonu.</summary>
internal sealed record StationInfo(
    [property: Description("Station number SR70.")] int Number,
    [property: Description("Station name.")] string Name,
    [property: Description("Station folder in the group.")] string Directory);
