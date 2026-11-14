using GVXEditor.Domain.Views;
using ToolsCore.Iniss2.Grafikon.Script;

namespace GVXEditor.Domain.Code;

/// <summary>
/// Text hlasenia z textov zvukov banky po jazykoch - co by server poskladal pre obsluhu. Vypnute kroky a pauzy sa
/// vynechaju, zvuk, ktory banka nema, je kluc v hranatych zatvorkach (ako u servera), vstavana polozka a volanie funkcie
/// su v lomenych zatvorkach (text zavisi od vlaku).
/// </summary>
internal static class ReportPreview
{
    /// <summary>
    /// Texty hlasenia po jazykoch: jazyky zvukov v poradi hlasenia; zvuk bez jazyka, vstavana polozka a volanie hovoria
    /// vo vsetkych jazykoch banky. Jazyk, ktory banka nema, ma text <see langword="null" /> (server ho nevyhlasi).
    /// </summary>
    /// <param name="report">hlasenie</param>
    /// <param name="bank">zvukova banka instalacie</param>
    public static List<(string Language, string? Text)> Texts(ScriptReport report, SoundBank bank)
    {
        ArgumentNullException.ThrowIfNull(report);
        ArgumentNullException.ThrowIfNull(bank);
        var steps = report.Blocks
            .SelectMany(block => block.Steps.Where(step => step is SoundStep or BuiltinStep or CallStep).Select(step => (block.Language, Step: step)))
            .ToList();
        var languages = steps.Select(item => (item.Step as SoundStep)?.Language ?? item.Language).OfType<string>().ToList();
        if (steps.Any(item => item.Step is not SoundStep { Language: not null }))
            languages.AddRange(bank.Languages.Select(language => language.Key));
        if (report.OnlyLanguage is { } only)
            languages = [.. languages.Where(language => string.Equals(language, only, StringComparison.OrdinalIgnoreCase))];

        var result = new List<(string, string?)>();
        foreach (var language in languages.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var words = steps
                .Where(item => (item.Language is null || string.Equals(item.Language, language, StringComparison.OrdinalIgnoreCase))
                               && (item.Step is not SoundStep { Language: { } own } || string.Equals(own, language, StringComparison.OrdinalIgnoreCase)))
                .Select(item => Word(bank, language, item.Step))
                .ToList();
            if (words.Count == 0)
                continue;
            var known = bank.Languages.Count == 0 || bank.Languages.Any(item => string.Equals(item.Key, language, StringComparison.OrdinalIgnoreCase));
            result.Add((language, known ? string.Join(" ", words) : null));
        }

        return result;
    }

    // text zvuku v jazyku (zvuk, ktory banka nema, v hranatych zatvorkach); vstavana polozka a volanie v lomenych
    private static string Word(SoundBank bank, string language, ScriptStep step)
    {
        switch (step)
        {
            case BuiltinStep builtin:
                return "‹" + ReportTexts.Builtin(builtin.Method) + "›";
            case CallStep call:
                return "‹" + call.Function + "›";
        }

        var sound = (SoundStep)step;
        var path = sound.Language is null ? language + "/" + sound.Path : sound.Path;
        return bank.Find(path) is { Text.Length: > 0 } found ? found.Text : "[" + sound.Path + "]";
    }
}
