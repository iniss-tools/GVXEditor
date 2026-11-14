namespace GVXEditor.Domain;

/// <summary>
/// Zapis jedneho suboru pri ulozeni instalacie.
/// </summary>
/// <param name="Path">subor</param>
/// <param name="HasContent">zoznam nie je prazdny - subor, ktory neexistuje, vznikne len s obsahom</param>
/// <param name="Write">zapis (uprava na mieste, vrati <see langword="false" />, ak subor uz mal tento obsah)</param>
internal sealed record FileWrite(string Path, bool HasContent, Func<bool> Write)
{
    /// <summary>
    /// Zapis treba vykonat: existujuci subor sa upravi, novy vznikne len s obsahom.
    /// </summary>
    public bool IsNeeded => HasContent || File.Exists(Path);
}
