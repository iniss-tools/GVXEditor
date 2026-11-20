using System.ComponentModel;

namespace GVXEditor.Mcp.Results;

/// <summary>Vysledok preview_board_texts.</summary>
internal sealed record TextPreview(
    [property: Description("Timetable group.")] string Group,
    [property: Description("Station number SR70.")] int Station,
    [property: Description("Text sets the texts can be generated into.")] IReadOnlyList<TextSetInfo> Sets,
    [property: Description("Set of the preview; null = the station has no set or board field to generate into.")] string? Set,
    [property: Description("What is generated: destination, origin, stationsTo or stationsFrom.")] string Kind,
    [property: Description("Number of texts generating would change.")] int Changes,
    [property: Description("Trains of the station.")] IReadOnlyList<TextPreviewItem> Trains);
