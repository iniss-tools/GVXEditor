using System.ComponentModel;

namespace GVXEditor.Mcp.Results;

/// <summary>Zistenie kontroly zmeny (upozornenie).</summary>
internal sealed record ProblemInfo(
    [property: Description("error or warning.")] string Severity,
    [property: Description("Description in the language of the program.")] string Message,
    [property: Description("Variant position from 1; null = the whole train.")] int? Variant,
    [property: Description("Route point from 1; null = the whole variant.")] int? Stop);
