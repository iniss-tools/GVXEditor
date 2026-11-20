using System.ComponentModel;

namespace GVXEditor.Mcp.Results;

/// <summary>Funkcia triedy LogText.</summary>
internal sealed record ScriptFunction(
    [property: Description("Name; reports, supplements and operator report patterns refer to it.")] string Name,
    [property: Description("Sub or Function.")] string Kind,
    [property: Description("Attributes, e.g. Dodatek, VzorVyluky.")] IReadOnlyList<string> Attributes,
    [property: Description("Description from the first attribute with text.")] string? Description,
    [property: Description("First line including attributes, from 1.")] int StartLine,
    [property: Description("Last line, from 1.")] int EndLine);
