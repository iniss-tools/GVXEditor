using System.ComponentModel;

namespace GVXEditor.Mcp.Results;

/// <summary>Vysledok explain_validity.</summary>
internal sealed record ValidityDays(
    [property: Description("Date remark.")] string Text,
    [property: Description("First day of the period.")] DateOnly From,
    [property: Description("Last day of the period.")] DateOnly To,
    [property: Description("Number of days.")] int Count,
    [property: Description("Days as continuous ranges.")] IReadOnlyList<DayRange> Days,
    [property: Description("How the server treats the remark differently than expected; null = no problem.")] string? Problem);
