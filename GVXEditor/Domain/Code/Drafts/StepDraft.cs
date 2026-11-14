using System.Globalization;
using ToolsCore.Iniss2.Grafikon.Script;

namespace GVXEditor.Domain.Code.Drafts;

/// <summary>
/// Krok hlasenia v tabulke okna Hlasenia: zvuk, pauza alebo vypnuty riadok v bloku jazyka. Cisla su text, aby tabulka
/// prijala aj chybny zapis - ukaze ho kontrola.
/// </summary>
internal sealed class StepDraft
{
    /// <summary>Druh kroku v okne: zvuk.</summary>
    public const string SoundKind = "sound";

    /// <summary>Druh kroku v okne: pauza.</summary>
    public const string PauseKind = "pause";

    /// <summary>Druh kroku v okne: vypnuty (zakomentovany) riadok.</summary>
    public const string OffKind = "off";

    /// <summary>Druh kroku v okne: volanie inej funkcie skriptu.</summary>
    public const string CallKind = "call";

    /// <summary>Oddelovac vnorenych podmienok v texte <see cref="Condition" />.</summary>
    public const string ConditionSeparator = " » ";

    /// <summary>Predpona vetvy Else v texte <see cref="Condition" />.</summary>
    public const string ElsePrefix = "Else ";

    /// <summary>Jazyk bloku <c>If (Jazyk = "…")</c>; prazdny = krok bez podmienky.</summary>
    public string Block { get; set; } = "";

    /// <summary>
    /// Dalsie podmienky kroku (vo vnutri jazykoveho bloku) od vonkajsej, oddelene <see cref="ConditionSeparator" />; vetva
    /// Else ma predponu <see cref="ElsePrefix" />, napr. <c>VlakS.JeProjizdejici » Else DlouheHlaseni</c>. Prazdne = bez podmienky.
    /// </summary>
    public string Condition { get; set; } = "";

    /// <summary>
    /// Druh kroku: <see cref="SoundKind" />, <see cref="PauseKind" />, <see cref="OffKind" />, <see cref="CallKind" /> alebo
    /// nazov vstavanej metody servera (<see cref="ScriptBuiltins" />).
    /// </summary>
    public string Kind { get; set; } = SoundKind;

    /// <summary>
    /// Kluc zvuku, dlzka pauzy v ms, text vypnuteho riadka, argumenty vstavanej metody (napr. <c>"V4", 0, True</c>) alebo
    /// volanie funkcie (<c>CZ_kam_jede()</c>).
    /// </summary>
    public string Value { get; set; } = "";

    /// <summary>Vstavana metoda kroku; <see langword="null" />, ak krok nie je vstavana metoda.</summary>
    public ScriptBuiltin? Builtin => Kind is SoundKind or PauseKind or OffKind or CallKind ? null : ScriptBuiltins.Find(Kind);

    /// <summary>Pauza pred zvukom v ms; prazdna = 0.</summary>
    public string Before { get; set; } = "";

    /// <summary>Pauza po zvuku v ms; prazdna = 0.</summary>
    public string After { get; set; } = "";

    /// <summary>Poznamka na konci riadka zvuku alebo pauzy; prazdna = bez poznamky.</summary>
    public string Comment { get; set; } = "";

    /// <summary>
    /// Koncept kroku zo skriptu.
    /// </summary>
    /// <param name="block">jazyk bloku; <see langword="null" /> = bez podmienky</param>
    /// <param name="step">krok</param>
    /// <param name="conditions">dalsie podmienky bloku od vonkajsej</param>
    public static StepDraft Of(string? block, ScriptStep step, IEnumerable<ScriptCondition>? conditions = null)
    {
        ArgumentNullException.ThrowIfNull(step);
        var draft = new StepDraft { Block = block ?? "", Comment = step.Comment ?? "", Condition = ConditionText(conditions ?? []) };
        switch (step)
        {
            case SoundStep sound:
                draft.Value = sound.Path;
                draft.Before = Number(sound.PauseBefore);
                draft.After = Number(sound.PauseAfter);
                break;
            case PauseStep pause:
                draft.Kind = PauseKind;
                draft.Value = pause.Milliseconds.ToString(CultureInfo.InvariantCulture);
                break;
            case CommentStep comment:
                draft.Kind = OffKind;
                draft.Value = comment.Text;
                draft.Comment = "";
                break;
            case BuiltinStep builtin:
                draft.Kind = builtin.Method;
                draft.Value = string.Join(", ", builtin.Arguments);
                break;
            case CallStep call:
                draft.Kind = CallKind;
                draft.Value = ScriptReportWriter.StepText(call with { Comment = null });
                break;
        }

        return draft;
    }

