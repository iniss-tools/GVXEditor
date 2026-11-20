using System.ComponentModel;

namespace GVXEditor.Mcp.Results;

/// <summary>Co zmena agenta urobila v programe.</summary>
internal sealed record ChangeInfo(
    [property: Description("Only a dry run; the installation stays as it is.")] bool DryRun,
    [property: Description("The change changes the installation (in a dry run: would change it).")] bool Changed,
    [property: Description("Undo step in GVXEditor, e.g. 'edit train 611 (agent)'; null = no step (dry run or nothing changed).")] string? Step);
