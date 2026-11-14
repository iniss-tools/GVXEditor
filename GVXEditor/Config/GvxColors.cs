using GVXEditor.Properties;
using GVXEditor.UI.Settings;
using ToolsCore.XML;

namespace GVXEditor.Config;

/// <summary>
/// Polozky farebnej schemy GVXEditora mimo ovladacich prvkov a editora kodu: druhy vlakov v zozname vlakov.
/// </summary>
public static class GvxColors
{
    /// <summary>
    /// Kategoria Druh vlaku.
    /// </summary>
    public static ColorCategory TrainTypes { get; } = new("TrainType", () => Resources.Scheme_TrainType, () => new TrainTypeColorPreview());

    private const ColorParts TypeParts = ColorParts.Fore | ColorParts.Back | ColorParts.Bold;

    // predvolene farby ako stlpec typu v GVDEditore, v tmavej scheme svetlejsie odtiene kvoli citatelnosti
    private static ColorKey TrainType(string id, Func<string> name, Color light, Color dark, bool bold, Func<TrainTypeScheme, ColorSetting> setting) =>
        new("TrainType." + id, TrainTypes, name, new ColorValue(light, Color.Transparent, bold), new ColorValue(dark, Color.Transparent, bold),
            TypeParts, style => setting(((GVXEditorStyle)style).TrainTypeScheme), null, UiColors.Box.Id);

    /// <summary>
    /// Polozky v poradi stromu na stranke schemy.
    /// </summary>
    public static IReadOnlyList<ColorKey> Keys { get; } =
    [
        TrainType("Os", () => Resources.Scheme_TrainTypeOs, Color.Black, Color.White, false, scheme => scheme.Os),
        TrainType("R", () => Resources.Scheme_TrainTypeR, Color.Red, Color.FromArgb(255, 90, 90), true, scheme => scheme.R),
        TrainType("X", () => Resources.Scheme_TrainTypeX, Color.Green, Color.FromArgb(90, 200, 110), true, scheme => scheme.X),
        TrainType("Sl", () => Resources.Scheme_TrainTypeSl, Color.OrangeRed, Color.OrangeRed, false, scheme => scheme.Sl)
    ];

    /// <summary>
    /// Farby skupiny druhu vlaku v scheme.
    /// </summary>
    internal static ColorSetting For(TrainTypeScheme scheme, Domain.Views.TrainTypeGroup group) => group switch
    {
        Domain.Views.TrainTypeGroup.R => scheme.R,
        Domain.Views.TrainTypeGroup.X => scheme.X,
        Domain.Views.TrainTypeGroup.Sl => scheme.Sl,
        _ => scheme.Os
    };
}
