using System.ComponentModel;

namespace GVXEditor.Mcp.Inputs;

/// <summary>Varianta vlaku (set_train).</summary>
internal sealed record VariantInput(
    [property: Description("Route points in running order, at least two.")] IReadOnlyList<StopInput> Route,
    [property: Description("Date remark in Czech, e.g. 'jede v 6,7'; empty = every day of the period.")] string Validity = "",
    [property: Description("Operator.")] string Operator = "",
    [property: Description("Variant flags as keys of the file separated by spaces: D, X, N, NP, O, M, R, L.")] string Flags = "",
    [property: Description("Note.")] string Note = "");