    /// <summary>
    /// Argumenty novej vstavanej polozky: povinne parametre s hodnotou podla druhu (skupinu treba doplnit).
    /// </summary>
    /// <param name="builtin">vstavana metoda</param>
    /// <param name="stationsParameter">parameter so stanicami vzoru; <see langword="null" /> = stanice do smeru vlaku</param>
    public static string Template(ScriptBuiltin builtin, string? stationsParameter)
    {
        ArgumentNullException.ThrowIfNull(builtin);
        return string.Join(", ", builtin.Parameters.Where(parameter => !parameter.Optional).Select(parameter => parameter.Kind switch
        {
            ScriptParameterKind.Group or ScriptParameterKind.Text => "\"\"",
            ScriptParameterKind.Bool => "False",
            ScriptParameterKind.Int => "0",
            ScriptParameterKind.Time => "VlakS.CasOdj",
            ScriptParameterKind.Position => "VlakS.PoziceOdjSkut",
            ScriptParameterKind.Stations => stationsParameter ?? "VlakS.StaniceDoSmeru(ModifikatorHlaseni)",
            _ => "Nothing"
        }));
    }

    /// <summary>
    /// Krok skriptu; <see langword="null" />, ak cisla nie su platne alebo chyba kluc zvuku.
    /// </summary>
    public ScriptStep? Build()
    {
        var comment = Comment.Length == 0 ? null : Comment;
        switch (Kind)
        {
            case SoundKind:
                if (Value.Trim().Length == 0 || Pause(Before) is not { } before || Pause(After) is not { } after)
                    return null;
                return new SoundStep(Value.Trim(), before, after) { Comment = comment };
            case PauseKind:
                return Value.Trim().Length > 0 && Pause(Value) is { } milliseconds ? new PauseStep(milliseconds) { Comment = comment } : null;
            case OffKind:
                return new CommentStep(Value);
            case CallKind:
                return ScriptReportReader.ReadLine(Value) is CallStep call ? call with { Comment = comment } : null;
            default:
                return Builtin is { } builtin && ScriptReportReader.ReadLine(builtin.Method + "(" + Value + ")") is BuiltinStep step
                    ? step with { Comment = comment }
                    : null;
        }
    }

    /// <summary>
    /// Vypne krok (zakomentuje riadok) alebo zapne vypnuty riadok, ak je to zvuk alebo pauza. Vrati
    /// <see langword="false" />, ak sa krok prepnut neda (neplatny krok, vypnuty riadok nie je krok).
    /// </summary>
    public bool Toggle()
    {
        if (Kind == OffKind)
        {
            if (ScriptReportReader.ReadLine(Value) is not { } step || step is CommentStep)
                return false;
            var on = Of(Block.Length == 0 ? null : Block, step);
            (Kind, Value, Before, After, Comment) = (on.Kind, on.Value, on.Before, on.After, on.Comment);
            return true;
        }

        if (Build() is not { } built)
            return false;
        (Kind, Value, Before, After, Comment) = (OffKind, ScriptReportWriter.StepText(built), "", "", "");
        return true;
    }

    /// <summary>Kopia kroku.</summary>
    public StepDraft Copy() => (StepDraft)MemberwiseClone();

    /// <summary>
    /// Text podmienok pre bunku tabulky, napr. <c>VlakS.JeProjizdejici » Else DlouheHlaseni</c>.
    /// </summary>
    public static string ConditionText(IEnumerable<ScriptCondition> conditions)
    {
        ArgumentNullException.ThrowIfNull(conditions);
        return string.Join(ConditionSeparator, conditions.Select(condition => (condition.Else ? ElsePrefix : "") + condition.Expression));
    }

    /// <summary>
    /// Podmienky z textu bunky; <see langword="null" />, ak niektora podmienka je prazdna.
    /// </summary>
    public static List<ScriptCondition>? Conditions(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        if (text.Trim().Length == 0)
            return [];

        var result = new List<ScriptCondition>();
        foreach (var part in text.Split(ConditionSeparator.Trim(), StringSplitOptions.TrimEntries))
        {
            var isElse = part.StartsWith(ElsePrefix, StringComparison.Ordinal);
            var expression = (isElse ? part[ElsePrefix.Length..] : part).Trim();
            if (expression.Length == 0)
                return null;
            result.Add(new ScriptCondition(expression, isElse));
        }

        return result;
    }

    /// <summary>
    /// Pauza v ms z textu: prazdny text je 0; <see langword="null" />, ak text nie je nezaporne cele cislo.
    /// </summary>
    public static int? Pause(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        var value = text.Trim();
        if (value.Length == 0)
            return 0;
        return int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var number) ? number : null;
    }

    private static string Number(int value) => value == 0 ? "" : value.ToString(CultureInfo.InvariantCulture);
}
