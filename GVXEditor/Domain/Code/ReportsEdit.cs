using System.Globalization;
using ToolsCore.Iniss2.Grafikon.Script;

namespace GVXEditor.Domain.Code;

/// <summary>
/// Skript hlaseni v okne Hlasenia: text sa meni hned po kazdej platnej uprave hlasenia, vzdy len rozsah jednej funkcie.
/// Funkcie s oznacenim hlasenia su v poradi textu; po zapise sa znova precita len zmenena funkcia.
/// </summary>
internal sealed class ReportsEdit
{
    /// <param name="text">text LogText.vb</param>
    public ReportsEdit(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        Original = text;
        Load(text);
    }

    /// <summary>Text skriptu pri otvoreni okna.</summary>
    public string Original { get; }

    /// <summary>Aktualny text skriptu.</summary>
    public string Text { get; private set; } = "";

    /// <summary>Aktualny text rozdeleny na funkcie.</summary>
    public LogTextScript Script { get; private set; } = LogTextScript.Parse("");

    /// <summary>Funkcie s oznacenim hlasenia v poradi textu: hlasenie alebo dovod rucneho kodu.</summary>
    public IReadOnlyList<ScriptReadResult> Reports { get; private set; } = [];

    /// <summary>
    /// Text skriptu z okna; bez zmeny vrati povodny text (ten isty objekt).
    /// </summary>
    public string Result() => Text == Original ? Original : Text;

    /// <summary>
    /// Nacita cely text znova (napr. po uprave v okne Skript hlaseni).
    /// </summary>
    public void Load(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        Text = text;
        Script = LogTextScript.Parse(text);
        Reports = ScriptReports.Read(Script);
    }

    /// <summary>
    /// Funkcia s oznacenim hlasenia podla nazvu (bez ohladu na velkost pismen ako VB.NET); <see langword="null" />, ak nie je.
    /// </summary>
    public ScriptReadResult? Find(string name) =>
        Reports.FirstOrDefault(result => string.Equals(result.Member.Name, name, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Zapise hlasenie na miesto funkcie <paramref name="replacing" />, bez nej ako novu funkciu.
    /// </summary>
    /// <returns>hlasenie precitane zo zapisaneho textu</returns>
    /// <exception cref="ArgumentException">nazov ma ina funkcia</exception>
    /// <exception cref="FormatException">skript nema triedu LogText</exception>
    public ScriptReadResult Write(ScriptReport report, string? replacing)
    {
        ArgumentNullException.ThrowIfNull(report);
        Text = ScriptReports.Write(Text, report, replacing);
        Script = LogTextScript.Parse(Text);
        Refresh(report.Name);
        return Find(report.Name)!;
    }

    /// <summary>
    /// Zmaze funkciu.
    /// </summary>
    public void Remove(string name)
    {
        Text = ScriptReports.Remove(Text, name);
        Script = LogTextScript.Parse(Text);
        Refresh(null);
    }

    /// <summary>
    /// Nazov, ktory este nema ziadna funkcia: zaklad, inak zaklad s cislom (<c>Nove_hlasenie_2</c>).
    /// </summary>
    public string UniqueName(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        if (Script.Find(name) is null)
            return name;
        for (var i = 2; ; i++)
        {
            var candidate = name + "_" + i.ToString(CultureInfo.InvariantCulture);
            if (Script.Find(candidate) is null)
                return candidate;
        }
    }

    /// <summary>
    /// Nazov noveho dodatku: <c>D</c> a cislo za najvacsim cislom funkcii <c>D&lt;cislo&gt;</c> (kluc dodatku v datach).
    /// </summary>
    public string NextAdditionName()
    {
        var numbers = Script.Members.Select(member => member.Name)
            .Where(name => name.Length > 1 && (name[0] == 'D' || name[0] == 'd') && name.Skip(1).All(char.IsAsciiDigit))
            .Select(name => long.TryParse(name.AsSpan(1), NumberStyles.None, CultureInfo.InvariantCulture, out var number) ? number : 0);
        return UniqueName("D" + (numbers.DefaultIfEmpty(0).Max() + 1).ToString(CultureInfo.InvariantCulture));
    }

    // zoznam po zapise: zmenena funkcia sa precita, ostatne dostanu novu polohu v texte
    private void Refresh(string? written)
    {
        var old = new Dictionary<string, ScriptReadResult>(StringComparer.OrdinalIgnoreCase);
        foreach (var result in Reports)
            old.TryAdd(result.Member.Name, result);

        Reports =
        [
            .. Script.Members.Where(member => member.Attributes.Any(attribute => ScriptReportKinds.TryParse(attribute.Name, out _)))
                .Select(member => !string.Equals(member.Name, written, StringComparison.OrdinalIgnoreCase) && old.TryGetValue(member.Name, out var result)
                    ? result with { Member = member, Line = result.Line + member.Start - result.Member.Start }
                    : ScriptReportReader.Read(Script, member))
        ];
    }
}
