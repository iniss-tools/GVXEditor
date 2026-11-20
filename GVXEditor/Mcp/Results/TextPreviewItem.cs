using System.ComponentModel;

namespace GVXEditor.Mcp.Results;

/// <summary>Vygenerovany text vlaku a co by sa s nim stalo.</summary>
internal sealed record TextPreviewItem(
    [property: Description("Train number when leaving the station.")] string Number,
    [property: Description("Date remark of the text.")] string Validity,
    [property: Description("Generated text.")] string Text,
    [property: Description("Text of the train in the set, otherwise the text the server would show without it.")] string Current,
    [property: Description("add, replace, keep, routeOrStationText (a route or station text stays), sameWithout (the server shows it anyway) or empty.")] string Change);
