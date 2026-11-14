namespace GVXEditor.Domain.Stations.Texts;

/// <summary>
/// Vygenerovany text vlaku pre sadu TTVlaky.xml.
/// </summary>
/// <param name="Number">cislo vlaku pri odchode zo stanice</param>
/// <param name="Validity">datumova poznamka varianty, ak sa texty variant s rovnakym cislom lisia; inak prazdna</param>
/// <param name="Text">text, casti oddelene <c>#</c>; prazdny = vlak text nedostane</param>
internal sealed record GeneratedText(string Number, string Validity, string Text)
{
    /// <summary>
    /// Text, ktory by server pre udaj ukazal bez zaznamu v sade: podla trasy (TTTrasy.xml), stanice (TTStanice.xml) alebo
    /// vypocitany; <see langword="null" />, ak server udaj nevyplni.
    /// </summary>
    public string? ServerText { get; init; }

    /// <summary>Server by aj bez zaznamu ukazal vo vsetkych variantach rovnaky text - zaznam netreba.</summary>
    public bool IsRedundant { get; init; }

    /// <summary>
    /// Server by aspon v niektorej variante ukazal neprazdny text podla trasy (TTTrasy.xml) alebo stanice (TTStanice.xml) -
    /// doplnenie chybajucich textov ho neprekryje.
    /// </summary>
    public bool HasFileText { get; init; }
}
