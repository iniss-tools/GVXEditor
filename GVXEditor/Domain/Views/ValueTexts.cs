using System.Globalization;
using GVXEditor.Properties;
using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Views;

/// <summary>
/// Texty hodnot, ktore su v suboroch len klucom: priznaky varianty a bodu trasy, priznaky stavu a automatiky, druhy a obsah
/// tabul. Kluc ostava v zatvorke, aby sa dal najst v dokumentacii formatov.
/// </summary>
internal static class ValueTexts
{
    private static readonly GvVariantFlags[] VariantOrder =
        [GvVariantFlags.D, GvVariantFlags.X, GvVariantFlags.N, GvVariantFlags.NP, GvVariantFlags.O, GvVariantFlags.M, GvVariantFlags.R, GvVariantFlags.L];

    private static readonly GvStopFlags[] StopOrder =
        [GvStopFlags.Interest, GvStopFlags.Passing, GvStopFlags.Pr, GvStopFlags.OperatingPoint, GvStopFlags.OnRequest];

    // kluc priznaku v subore (nie nazov C#)
    private static string VariantKey(GvVariantFlags flag) => flag switch
    {
        GvVariantFlags.O => "O", GvVariantFlags.M => "M", GvVariantFlags.R => "R", GvVariantFlags.L => "L", GvVariantFlags.D => "D",
        GvVariantFlags.X => "X", GvVariantFlags.N => "N", GvVariantFlags.NP => "NP",
        _ => throw new ArgumentOutOfRangeException(nameof(flag), flag, null)
    };

    private static string StopName(GvStopFlags flag) => flag switch
    {
        GvStopFlags.Passing => "Passing", GvStopFlags.Interest => "Interest", GvStopFlags.Pr => "Pr", GvStopFlags.OperatingPoint => "OperatingPoint",
        GvStopFlags.OnRequest => "OnRequest",
        _ => throw new ArgumentOutOfRangeException(nameof(flag), flag, null)
    };

    private static string? Text(string name) => Resources.ResourceManager.GetString(name, Resources.Culture);

    /// <summary>Text s klucom v zatvorke, napr. <c>Dialkovy (D)</c>; neznamy kluc sam.</summary>
    private static string WithKey(string prefix, string key) =>
        Text(prefix + key) is { } text ? string.Format(CultureInfo.CurrentCulture, Resources.Value_WithKey, text, key) : key;

    /// <summary>Priznak varianty pre zaciarkavacie policko, napr. <c>Dialkovy (D)</c>.</summary>
    public static string VariantFlag(GvVariantFlags flag) => WithKey("Flag_Variant_", VariantKey(flag));

    /// <summary>Co priznak varianty robi na serveri a v datach (napoveda).</summary>
    public static string VariantFlagTip(GvVariantFlags flag) => Text("Flag_VariantTip_" + VariantKey(flag)) ?? "";

    /// <summary>Priznaky varianty slovami do zoznamu, napr. <c>dialkovy, nizkopodlazny</c>.</summary>
    public static string VariantFlags(GvVariantFlags flags) =>
        string.Join(", ", VariantOrder.Where(flag => flags.HasFlag(flag)).Select(flag => Text("Flag_VariantShort_" + VariantKey(flag)) ?? VariantKey(flag)));

    /// <summary>Hlavicka stlpca priznaku bodu trasy, napr. <c>Prejazd</c>.</summary>
    public static string StopFlag(GvStopFlags flag) => Text("Flag_Stop_" + StopName(flag)) ?? StopName(flag);

    /// <summary>Co priznak bodu trasy znamena (napoveda).</summary>
    public static string StopFlagTip(GvStopFlags flag) => Text("Flag_StopTip_" + StopName(flag)) ?? "";

    /// <summary>Priznaky bodu trasy slovami do zoznamu, napr. <c>zaujmova, prejazd</c>.</summary>
    public static string StopFlags(GvStopFlags flags) =>
        string.Join(", ", StopOrder.Where(flag => flags.HasFlag(flag)).Select(flag => Text("Flag_StopShort_" + StopName(flag)) ?? StopName(flag)));

    /// <summary>Priznak stavu alebo automatiky, napr. <c>Stoji v stanici (STOJI)</c>.</summary>
    public static string NodeFlag(string key) => WithKey("Flag_Node_", key);

    /// <summary>Priznak stavu alebo automatiky slovami bez kluca; priznak bez znameho vyznamu klucom.</summary>
    public static string NodeFlagText(string key) => Text("Flag_Node_" + key) ?? key;

    /// <summary>Druh casti tabule, napr. <c>Nastupistna (PLATFORM)</c>.</summary>
    public static string PartKind(string key) => WithKey("Value_Kind_", key);

    /// <summary>Druh zariadenia predlohy, napr. <c>LCD s vrstvami (LCD1)</c>.</summary>
    public static string DeviceClass(string key) => WithKey("Value_Class_", key);

    /// <summary>Obsah udaja predlohy, napr. <c>Cas odchodu (CasOdjezdu)</c>.</summary>
    public static string Content(string key) => WithKey("Value_Content_", key);

    /// <summary>Zarovnanie udaja slovom.</summary>
    public static string Align(string key) => Text("Value_Align_" + key) ?? key;

    /// <summary>Sposob prekladu, napr. <c>Bez prekladu (FREE)</c>.</summary>
    public static string Translation(string key) => WithKey("Value_Translation_", key);

    /// <summary>Sposob prekladu slovami bez kluca (do uzkeho stlpca tabulky).</summary>
    public static string TranslationText(string key) => Text("Value_Translation_" + key) ?? key;

    /// <summary>
    /// Vyber hodnot: zname kluce s textom, chybajuci atribut ako predvolena hodnota a hodnoty, ktore su v datach navyse.
    /// </summary>
    /// <param name="known">kluce, ktore server pozna</param>
    /// <param name="text">text kluca</param>
    /// <param name="defaultKey">kluc, ktory server pouzije pri chybajucom atribute; <see langword="null" /> = atribut nechyba</param>
    /// <param name="present">hodnoty v datach</param>
    public static List<ValueChoice> Choices(IReadOnlyList<string> known, Func<string, string> text, string? defaultKey, IEnumerable<string> present)
    {
        ArgumentNullException.ThrowIfNull(known);
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(present);
        var choices = new List<ValueChoice>();
        if (defaultKey is not null)
            choices.Add(new ValueChoice("", string.Format(CultureInfo.CurrentCulture, Resources.Value_Default, text(defaultKey))));
        choices.AddRange(known.Select(key => new ValueChoice(key, text(key))));
        choices.AddRange(present.Where(value => value.Length > 0 && !known.Contains(value, StringComparer.Ordinal)).Distinct(StringComparer.Ordinal)
            .Select(value => new ValueChoice(value, value)));
        return choices;
    }
}
