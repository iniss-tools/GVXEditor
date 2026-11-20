using System.ComponentModel;

namespace GVXEditor.Mcp.Results;

/// <summary>Vysledok copy_train, delete_train a shift_validity.</summary>
internal sealed record TrainsChangeResult(
    [property: Description("Timetable group.")] string Group,
    [property: Description("Train indexes in GV.XML after the change: copies, shifted trains; empty after deleting.")] IReadOnlyList<int> Trains,
    [property: Description("Station, lockout and diversion records removed with deleted trains.")] int RemovedRecords,
    ChangeInfo Change);
