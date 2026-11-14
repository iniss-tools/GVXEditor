namespace GVXEditor.Domain.Code;

/// <summary>
/// Kod vlozeny v XML (vyraz TabTab, podmienka stavu): v subore je casto odsadeny pod znackou a obaleny prazdnymi riadkami.
/// Editor ukazuje kod bez tohto obalu a pri zapise zmeneneho kodu obal obnovi, nezmeneny kod ostane po znakoch.
/// </summary>
internal static class EmbeddedCode
{
    /// <summary>
    /// Kod na zobrazenie: bez uvodnych prazdnych riadkov, bez koncovych bielych znakov a bez spolocneho odsadenia riadkov.
    /// Jednoriadkovy kod ostane, ako je.
    /// </summary>
    public static string Display(string raw)
    {
        ArgumentNullException.ThrowIfNull(raw);
        if (!raw.Contains('\n', StringComparison.Ordinal))
            return raw;

        var lines = Lines(raw);
        var first = lines.FindIndex(line => line.Trim().Length > 0);
        if (first < 0)
            return "";
        var last = lines.FindLastIndex(line => line.Trim().Length > 0);
        var body = lines.GetRange(first, last - first + 1);
        var indent = body.Where(line => line.Trim().Length > 0).Min(line => line.Length - line.TrimStart(' ', '\t').Length);
        return string.Join("\n", body.Select(line => line.Length >= indent ? line[indent..] : line.TrimStart(' ', '\t')).Select(line => line.TrimEnd()));
    }

    /// <summary>
    /// Pocet riadkov suboru pred prvym riadkom zobrazeneho kodu - riadok chyby servera minus tento posun je riadok v editore.
    /// </summary>
    public static int LineOffset(string raw)
    {
        ArgumentNullException.ThrowIfNull(raw);
        var lines = Lines(raw);
        var first = lines.FindIndex(line => line.Trim().Length > 0);
        return first < 0 ? 0 : first;
    }

    /// <summary>
    /// Kod na zapis: bez zmeny voci zobrazeniu vrati <paramref name="original" />. Viacriadkovy kod dostane obal povodneho
    /// kodu (odsadenie riadkov a riadok pred koncovou znackou), pri novom kode odsadenie <paramref name="indent" />.
    /// </summary>
    /// <param name="edited">kod z editora</param>
    /// <param name="original">kod v subore</param>
    /// <param name="indent">odsadenie riadkov noveho viacriadkoveho kodu</param>
    /// <param name="closing">odsadenie koncovej znacky noveho viacriadkoveho kodu</param>
    public static string Store(string edited, string original, string indent, string closing)
    {
        ArgumentNullException.ThrowIfNull(edited);
        ArgumentNullException.ThrowIfNull(original);
        var code = edited.Replace("\r\n", "\n", StringComparison.Ordinal).TrimEnd();
        if (code == Display(original))
            return original;
        if (code.Length == 0)
            return "";

        var wrapped = original.Contains('\n', StringComparison.Ordinal) && original.Trim().Length > 0;
        if (!wrapped && !code.Contains('\n', StringComparison.Ordinal))
            return code;

        if (wrapped)
        {
            var lines = Lines(original);
            var body = lines.Where(line => line.Trim().Length > 0).ToList();
            indent = body.Count > 0 ? body[0][..(body[0].Length - body[0].TrimStart(' ', '\t').Length)] : indent;
            closing = lines[^1].Trim().Length == 0 ? lines[^1] : closing;
        }

        var newline = original.Contains("\r\n", StringComparison.Ordinal) ? "\r\n" : "\n";
        return newline + string.Join(newline, code.Split('\n').Select(line => line.Length == 0 ? "" : indent + line)) + newline + closing;
    }

    private static List<string> Lines(string text) => [.. text.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n')];
}
