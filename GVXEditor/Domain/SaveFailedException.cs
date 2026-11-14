namespace GVXEditor.Domain;

/// <summary>
/// Zapis zlyhal. Subory sa vratili do stavu pred ulozenim, alebo ostali v zalohe.
/// </summary>
internal sealed class SaveFailedException : Exception
{
    /// <param name="rolledBack">subory sa vratili do stavu pred ulozenim</param>
    /// <param name="backupPath">zaloha povodnych suborov, ak sa ich nepodarilo vratit</param>
    /// <param name="inner">povodna chyba</param>
    public SaveFailedException(bool rolledBack, string backupPath, Exception inner) : base(inner.Message, inner)
    {
        RolledBack = rolledBack;
        BackupPath = backupPath;
    }

    /// <summary>Subory sa vratili do stavu pred ulozenim.</summary>
    public bool RolledBack { get; }

    /// <summary>Zaloha povodnych suborov.</summary>
    public string BackupPath { get; }
}
