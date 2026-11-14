using System.Globalization;
using GVXEditor.Domain.Server;
using GVXEditor.Properties;

namespace GVXEditor.Domain.Analysis;

/// <summary>
/// Zistenia z overenia kodom servera: chyba nacitania a prekladu ako chyba, zaznamy Error.log ako upozornenia.
/// </summary>
internal static class ServerFindings
{
    private const int MaxMessage = 400;

    /// <summary>
    /// Zistenia z vysledku mosta.
    /// </summary>
    public static List<Finding> Of(ServerVerification verification)
    {
        ArgumentNullException.ThrowIfNull(verification);
        var findings = new List<Finding>();

        // pri chybe prekladu su konkretne chyby v zozname, celkove hlasenie by sa opakovalo
        if (verification.Fatal is { } fatal && verification.CompileErrors.Count == 0)
            findings.Add(new Finding(FindingSeverity.Error, -1, -1, Resources.Place_Server, Text(Resources.Check_ServerFatal, verification.Stage ?? "", fatal)));

        foreach (var error in verification.CompileErrors)
        {
            var place = error.Description.Length > 0 ? error.Description.Split('\n')[0].Trim() : error.Source;
            findings.Add(new Finding(FindingSeverity.Error, -1, -1, place,
                Text(Resources.Check_ServerCompile, error.Source, error.Line, error.Number, error.Message)));
        }

        foreach (var entry in verification.Log)
            findings.Add(new Finding(FindingSeverity.Warning, -1, -1, "Error.log", Text(Resources.Check_ServerLog, Summary(entry))));

        return findings;
    }

    // zaznam logu bez vypisu zasobnika, riadky spojene
    private static string Summary(string entry)
    {
        var lines = entry.Split('\n').Select(line => line.TrimEnd('\r')).Where(line => !line.TrimStart().StartsWith("at ", StringComparison.Ordinal))
            .Select(line => line.Trim()).Where(line => line.Length > 0);
        var text = string.Join(" ", lines);
        return text.Length > MaxMessage ? text[..MaxMessage] + "…" : text;
    }

    private static string Text(string format, params object[] args) => string.Format(CultureInfo.CurrentCulture, format, args);
}
