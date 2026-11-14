using GVXEditor.Domain.Analysis;
using GVXEditor.Properties;
using GVXEditor.UI.Controls;

namespace GVXEditor.UI.Analysis;

/// <summary>
/// Riadok zoznamu zisteni.
/// </summary>
/// <param name="Finding">zistenie</param>
internal sealed record FindingRow(Finding Finding)
{
    /// <summary>Zavaznost slovom (popis ikony).</summary>
    public string Severity => Finding.Severity switch
    {
        FindingSeverity.Error => Resources.Severity_Error,
        FindingSeverity.Warning => Resources.Severity_Warning,
        _ => Resources.Severity_Info
    };

    /// <summary>Stavova ikona zavaznosti z banky ikon.</summary>
    public string SeverityIcon => ProblemList.IconName(Finding.Severity);

    /// <summary>Miesto zistenia.</summary>
    public string Place => Finding.Place;

    /// <summary>Popis.</summary>
    public string Message => Finding.Message;
}
