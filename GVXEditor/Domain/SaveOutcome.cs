namespace GVXEditor.Domain;

/// <summary>
/// Ako dopadlo ulozenie instalacie.
/// </summary>
internal enum SaveOutcome
{
    /// <summary>Zmenene subory sa zapisali.</summary>
    Saved,

    /// <summary>Nie je co ulozit.</summary>
    NothingToSave,

    /// <summary>Server instalacie bezi - nic sa nezapisalo, server by subory pri dalsej uprave prepisal.</summary>
    ServerRunning,

    /// <summary>Niektory subor po nacitani zmenil iny program (napr. server) - nic sa nezapisalo.</summary>
    ChangedOnDisk
}
