using System.ComponentModel;
using ToolsCore.Iniss2.Grafikon.Script;

namespace GVXEditor.Domain.Code.Drafts;

/// <summary>
/// Hlasenie v okne Hlasenia: oznacenie funkcie a kroky po jazykoch. Obmedzenie na stanice je text ako v kode (cisla
/// oddelene ciarkou), typ vyluky kluc <c>TYPVYLUKY.*</c>.
/// </summary>
internal sealed class ReportDraft
{
    /// <summary>Nazov parametra so stanicami, ktory dostane novy vzor (ako v korpuse).</summary>
    public const string DefaultStationsParameter = "aiStanice";

    // poradove argumenty oznacenia, ktore okno neupravuje (druh bez typu vyluky) - ostanu, kym sa druh nezmeni
    private IReadOnlyList<string> _otherArguments = [];
    private ScriptReportKind _otherArgumentsKind;
    private string? _stationsParameter;

    /// <summary>Nazov funkcie = kluc hlasenia.</summary>
    public string Name { get; set; } = "";

    /// <summary>Druh hlasenia podla oznacenia.</summary>
    public ScriptReportKind Kind { get; set; } = ScriptReportKind.Other;

    /// <summary>Popis z oznacenia, ktory ukazuje klient.</summary>
    public string Description { get; set; } = "";

    /// <summary>Obmedzenie <c>ProStanice</c>: cisla stanic oddelene ciarkou; prazdne = bez obmedzenia.</summary>
    public string ForStations { get; set; } = "";

    /// <summary>Obmedzenie <c>MimoStanice</c>; prazdne = bez obmedzenia.</summary>
    public string NotStations { get; set; } = "";

    /// <summary>Typ vyluky (vyluka a vzor vyluky), napr. <c>TYPVYLUKY.AUTO</c>.</summary>
    public string ExclusionType { get; set; } = "";

    /// <summary>Stanice oznacenia <c>Vyluka</c>: cisla oddelene ciarkou.</summary>
    public string ExclusionStations { get; set; } = "";

    /// <summary>Hlasenie len pre jazyk; prazdne = vsetky jazyky.</summary>
    public string OnlyLanguage { get; set; } = "";

    /// <summary>Podmienky, pri ktorych sa hlasenie nevyhlasi: vyrazy VB.NET, kazdy na vlastnom riadku.</summary>
    public string Guards { get; set; } = "";

    /// <summary>Kroky v sekcii: pri chybe zvuku sa vysledok hlasenia vrati do stavu pred sekciou.</summary>
    public bool Section { get; set; }

    /// <summary>Kroky v poradi kodu, kazdy so svojim jazykovym blokom.</summary>
    public BindingList<StepDraft> Steps { get; } = [];

    /// <summary>
    /// Koncept hlasenia zo skriptu.
    /// </summary>
    public static ReportDraft Of(ScriptReport report)
    {
        ArgumentNullException.ThrowIfNull(report);
        var draft = new ReportDraft
        {
            Name = report.Name,
            Kind = report.Kind,
            Description = report.Description,
            ForStations = report.ForStations ?? "",
            NotStations = report.NotStations ?? "",
            OnlyLanguage = report.OnlyLanguage ?? "",
            Guards = string.Join(Environment.NewLine, report.Guards),
            Section = report.Section,
            _stationsParameter = report.StationsParameter
        };

        if (IsExclusion(report.Kind))
        {
            draft.ExclusionType = report.Arguments.Count > 0 ? report.Arguments[0] : "";
            draft.ExclusionStations = string.Join(",", report.Arguments.Skip(1));
        }
        else
        {
            draft._otherArguments = report.Arguments;
            draft._otherArgumentsKind = report.Kind;
        }

        foreach (var block in report.Blocks)
        foreach (var step in block.Steps)
            draft.Steps.Add(StepDraft.Of(block.Language, step, block.Conditions));
        return draft;
    }

    /// <summary>
    /// Hlasenie z konceptu; <see langword="null" />, ak niektory krok alebo podmienka nie je platna (ukaze ich kontrola).
    /// Susedne kroky s rovnakym jazykom a podmienkami su jeden blok.
    /// </summary>
    public ScriptReport? Build()
    {
        var blocks = new List<ScriptBlock>();
        var steps = new List<ScriptStep>();
        string? language = null;
        var conditions = "";
        foreach (var draft in Steps)
        {
            if (draft.Build() is not { } step || StepDraft.Conditions(draft.Condition) is null)
                return null;
            var block = draft.Block.Trim().Length == 0 ? null : draft.Block.Trim();
            var condition = draft.Condition.Trim();
            if (steps.Count > 0 && (block != language || condition != conditions))
            {
                blocks.Add(new ScriptBlock(language, steps) { Conditions = StepDraft.Conditions(conditions)! });
                steps = [];
            }

            language = block;
            conditions = condition;
            steps.Add(step);
        }

        if (steps.Count > 0)
            blocks.Add(new ScriptBlock(language, steps) { Conditions = StepDraft.Conditions(conditions)! });

        var limit = ScriptReportKinds.HasStationLimit(Kind);
        return new ScriptReport(Name.Trim(), Kind)
        {
            Description = Description,
            Arguments = Arguments(),
            ForStations = limit && ForStations.Trim().Length > 0 ? ForStations.Trim() : null,
            NotStations = limit && NotStations.Trim().Length > 0 ? NotStations.Trim() : null,
            StationsParameter = ScriptReportKinds.IsPattern(Kind) ? _stationsParameter ?? DefaultStationsParameter : null,
            OnlyLanguage = OnlyLanguage.Trim().Length == 0 ? null : OnlyLanguage.Trim(),
            Guards = GuardList(Guards),
            Section = Section,
            Blocks = blocks
        };
    }

    /// <summary>Podmienky ukoncenia z textu: neprazdne riadky bez okrajovych medzier.</summary>
    public static List<string> GuardList(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        return [.. text.Split('\n').Select(line => line.Trim()).Where(line => line.Length > 0)];
    }

    /// <summary>Druh s typom vyluky (vyluka a vzor vyluky).</summary>
    public static bool IsExclusion(ScriptReportKind kind) => kind is ScriptReportKind.Exclusion or ScriptReportKind.ExclusionPattern;

    // poradove argumenty oznacenia podla druhu
    private IReadOnlyList<string> Arguments()
    {
        if (!IsExclusion(Kind))
            return Kind == _otherArgumentsKind ? _otherArguments : [];

        var arguments = new List<string> { ExclusionType.Trim() };
        if (Kind == ScriptReportKind.Exclusion)
            arguments.AddRange(ExclusionStations.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
        return arguments;
    }
}
