using System.ComponentModel;

namespace GVXEditor.Mcp.Results;

/// <summary>Text tabule pri vlaku.</summary>
internal sealed record TrainTextInfo(
    [property: Description("Text set.")] string Set,
    [property: Description("Date remark.")] string Validity,
    [property: Description("Text, parts separated by #.")] string Text,
    [property: Description("Font; -1 = font of the board template.")] int Font);
