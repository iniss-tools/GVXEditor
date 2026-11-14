namespace GVXEditor.Domain.Boards;

/// <summary>
/// Hodnoty vymenovani servera pre tabule a predlohy (XmlSerializer cita len tieto nazvy, s rozlisenim velkosti pismen).
/// </summary>
internal static class BoardValues
{
    /// <summary>Druhy casti tabule (<c>t</c>, TABLETYPE); chybajuci = NODEF.</summary>
    public static IReadOnlyList<string> PartKinds { get; } = ["NODEF", "DEPARTURE", "ARRIVAL", "PLATFORM", "SUBWAY", "INFO"];

    /// <summary>Druhy zariadenia predlohy (<c>cls</c>, TABLECLASS).</summary>
    public static IReadOnlyList<string> Classes { get; } =
    [
        "INTERNET", "LCD", "LCD1", "LCD2", "ERP", "ERP2", "FERS", "ELEN", "ELENOLD", "ELEN10", "ELEN16", "ELEN16KAM", "ELENWIN", "ELEKON", "ELEKONKAM",
        "ERP_ELEN", "HERMAN", "HERMANMULTIOKNO", "HERMAN_LCD", "RGPLUS", "RGPLUS_COLOR", "MSVRADOM", "ERS", "APEL", "APEL_AN", "ELSVO"
    ];

    /// <summary>Druhy, ktore server pozna, ale tabulu s nimi nespusti.</summary>
    public static IReadOnlySet<string> UnsupportedClasses { get; } = new HashSet<string>(["ERS", "APEL", "APEL_AN", "ELSVO"], StringComparer.Ordinal);

    /// <summary>Druhy s bankou pisem v binarnom subore TableFonts\&lt;f&gt;.bin - atribut <c>f</c> je povinny.</summary>
    public static IReadOnlySet<string> BinaryFontClasses { get; } =
        new HashSet<string>(["ELEN", "ELENOLD", "ELEN10", "ELEN16", "ELEN16KAM", "ELENWIN"], StringComparer.Ordinal);

    /// <summary>Druhy s predvolenou bankou pisem servera - s atributom <c>f</c> server banku nenajde.</summary>
    public static IReadOnlySet<string> DefaultFontClasses { get; } = new HashSet<string>(["ELEKON", "ELEKONKAM"], StringComparer.Ordinal);

    /// <summary>Druh s multioknami (atribut <c>windef</c> casti).</summary>
    public const string MultiWindowClass = "HERMANMULTIOKNO";

    /// <summary>Druh casti s informacnym textom - jediny, ktory pouzije webovy zdroj.</summary>
    public const string InfoKind = "INFO";

    /// <summary>Druhy obsahu udaja (<c>inf</c>, ARGSPANEL); chybajuci = Not.</summary>
    public static IReadOnlyList<string> Contents { get; } =
    [
        "Not", "NoDef", "Free", "VychoziNazev", "PredNazev", "PoNazevNastupiste", "PoNazev", "CilNazevNastupiste", "CilNazevPodchod", "CilNazev",
        "CasOdjezdu", "CasPrijezdu", "CasOdjezduAktualni", "CasPrijezduAktualni", "ZpozdeniPrijezd", "ZpozdeniOdjezd", "ZpozdeniPrijezdPopis",
        "ZpozdeniOdjezdPopis", "MinutyDoOdjezdu", "TypVlaku", "CisloVlaku", "TypCisloVlaku", "TypCisloVlaku6", "TypCisloVlakuKompr", "TypNazevOrCislo",
        "NazevVlaku", "Dopravce", "Linka", "PriznakyVlaku", "HexTypVlakuEx", "NastupistePrijezd", "NastupisteOdjezd", "KolejPrijezd", "KolejOdjezd",
        "NastKolejPrijezd", "NastKolejOdjezd", "KolejAltPrijezd", "KolejAltOdjezd", "SektorPrijezd", "SektorOdjezd", "VlakStojiVeStanici", "TextLine1",
        "TextLine2", "ZobrazenoPrijezd", "ZobrazenoOdjezd", "ZobrazenoNast", "ZobrazenoPodch", "HermanLedMp3", "TabName"
    ];

    /// <summary>Zarovnanie udaja (<c>align</c>, ALIGN); chybajuce = LEFT.</summary>
    public static IReadOnlyList<string> Aligns { get; } = ["LEFT", "RIGHT", "CENTER"];

    /// <summary>Sposoby prekladu hodnoty (<c>trn</c>, TRANSLATETYPE); chybajuci = FREE.</summary>
    public static IReadOnlyList<string> Translations { get; } = ["FREE", "TABLE", "TABLETIME", "TRANSLATE", "CHAR"];

    /// <summary>
    /// Hodnoty pre vyber: zname hodnoty, prazdna (atribut chyba) a hodnoty, ktore uz su v datach (aby ich vyber ukazal).
    /// </summary>
    public static List<string> Choices(IReadOnlyList<string> known, IEnumerable<string> present) =>
        [.. known, .. present.Prepend("").Where(value => !known.Contains(value, StringComparer.Ordinal)).Distinct(StringComparer.Ordinal)];
}
