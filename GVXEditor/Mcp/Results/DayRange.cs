using System.ComponentModel;

namespace GVXEditor.Mcp.Results;

/// <summary>Suvisly usek dni.</summary>
internal sealed record DayRange(
    [property: Description("First day.")] DateOnly From,
    [property: Description("Last day.")] DateOnly To);
