using System.ComponentModel;

namespace GVXEditor.Mcp.Results;

/// <summary>Vysledok rename_train_number.</summary>
internal sealed record RenameResult(
    [property: Description("Timetable group.")] string Group,
    [property: Description("Old number.")] string From,
    [property: Description("New number.")] string To,
    [property: Description("Indexes of trains renamed.")] IReadOnlyList<int> Trains,
    [property: Description("Records of the old number in station files, lockouts and diversions of the group.")] int Records,
    ChangeInfo Change);
