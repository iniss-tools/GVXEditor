using System.ComponentModel;

namespace GVXEditor.Mcp.Results;

/// <summary>Vysledok get_file.</summary>
internal sealed record FileContent(
    [property: Description("Path relative to the data folder.")] string Path,
    [property: Description("The file exists or saving would create it.")] bool Exists,
    [property: Description("The text contains changes not saved yet.")] bool Unsaved,
    [property: Description("Number of lines of the file.")] int TotalLines,
    [property: Description("First returned line from 0.")] int Offset,
    [property: Description("Returned lines.")] string Text);
