using System.ComponentModel;

namespace GVXEditor.Mcp.Results;

/// <summary>Vlak v zozname (udaje z prvej varianty).</summary>
internal sealed record TrainListItem(
    [property: Description("Train index in GV.XML from 0, the train parameter of other tools.")] int Index,
    [property: Description("All train numbers along the routes of all variants.")] string Numbers,
    [property: Description("Train type key, e.g. Os, R, Ex.")] string Type,
    [property: Description("Train name.")] string Name,
    [property: Description("Origin station.")] string From,
    [property: Description("Departure from the origin.")] string Departure,
    [property: Description("Destination station.")] string To,
    [property: Description("Arrival at the destination; +1 = next day.")] string Arrival,
    [property: Description("Validity period of the train.")] string Period,
    [property: Description("Number of variants.")] int Variants,
    [property: Description("Operator.")] string Operator);
