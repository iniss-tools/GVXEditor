using System.Globalization;
using System.Text.RegularExpressions;
using GVXEditor.Domain.Code.Drafts;
using GVXEditor.Domain.Views;
using GVXEditor.Properties;
using ToolsCore.CodeEditor;
using ToolsCore.Iniss.Tools;
using ToolsCore.Iniss2.Grafikon.Script;

namespace GVXEditor.Domain.Rules;

/// <summary>
/// Kontrola hlasenia v okne Hlasenia. Chyba = kod by sa neprelozil alebo by sa nedal zapisat (hlasenie sa do skriptu
/// nezapise), upozornenie = server hlasenie prelozi, ale cast z neho nevyhlasi.
/// </summary>
internal static partial class ReportRules
{
    private static readonly HashSet<string> Keywords = new(VisualBasicSyntax.Keywords.Split(' ', StringSplitOptions.RemoveEmptyEntries),
        StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Zistenia hlasenia.
    /// </summary>
    /// <param name="draft">koncept hlasenia</param>
    /// <param name="nameTaken">nazov ma ina funkcia skriptu</param>
    /// <param name="isFunction">meno je funkcia skriptu (volanie, <c>AddressOf</c>)</param>
    /// <param name="bank">zvukova banka; <see langword="null" /> = zvuky sa v banke neoveruju</param>
    public static List<SetupProblem> Check(ReportDraft draft, Func<string, bool> nameTaken, Func<string, bool> isFunction, SoundBank? bank)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(nameTaken);
        ArgumentNullException.ThrowIfNull(isFunction);
        var problems = new List<SetupProblem>();
        var name = draft.Name.Trim();
        var where = Format(Resources.Reports_Where, name);

        if (name.Length == 0)
            problems.Add(new SetupProblem(true, Format(Resources.Reports_NoName, where)));
        else if (!IdentifierPattern().IsMatch(name))
            problems.Add(new SetupProblem(true, Format(Resources.Reports_BadName, where, name)));
        else if (Keywords.Contains(name))
            problems.Add(new SetupProblem(true, Format(Resources.Reports_KeywordName, where, name)));
        else if (nameTaken(name))
            problems.Add(new SetupProblem(true, Format(Resources.Reports_DuplicateName, where, name)));

        if (Unencodable(name + draft.Description + string.Concat(draft.Steps.Select(step => step.Value + step.Comment))) is { } bad)
            problems.Add(new SetupProblem(true, Format(Resources.Reports_Unencodable, where, bad)));
        if (draft.Description.Contains('\n', StringComparison.Ordinal) || draft.Description.Contains('\r', StringComparison.Ordinal))
            problems.Add(new SetupProblem(true, Format(Resources.Reports_LineBreak, where)));
        else if (draft.Description.Trim().Length == 0)
            problems.Add(new SetupProblem(false, Format(Resources.Reports_NoDescription, where)));

        if (ScriptReportKinds.HasStationLimit(draft.Kind))
        {
            foreach (var stations in new[] { draft.ForStations, draft.NotStations })
            {
                if (stations.Trim().Length > 0 && !StationsPattern().IsMatch(stations.Trim()))
                    problems.Add(new SetupProblem(false, Format(Resources.Reports_BadStations, where, stations.Trim())));
            }
        }

        if (ReportDraft.IsExclusion(draft.Kind))
        {
            if (!ExclusionTypePattern().IsMatch(draft.ExclusionType.Trim()))
                problems.Add(new SetupProblem(true, Format(Resources.Reports_NoExclusionType, where)));
            if (draft.Kind == ScriptReportKind.Exclusion && draft.ExclusionStations.Trim().Length > 0 && !StationsPattern().IsMatch(draft.ExclusionStations.Trim()))
                problems.Add(new SetupProblem(true, Format(Resources.Reports_BadExclusionStations, where)));
        }

        var only = draft.OnlyLanguage.Trim();
        if (only.Length > 0 && !LanguagePattern().IsMatch(only))
            problems.Add(new SetupProblem(true, Format(Resources.Reports_BadLanguage, where, only)));
        if (Unencodable(draft.Guards + string.Concat(draft.Steps.Select(step => step.Condition))) is { } badCode)
            problems.Add(new SetupProblem(true, Format(Resources.Reports_Unencodable, where, badCode)));

        for (var i = 0; i < draft.Steps.Count; i++)
            CheckStep(draft.Steps[i], Format(Resources.Reports_StepWhere, where, i + 1), only, isFunction, bank, problems);

        return problems;
    }

