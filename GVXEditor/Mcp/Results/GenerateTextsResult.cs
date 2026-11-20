using System.ComponentModel;

namespace GVXEditor.Mcp.Results;

/// <summary>Vysledok generate_board_texts.</summary>
internal sealed record GenerateTextsResult(
    [property: Description("Timetable group.")] string Group,
    [property: Description("Station number SR70.")] int Station,
    [property: Description("Text set the texts were written to.")] string Set,
    [property: Description("What was generated: destination, origin, stationsTo or stationsFrom.")] string Kind,
    [property: Description("Number of texts written.")] int Written,
    ChangeInfo Change);
