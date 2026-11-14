using System.Xml.Serialization;
using ToolsCore.XML;

namespace GVXEditor.Config;

/// <summary>
/// Nastavenia programu GVXEditor (config.xml).
/// </summary>
[XmlRoot("CONFIG")]
public record GVXEditorConfig : ConfigBase
{
    /// <summary>
    /// Navod k nastaveniam programu v dokumentacii.
    /// </summary>
    public const string LinkAppSettings = "https://iniss-tools.fvat.sk/docs/gvxeditor/nastavenia-programu/";

    /// <inheritdoc />
    public override string LinkAppSettingsGuide => LinkAppSettings;

    /// <summary>
    /// Predvolene nastavenia.
    /// </summary>
    public GVXEditorConfig()
    {
    }

    /// <summary>
    /// Kopia nastaveni (okno Nastavenia programu) - skratky a stlpce sa nesmu zdielat s povodnymi.
    /// </summary>
    protected GVXEditorConfig(GVXEditorConfig original) : base(original)
    {
    }
}