    private static void CheckStep(StepDraft step, string where, string only, Func<string, bool> isFunction, SoundBank? bank, List<SetupProblem> problems)
    {
        var block = step.Block.Trim();
        if (StepDraft.Conditions(step.Condition) is null)
            problems.Add(new SetupProblem(true, Format(Resources.Reports_BadCondition, where, step.Condition)));
        if (block.Length > 0 && !LanguagePattern().IsMatch(block))
            problems.Add(new SetupProblem(true, Format(Resources.Reports_BadLanguage, where, block)));
        else if (block.Length > 0 && only.Length > 0 && !string.Equals(block, only, StringComparison.OrdinalIgnoreCase) && step.Kind != StepDraft.OffKind)
            problems.Add(new SetupProblem(false, Format(Resources.Reports_LanguageNeverSpoken, where, block, only)));

        switch (step.Kind)
        {
            case StepDraft.SoundKind:
                CheckSound(step, where, block, bank, problems);
                break;
            case StepDraft.PauseKind:
                if (step.Value.Trim().Length == 0 || StepDraft.Pause(step.Value) is null)
                    problems.Add(new SetupProblem(true, Format(Resources.Reports_BadPause, where, step.Value)));
                break;
            case StepDraft.OffKind:
                break;
            case StepDraft.CallKind:
                if (step.Build() is not CallStep call)
                    problems.Add(new SetupProblem(true, Format(Resources.Reports_BadCall, where, step.Value)));
                else
                    CheckFunctions(call.Function, call.Arguments, where, isFunction, problems);
                break;
            default:
                CheckBuiltin(step, where, block, isFunction, bank, problems);
                break;
        }
    }

    // vstavana metoda: argumenty, ktore model opise, skupiny zvukov v banke a funkcie AddressOf
    private static void CheckBuiltin(StepDraft step, string where, string block, Func<string, bool> isFunction, SoundBank? bank, List<SetupProblem> problems)
    {
        if (step.Builtin is not { } builtin || step.Build() is not BuiltinStep built)
        {
            problems.Add(new SetupProblem(true, Format(Resources.Reports_BadBuiltin, where, step.Value, step.Kind,
                step.Builtin is { } known ? ReportTexts.Parameters(known) : "")));
            return;
        }

        CheckFunctions(null, built.Arguments, where, isFunction, problems);
        if (bank is null || bank.Languages.Count == 0)
            return;
        var language = bank.Sounds(block.Length > 0 ? block : bank.Languages[0].Key);
        if (language is null)
            return;
        for (var i = 0; i < builtin.Parameters.Count; i++)
        {
            if (builtin.Parameters[i].Kind != ScriptParameterKind.Group || Literal(builtin.Argument(built.Arguments, i)) is not { } group)
                continue;
            if (!language.Groups.Any(item => string.Equals(item.Key, group, StringComparison.OrdinalIgnoreCase)))
                problems.Add(new SetupProblem(false, Format(Resources.Reports_UnknownGroup, where, group)));
        }
    }

    // volana funkcia a funkcie v argumentoch AddressOf musia byt v skripte
    private static void CheckFunctions(string? function, IEnumerable<string> arguments, string where, Func<string, bool> isFunction, List<SetupProblem> problems)
    {
        var names = arguments.Select(argument => AddressOfPattern().Match(argument.Trim())).Where(match => match.Success)
            .Select(match => match.Groups["name"].Value);
        foreach (var name in function is null ? names : names.Prepend(function))
        {
            if (!isFunction(name))
                problems.Add(new SetupProblem(true, Format(Resources.Reports_UnknownFunction, where, name)));
        }
    }

