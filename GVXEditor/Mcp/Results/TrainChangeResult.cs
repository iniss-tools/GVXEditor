using System.ComponentModel;

namespace GVXEditor.Mcp.Results;

/// <summary>Vysledok set_train.</summary>
internal sealed record TrainChangeResult(
    [property: Description("Timetable group.")] string Group,
    [property: Description("Train index in GV.XML (a new train is the last one).")] int Train,
    [property: Description("All train numbers.")] string Numbers,
    [property: Description("Warnings of the train check; they do not prevent the change.")] IReadOnlyList<ProblemInfo> Warnings,
    ChangeInfo Change);
