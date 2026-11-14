using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Setup.Drafts;

/// <summary>Typ hlasenia stanice (ReportKeys.xml).</summary>
internal sealed class ReportKeyDraft
{
    /// <summary>Nazov hlasenia ako v LogText a stavovom diagrame.</summary>
    public string Name { get; set; } = "";

    /// <summary>Pismeno, podla ktoreho sa vyberaju dodatky a radenie.</summary>
    public string Key { get; set; } = "";

    /// <summary>Kategorie vlakov V, P, K oddelene medzerou.</summary>
    public string Categories { get; set; } = "";

    /// <summary>Koncept typu hlasenia.</summary>
    public static ReportKeyDraft Of(ReportKey key)
    {
        ArgumentNullException.ThrowIfNull(key);
        return new ReportKeyDraft { Name = key.Name, Key = key.Key, Categories = key.Categories };
    }

    /// <summary>Typ hlasenia suboru.</summary>
    public ReportKey To() => new(Name, Key, Categories);
}
