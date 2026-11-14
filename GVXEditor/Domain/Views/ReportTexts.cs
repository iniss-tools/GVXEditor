using System.Globalization;
using GVXEditor.Domain.Code.Drafts;
using GVXEditor.Properties;
using ToolsCore.Iniss2.Grafikon.Script;

namespace GVXEditor.Domain.Views;

/// <summary>
/// Texty okna Hlasenia: druhy hlaseni, druhy krokov, typy vyluk a dovody rucneho kodu.
/// </summary>
internal static class ReportTexts
{
    /// <summary>Typy vyluky servera (hodnoty TYPVYLUKY) v poradi dokumentacie.</summary>
    public static IReadOnlyList<string> ExclusionTypes { get; } = ["NONE", "TRATOVA", "PRIJEZD", "ODJEZD", "PRIJODJ", "AUTO"];

    /// <summary>Predvoleny typ vyluky noveho hlasenia.</summary>
    public const string DefaultExclusionType = "TYPVYLUKY.AUTO";

    /// <summary>Text druhu hlasenia.</summary>
    public static string Kind(ScriptReportKind kind) => kind switch
    {
        ScriptReportKind.Basic => Resources.Reports_KindBasic,
        ScriptReportKind.Supplementary => Resources.Reports_KindSupplementary,
        ScriptReportKind.Other => Resources.Reports_KindOther,
        ScriptReportKind.Addition => Resources.Reports_KindAddition,
        ScriptReportKind.Exclusion => Resources.Reports_KindExclusion,
        ScriptReportKind.Diversion => Resources.Reports_KindDiversion,
        ScriptReportKind.AdditionPattern => Resources.Reports_KindAdditionPattern,
        ScriptReportKind.ExclusionPattern => Resources.Reports_KindExclusionPattern,
        ScriptReportKind.DiversionPattern => Resources.Reports_KindDiversionPattern,
        ScriptReportKind.Generic => Resources.Reports_KindGeneric,
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
    };

    /// <summary>Druhy hlaseni na vyber: kluc je nazov oznacenia.</summary>
    public static List<ValueChoice> Kinds() =>
        [.. ScriptReportKinds.All.Select(kind => new ValueChoice(ScriptReportKinds.AttributeName(kind), Kind(kind)))];

    /// <summary>Druhy krokov na vyber: zvuk, pauza, vypnuty riadok, volanie funkcie a vstavane metody servera.</summary>
    public static List<ValueChoice> StepKinds() =>
    [
        new(StepDraft.SoundKind, Resources.Reports_StepSound),
        new(StepDraft.PauseKind, Resources.Reports_StepPause),
        new(StepDraft.OffKind, Resources.Reports_StepOff),
        new(StepDraft.CallKind, Resources.Reports_StepCall),
        .. ScriptBuiltins.All.Select(builtin => new ValueChoice(builtin.Method,
            string.Format(CultureInfo.CurrentCulture, Resources.Value_WithKey, Builtin(builtin.Method), builtin.Method)))
    ];

    // casto pouzivane podmienky (vyraz VB.NET, kluc textu); okno ich ponuka ako pomoc, podmienka moze byt aj iny vyraz
    private static readonly (string Expression, string Key)[] ConditionCatalog =
    [
        ("VlakS.JeVychozi", "Reports_CondStarting"),
        ("VlakS.JeProjizdejici", "Reports_CondPassing"),
        ("VlakS.JeKoncici", "Reports_CondEnding"),
        ("Not VlakS.JeVychozi", "Reports_CondNotStarting"),
        ("Not VlakS.JeKoncici", "Reports_CondNotEnding"),
        ("VlakS.JeVychozi OrElse VlakS.JeProjizdejici", "Reports_CondDeparting"),
        ("KratkeHlaseni", "Reports_CondShort"),
        ("DlouheHlaseni", "Reports_CondLong"),
        ("HlasiSeDodatek", "Reports_CondAddition"),
        ("Automatika", "Reports_CondAutomatic"),
        ("VlakS.VylukaPrij", "Reports_CondExclusionArrival"),
        ("VlakS.VylukaOdj", "Reports_CondExclusionDeparture"),
        ("VlakS.JeOdklonen", "Reports_CondDiverted"),
        ("(VlakS.PriznakyStanice And VlajkyStanice.PR) = VlajkyStanice.PR", "Reports_CondTransfer"),
        ("(aiStanice IsNot Nothing) AndAlso (aiStanice.Length = 1)", "Reports_CondOneStation"),
        ("(aiStanice IsNot Nothing) AndAlso (aiStanice.Length > 1)", "Reports_CondMoreStations")
    ];

    /// <summary>
    /// Casto pouzivane podmienky: kluc je vyraz VB.NET, text popis s vyrazom.
    /// </summary>
    public static List<ValueChoice> Conditions() =>
    [
        .. ConditionCatalog.Select(item => new ValueChoice(item.Expression, string.Format(CultureInfo.CurrentCulture, Resources.Value_WithKey,
            Resources.ResourceManager.GetString(item.Key, Resources.Culture) ?? item.Expression, item.Expression)))
    ];

    /// <summary>Text vstavanej metody servera, napr. <c>Druh vlaku</c>; neznama metoda nazvom.</summary>
    public static string Builtin(string method) =>
        Resources.ResourceManager.GetString("Reports_Builtin" + method, Resources.Culture) ?? method;

    /// <summary>
    /// Napoveda parametrov vstavanej metody, napr. <c>skupina, cas</c>; nepovinne v hranatych zatvorkach.
    /// </summary>
    public static string Parameters(ScriptBuiltin builtin)
    {
        ArgumentNullException.ThrowIfNull(builtin);
        return string.Join(", ", builtin.Parameters.Select(parameter =>
        {
            var text = Resources.ResourceManager.GetString("Reports_Param" + parameter.Name, Resources.Culture) ?? parameter.Name;
            return parameter.Optional ? "[" + text + "]" : text;
        }));
    }

    /// <summary>Typy vyluky na vyber: kluc <c>TYPVYLUKY.*</c>, text s hodnotou v zatvorke; prazdny kluc na zaciatku.</summary>
    public static List<ValueChoice> Exclusions() =>
    [
        new("", ""),
        .. ExclusionTypes.Select(type => new ValueChoice("TYPVYLUKY." + type, string.Format(CultureInfo.CurrentCulture, Resources.Value_WithKey,
            Resources.ResourceManager.GetString("Reports_Exclusion" + type, Resources.Culture) ?? type, type)))
    ];

    /// <summary>Dovod rucneho kodu pre obsluhu.</summary>
    public static string Manual(ScriptManualReason reason) => reason switch
    {
        ScriptManualReason.NotReport => Resources.Reports_ManualNotReport,
        ScriptManualReason.Signature => Resources.Reports_ManualSignature,
        ScriptManualReason.Attribute => Resources.Reports_ManualAttribute,
        ScriptManualReason.Statement => Resources.Reports_ManualStatement,
        ScriptManualReason.Layout => Resources.Reports_ManualLayout,
        _ => ""
    };
}
