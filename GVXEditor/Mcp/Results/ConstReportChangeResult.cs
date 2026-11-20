using System.ComponentModel;

namespace GVXEditor.Mcp.Results;

/// <summary>Vysledok set_const_report a delete_const_report.</summary>
internal sealed record ConstReportChangeResult(
    [property: Description("Operator report number.")] int Number,
    [property: Description("Warnings of the check, e.g. a pattern missing in LogText.vb or a deleted report still referenced by trains.")] IReadOnlyList<ProblemInfo> Warnings,
    ChangeInfo Change);