    // hodnota textoveho argumentu ("…" so zdvojenymi uvodzovkami); iny argument null
    private static string? Literal(string? argument)
    {
        var text = argument?.Trim();
        return text is { Length: >= 2 } && text[0] == '"' && text[^1] == '"' ? text[1..^1].Replace("\"\"", "\"", StringComparison.Ordinal) : null;
    }

    private static void CheckSound(StepDraft step, string where, string block, SoundBank? bank, List<SetupProblem> problems)
    {
        foreach (var pause in new[] { step.Before, step.After })
        {
            if (StepDraft.Pause(pause) is null)
                problems.Add(new SetupProblem(true, Format(Resources.Reports_BadPause, where, pause)));
        }

        var path = step.Value.Trim();
        if (path.Length == 0)
        {
            problems.Add(new SetupProblem(true, Format(Resources.Reports_NoSound, where)));
            return;
        }

        var parts = path.Split('/');
        if (parts.Length is < 2 or > 3 || parts.Any(part => part.Length == 0 || part.Any(c => char.IsWhiteSpace(c) || c == '"')))
        {
            problems.Add(new SetupProblem(true, Format(Resources.Reports_BadSound, where, path)));
            return;
        }

        var language = parts.Length == 3 ? parts[0] : null;
        if (language is not null && block.Length > 0 && !string.Equals(language, block, StringComparison.OrdinalIgnoreCase))
            problems.Add(new SetupProblem(false, Format(Resources.Reports_OtherLanguage, where, language, block)));

        // jazyk, ktory banka nema, server neoveruje; zvuk bez jazyka hra v jazyku hlasenia - overi sa v jazyku bloku alebo v prvom jazyku banky
        if (bank is null || bank.Languages.Count == 0)
            return;
        var checkedLanguage = language ?? (block.Length > 0 ? block : bank.Languages[0].Key);
        if (bank.Sounds(checkedLanguage) is null)
            return;
        var full = language is null ? checkedLanguage + "/" + path : path;
        if (bank.Find(full) is null)
            problems.Add(new SetupProblem(false, Format(Resources.Reports_UnknownSound, where, full)));
    }

    /// <summary>
    /// Znaky textu, ktore CP1250 nema (najviac pat); <see langword="null" />, ak sa text da zapisat.
    /// </summary>
    public static string? Unencodable(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        var bad = text.Where(c => c > 127 && Encodings.Win1250.GetString(Encodings.Win1250.GetBytes(c.ToString())) != c.ToString()).Distinct().Take(5).ToList();
        return bad.Count == 0 ? null : string.Concat(bad);
    }

    private static string Format(string format, params object[] args) => string.Format(CultureInfo.CurrentCulture, format, args);

    [GeneratedRegex(@"^(?:\p{L}[\p{L}\p{N}_]*|_[\p{L}\p{N}_]+)$", RegexOptions.CultureInvariant)]
    private static partial Regex IdentifierPattern();

    [GeneratedRegex(@"^\d+(?:\s*,\s*\d+)*$", RegexOptions.CultureInvariant)]
    private static partial Regex StationsPattern();

    [GeneratedRegex(@"^TYPVYLUKY\.(?:NONE|TRATOVA|PRIJEZD|ODJEZD|PRIJODJ|AUTO)$", RegexOptions.CultureInvariant)]
    private static partial Regex ExclusionTypePattern();

    [GeneratedRegex(@"^[\p{L}\p{N}_]+$", RegexOptions.CultureInvariant)]
    private static partial Regex LanguagePattern();

    [GeneratedRegex(@"^(?:[\p{L}_][\p{L}\p{N}_]*\s*:=\s*)?AddressOf\s+(?<name>[\p{L}_][\p{L}\p{N}_]*)$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex AddressOfPattern();
}
