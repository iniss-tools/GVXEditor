using System.ComponentModel;

namespace GVXEditor.Mcp.Results;

/// <summary>Sada textov, do ktorej sa da generovat.</summary>
internal sealed record TextSetInfo(
    [property: Description("Set name.")] string Name,
    [property: Description("Board fields (template / field) where the texts appear.")] IReadOnlyList<string> Targets,
    [property: Description("What the field content generates; null = generating is not supported for it.")] string? Kind,
    [property: Description("A new set for a template field, not yet in any text file of the station.")] bool IsNew);
