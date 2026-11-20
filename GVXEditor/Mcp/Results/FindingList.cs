using System.ComponentModel;

namespace GVXEditor.Mcp.Results;

/// <summary>Vysledok check_installation a verify_with_server.</summary>
internal sealed record FindingList(
    [property: Description("Errors - the server would not start or not read a file.")] int Errors,
    [property: Description("Warnings - the server would use the data differently than expected.")] int Warnings,
    [property: Description("Information, e.g. records no train uses.")] int Infos,
    [property: Description("Findings, errors first.")] IReadOnlyList<FindingInfo> Findings);
