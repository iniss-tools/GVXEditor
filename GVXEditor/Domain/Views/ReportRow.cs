using System.Globalization;
using GVXEditor.Domain.Code;
using GVXEditor.Properties;
using ToolsCore.Iniss2.Grafikon.Script;

namespace GVXEditor.Domain.Views;

/// <summary>
/// Riadok zoznamu okna Hlasenia: funkcia s oznacenim hlasenia, upravitelna v editore alebo rucny kod.
/// </summary>
/// <param name="result">funkcia skriptu</param>
/// <param name="references">odkazy z dat na nazov funkcie; <see langword="null" /> = ziadne</param>
internal sealed class ReportRow(ScriptReadResult result, ReportReferences? references)
{
    /// <summary>Funkcia skriptu.</summary>
    public ScriptReadResult Result { get; } = result;

    /// <summary>Odkazy z dat na funkciu.</summary>
    public ReportReferences? Refs { get; } = references;

    /// <summary>Nazov funkcie.</summary>
    public string Name => Result.Member.Name;

    /// <summary>Druh hlasenia podla prveho oznacenia hlasenia.</summary>
    public ScriptReportKind Kind => Result.Report?.Kind ?? FirstKind(Result.Member);

    /// <summary>Text druhu.</summary>
    public string KindText => ReportTexts.Kind(Kind);

    /// <summary>Popis z oznacenia.</summary>
    public string Description => Result.Report?.Description ?? Result.Member.Attributes.Select(attribute => attribute.Description).FirstOrDefault(text => text is not null) ?? "";

    /// <summary>Upravitelne v editore alebo rucny kod.</summary>
    public string StateText => Result.Report is null ? Resources.Reports_Manual : Resources.Reports_Editable;

    /// <summary>Pocet odkazov z dat; bez odkazov prazdne.</summary>
    public string References => Refs is { Total: > 0 } refs ? refs.Total.ToString(CultureInfo.CurrentCulture) : "";

    /// <summary>Napoveda riadka: odkazy z dat a dovod rucneho kodu.</summary>
    public string ToolTip => string.Join(Environment.NewLine,
        new[] { Refs?.Describe() ?? "", Result.Report is null ? ReportTexts.Manual(Result.Reason) : "" }.Where(text => text.Length > 0));

    private static ScriptReportKind FirstKind(LogTextMember member)
    {
        foreach (var attribute in member.Attributes)
        {
            if (ScriptReportKinds.TryParse(attribute.Name, out var kind))
                return kind;
        }

        return ScriptReportKind.Generic;
    }
}
