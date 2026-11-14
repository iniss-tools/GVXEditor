using System.Globalization;
using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Code.Drafts;

/// <summary>
/// Stav stavoveho diagramu v okne: udaje stavu, tlacidla a automatiky. Kod vyrazov je v tvare na zobrazenie.
/// </summary>
internal sealed class NodeDraft
{
    // odsadenie viacriadkovej podmienky v subore: prvok c je pod stavom v grafe
    private const string ConditionIndent = "        ";
    private const string ConditionClosing = "      ";

    /// <summary>Stav zo suboru; <see langword="null" /> = novy stav.</summary>
    public StateNode? Original { get; private init; }

    /// <summary>Kluc stavu.</summary>
    public string Key { get; set; } = "";

    /// <summary>Nazov.</summary>
    public string Name { get; set; } = "";

    /// <summary>Nazov hlasenia z ReportKeys.xml; prazdny = bez hlasenia.</summary>
    public string Report { get; set; } = "";

    /// <summary>Priznaky stavu v poradi suboru.</summary>
    public List<string> Flags { get; } = [];

    /// <summary>Podmienka stavu; prazdna = plati vzdy.</summary>
    public string Condition { get; set; } = "";

    /// <summary>Nazov podmienky.</summary>
    public string ConditionName { get; set; } = "";

    /// <summary>Stav ma tlacidlo.</summary>
    public bool HasButton { get; set; }

    /// <summary>Text tlacidla.</summary>
    public string ButtonText { get; set; } = "";

    /// <summary>Poradie tlacidla v klientovi ako text.</summary>
    public string ButtonPosition { get; set; } = "0";

    /// <summary>Stav ma automatiku.</summary>
    public bool HasAutomation { get; set; }

    /// <summary>Cas spustenia automatiky.</summary>
    public string Time { get; set; } = "";

    /// <summary>Dodatocna podmienka automatiky.</summary>
    public string AutomationCondition { get; set; } = "";

    /// <summary>Nazov podmienky automatiky.</summary>
    public string AutomationConditionName { get; set; } = "";

    /// <summary>Rezim automatiky.</summary>
    public string Mode { get; set; } = "";

    /// <summary>Interval opakovania.</summary>
    public string Repeat { get; set; } = "";

    /// <summary>Koncovy cas opakovania.</summary>
    public string End { get; set; } = "";

    /// <summary>Variant hlasenia 1 kratke, 2 dlhe; <see langword="null" /> = server zvoli sam.</summary>
    public int? Variant { get; set; }

    /// <summary>Priznaky automatiky.</summary>
    public List<string> AutomationFlags { get; } = [];

    /// <summary>Poradie tlacidla; <see langword="null" />, ak text nie je cislo.</summary>
    public int? Position => int.TryParse(ButtonPosition.Trim(), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var value) ? value : null;

    /// <summary>Koncept stavu.</summary>
    public static NodeDraft Of(StateNode node)
    {
        ArgumentNullException.ThrowIfNull(node);
        var draft = new NodeDraft
        {
            Original = node,
            Key = node.Key,
            Name = node.Name,
            Report = node.Report,
            Condition = EmbeddedCode.Display(node.Condition),
            ConditionName = node.ConditionName,
            HasButton = node.Button is not null,
            ButtonText = node.Button?.Text ?? "",
            ButtonPosition = (node.Button?.Position ?? 0).ToString(CultureInfo.InvariantCulture),
            HasAutomation = node.Automation is not null,
            Time = node.Automation?.Time ?? "",
            AutomationCondition = EmbeddedCode.Display(node.Automation?.Condition ?? ""),
            AutomationConditionName = node.Automation?.ConditionName ?? "",
            Mode = node.Automation?.Mode ?? "",
            Repeat = node.Automation?.Repeat ?? "",
            End = node.Automation?.End ?? "",
            Variant = node.Automation?.Variant
        };
        draft.Flags.AddRange(node.Flags);
        draft.AutomationFlags.AddRange(node.Automation?.Flags ?? []);
        return draft;
    }

    /// <summary>
    /// Stav suboru; bez zmeny vrati povodny stav (ten isty objekt).
    /// </summary>
    public StateNode To()
    {
        var original = Original;
        var node = new StateNode(Key)
        {
            Name = Name,
            Report = Report,
            Flags = Flags.SequenceEqual(original?.Flags ?? []) ? original?.Flags ?? [] : [.. Flags],
            Condition = EmbeddedCode.Store(Condition, original?.Condition ?? "", ConditionIndent, ConditionClosing),
            ConditionName = ConditionName,
            Button = HasButton ? Same(new StateButton(ButtonText, Position ?? 0), original?.Button) : null,
            Automation = HasAutomation ? Automation(original?.Automation) : null
        };
        return original is not null && Same(original, node) ? original : node;
    }

    private StateAutomation Automation(StateAutomation? original)
    {
        var automation = new StateAutomation(Time)
        {
            Condition = EmbeddedCode.Store(AutomationCondition, original?.Condition ?? "", ConditionIndent + "  ", ConditionClosing + "  "),
            ConditionName = AutomationConditionName,
            Mode = Mode,
            Repeat = Repeat,
            End = End,
            Variant = Variant,
            Flags = AutomationFlags.SequenceEqual(original?.Flags ?? []) ? original?.Flags ?? [] : [.. AutomationFlags]
        };
        return original is not null && Same(original, automation) ? original : automation;
    }

    private static StateButton Same(StateButton button, StateButton? original) => button == original ? original : button;

    private static bool Same(StateAutomation a, StateAutomation b) =>
        a.Time == b.Time && a.Condition == b.Condition && a.ConditionName == b.ConditionName && a.Mode == b.Mode && a.Repeat == b.Repeat && a.End == b.End &&
        a.Variant == b.Variant && a.Flags.SequenceEqual(b.Flags);

    /// <summary>Rovnost stavov vratane zoznamov priznakov.</summary>
    public static bool Same(StateNode a, StateNode b)
    {
        ArgumentNullException.ThrowIfNull(a);
        ArgumentNullException.ThrowIfNull(b);
        return a.Key == b.Key && a.Name == b.Name && a.Report == b.Report && a.Flags.SequenceEqual(b.Flags) && a.Condition == b.Condition &&
               a.ConditionName == b.ConditionName && a.Button == b.Button &&
               (a.Automation is null ? b.Automation is null : b.Automation is not null && Same(a.Automation, b.Automation));
    }
}
