namespace GVXEditor.Domain;

/// <summary>
/// Cas zmeny a velkost suboru pri nacitani - podla nich sa pred ulozenim zisti, ci subor medzicasom zmenil iny program.
/// </summary>
/// <param name="LastWriteUtc">cas poslednej zmeny</param>
/// <param name="Length">velkost v bajtoch; -1 = subor neexistoval</param>
internal sealed record FileStamp(DateTime LastWriteUtc, long Length)
{
    /// <summary>
    /// Stav suboru teraz.
    /// </summary>
    public static FileStamp Of(string file)
    {
        var info = new FileInfo(file);
        return info.Exists ? new FileStamp(info.LastWriteTimeUtc, info.Length) : new FileStamp(DateTime.MinValue, -1);
    }
}
