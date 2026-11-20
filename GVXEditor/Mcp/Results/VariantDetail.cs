using System.ComponentModel;

namespace GVXEditor.Mcp.Results;

/// <summary>Varianta vlaku.</summary>
internal sealed record VariantDetail(
    [property: Description("Variant position from 1.")] int Position,
    [property: Description("Date remark; empty = every day of the period.")] string Validity,
    [property: Description("Days on which exactly this variant applies (earlier variants take precedence).")] int Days,
    [property: Description("Operator.")] string Operator,
    [property: Description("Variant flags as keys of the file separated by spaces: D, X, N, NP, O, M, R, L.")] string Flags,
    [property: Description("Note.")] string Note,
    [property: Description("Route points in running order.")] IReadOnlyList<StopDetail> Route);
