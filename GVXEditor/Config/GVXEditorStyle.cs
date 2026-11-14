using ToolsCore.CodeEditor;
using ToolsCore.XML;

namespace GVXEditor.Config;

/// <summary>
/// Farebna schema GVXEditora: ovladacie prvky, druhy vlakov v zozname vlakov a editor kodu VB.NET.
/// </summary>
public class GVXEditorStyle : Style, IVisualBasicStyle
{
    private static readonly IReadOnlyList<ColorKey> AllKeys = [.. UiColors.Keys, .. GvxColors.Keys, .. VisualBasicColors.Keys];

    /// <summary>
    /// Farby stlpca Druh v zozname vlakov.
    /// </summary>
    public TrainTypeScheme TrainTypeScheme { get; } = new();

    /// <summary>
    /// Farby editora kodu VB.NET (stavovy diagram, tabulky TabTab, skript hlaseni, podmienky tabul).
    /// </summary>
    public VisualBasicScheme VisualBasicScheme { get; } = new();

    /// <inheritdoc />
    public override IReadOnlyList<ColorKey> ColorKeys => AllKeys;

    /// <inheritdoc />
    protected override Style CreateEmpty() => new GVXEditorStyle();
}
