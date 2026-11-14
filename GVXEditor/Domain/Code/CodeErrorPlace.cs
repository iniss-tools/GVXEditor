using System.Text.RegularExpressions;
using GVXEditor.Domain.Server;

namespace GVXEditor.Domain.Code;

/// <summary>
/// Miesto chyby prekladu kodu serverom: subor, stanica, vyraz a riadok vo vyraze.
/// </summary>
/// <param name="Error">chyba z mosta</param>
internal sealed partial record CodeErrorPlace(ServerCompileError Error)
{
    /// <summary>Stavovy diagram: priecinok grafikonu; inak prazdny.</summary>
    public string Group { get; init; } = "";

    /// <summary>Stavovy diagram: priecinok stanice; inak prazdny.</summary>
    public string Station { get; init; } = "";

    /// <summary>Stavovy diagram: popis grafu; prazdny pri vyraze cisla grafu.</summary>
    public string Graph { get; init; } = "";

    /// <summary>Stavovy diagram: kluc stavu; prazdny pri vyraze cisla grafu.</summary>
    public string Node { get; init; } = "";

    /// <summary>Stavovy diagram: vyraz (<c>indg</c>, <c>c</c>, <c>t</c>, <c>m</c>, <c>r</c>, <c>e</c>).</summary>
    public string Field { get; init; } = "";

    /// <summary>Stavovy diagram: vyraz patri automatike stavu.</summary>
    public bool Automation { get; init; }

    /// <summary>TabTab: nazov tabulky; inak prazdny.</summary>
    public string Table { get; init; } = "";

    /// <summary>Chyba je varovanie kompilatora (BC4xxxx) - server ho hlasi len spolu s chybou.</summary>
    public bool IsWarning => Error.Number.StartsWith("BC4", StringComparison.Ordinal);

    /// <summary>Riadok vo vyraze alebo v subore od 1.</summary>
    public int Line => Error.Line;

    /// <summary>Preklad stavoveho diagramu.</summary>
    public bool IsStateDiagram => Error.Source == "statedgm.xml";

    /// <summary>Preklad tabuliek TabTab.</summary>
    public bool IsTabTab => Error.Source == "TabTab";

    /// <summary>Preklad LogText.vb.</summary>
    public bool IsLogText => Error.Source == "LogText.vb";

    /// <summary>Preklad podmienok zobrazenia casti tabul (server neuvadza miesto, len kod podmienky).</summary>
    public bool IsTables => Error.Source == "Tables.xml";

    /// <summary>
    /// Chyba patri podmienke s tymto kodom: server pred kod bez slova <c>return</c> doplni <c>return </c>.
    /// </summary>
    /// <param name="condition">podmienka casti tabule, ako je v subore</param>
    public bool MatchesCondition(string condition)
    {
        ArgumentNullException.ThrowIfNull(condition);
        return condition.Length > 0 && (Error.Code == condition || Error.Code == "return " + condition);
    }

    [GeneratedRegex(@"v ""(?<group>[^"":]*):(?<station>[^""]*)""(, graf ""(?<graph>[^""]*)"")?(, uzel ""(?<node>[^""]*)"")?, (?<auto>automatika )?funkce ""(?<field>\w+)""",
        RegexOptions.CultureInvariant)]
    private static partial Regex StateDiagramPlace();

    /// <summary>
    /// Miesto chyby podla popisu servera, napr. <c>Soubor "StateDgm.xml" v "2026:01 Lipovec", graf "#Vychozi", uzel "Nastup",
    /// automatika funkce "t"</c>; pri TabTab je popis nazov tabulky.
    /// </summary>
    public static CodeErrorPlace Of(ServerCompileError error)
    {
        ArgumentNullException.ThrowIfNull(error);
        var place = new CodeErrorPlace(error);
        if (place.IsTabTab)
            return place with { Table = error.Description };
        if (!place.IsStateDiagram || StateDiagramPlace().Match(error.Description) is not { Success: true } match)
            return place;

        return place with
        {
            Group = match.Groups["group"].Value,
            Station = match.Groups["station"].Value,
            Graph = match.Groups["graph"].Value,
            Node = match.Groups["node"].Value,
            Field = match.Groups["field"].Value,
            Automation = match.Groups["auto"].Success
        };
    }
}
