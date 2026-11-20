using System.ComponentModel;

namespace GVXEditor.Mcp.Results;

/// <summary>Vysledok list_script_functions.</summary>
internal sealed record ScriptFunctions(
    [property: Description("Number of lines of LogText.vb.")] int TotalLines,
    [property: Description("Functions of class LogText in script order.")] IReadOnlyList<ScriptFunction> Functions);
