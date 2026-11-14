namespace IniSrvBridge;

/// <summary>
/// Chyba alebo varovanie prekladu kodu VB.NET (server zastavi aj varovanie). Trieda, nie record - .NET Framework nema
/// IsExternalInit.
/// </summary>
internal sealed class CompileError
{
    /// <param name="source">co sa prekladalo: statedgm.xml, TabTab, Tables.xml alebo LogText.vb</param>
    /// <param name="line">riadok v kode (u vyrazov riadok vyrazu)</param>
    /// <param name="column">stlpec</param>
    /// <param name="number">cislo chyby kompilatora, napr. BC30451</param>
    /// <param name="message">text chyby</param>
    /// <param name="description">kde je vyraz (napr. stanica a uzol), ak ho server uvadza</param>
    /// <param name="code">kod vyrazu</param>
    public CompileError(string source, int line, int column, string number, string message, string description, string code)
    {
        Source = source;
        Line = line;
        Column = column;
        Number = number;
        Message = message;
        Description = description;
        Code = code;
    }

    public string Source { get; }

    public int Line { get; }

    public int Column { get; }

    public string Number { get; }

    public string Message { get; }

    public string Description { get; }

    public string Code { get; }
}
