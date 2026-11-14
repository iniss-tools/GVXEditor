using System.Globalization;
using GVXEditor.Properties;
using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Stations;

/// <summary>
/// Kluce dodatkov, vyluk a odklonov: cislo je hlasenie obsluhy z ConstReports.xml, text je funkcia LogText.vb.
/// </summary>
/// <param name="Functions">funkcie LogText.vb; <see langword="null" />, ak subor chyba (textovy kluc sa potom neoveruje)</param>
/// <param name="Descriptions">popisy funkcii z ich oznaceni</param>
/// <param name="ConstReports">hlasenia obsluhy</param>
internal sealed record KeyCatalog(
    IReadOnlyDictionary<string, HashSet<string>>? Functions,
    IReadOnlyDictionary<string, string>? Descriptions,
    IReadOnlyList<ConstReport> ConstReports)
{
    /// <summary>
    /// Kluc existuje; prazdny kluc (zaznam nic neprehra) je v poriadku.
    /// </summary>
    public bool IsKnown(string key)
    {
        var text = key.Trim();
        if (text.Length == 0)
            return true;
        if (int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var number))
            return ConstReports.Any(report => report.Key == number);
        return Functions is null || Functions.ContainsKey(text);
    }

    /// <summary>
    /// Kluce na vyber: funkcie LogText s oznacenim (napr. <c>Dodatek</c>), hlasenia obsluhy cislom a hodnoty, ktore su
    /// v datach. Text je kluc s popisom, prazdny kluc (zaznam nic neprehra) je na zaciatku.
    /// </summary>
    /// <param name="attribute">oznacenie funkcie, napr. Dodatek, Vyluka, Odklon</param>
    /// <param name="present">kluce v datach</param>
    public List<Views.ValueChoice> Choices(string attribute, IEnumerable<string> present) =>
        Choices([attribute], ConstReports.OrderBy(report => report.Key).Select(report => report.Key.ToString(CultureInfo.InvariantCulture)), present);

    /// <summary>
    /// Funkcie LogText s niektorym z oznaceni (napr. vzory hlaseni obsluhy) a hodnoty, ktore su v datach.
    /// </summary>
    public List<Views.ValueChoice> FunctionChoices(IReadOnlyCollection<string> attributes, IEnumerable<string> present) => Choices(attributes, [], present);

    private List<Views.ValueChoice> Choices(IReadOnlyCollection<string> attributes, IEnumerable<string> reports, IEnumerable<string> present)
    {
        ArgumentNullException.ThrowIfNull(present);
        var keys = new List<string> { "" };
        if (Functions is not null)
            keys.AddRange(Functions.Where(pair => pair.Value.Overlaps(attributes)).Select(pair => pair.Key).Order(StringComparer.Ordinal));
        keys.AddRange(reports);
        keys.AddRange(present.Where(key => !keys.Contains(key, StringComparer.Ordinal)).Distinct(StringComparer.Ordinal));
        return [.. keys.Select(key => new Views.ValueChoice(key, Choice(key)))];
    }

    // kluc a popis v jednom texte (vo vybere nie je stlpec popisu)
    private string Choice(string key)
    {
        var description = Describe(key);
        return description.Length == 0 ? key : string.Format(CultureInfo.CurrentCulture, Resources.Key_Choice, key, description);
    }

    /// <summary>
    /// Popis kluca pre obsluhu: nazov hlasenia obsluhy, popis funkcie LogText alebo jej oznacenia.
    /// </summary>
    public string Describe(string key)
    {
        var text = key.Trim();
        if (text.Length == 0)
            return "";
        if (int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var number))
            return ConstReports.FirstOrDefault(report => report.Key == number)?.Name ?? Resources.Key_Unknown;
        if (Descriptions is not null && Descriptions.TryGetValue(text, out var description))
            return description;
        if (Functions is null)
            return "";
        return Functions.TryGetValue(text, out var attributes)
            ? string.Format(CultureInfo.CurrentCulture, Resources.Key_Function, string.Join(", ", attributes))
            : Resources.Key_Unknown;
    }
}
