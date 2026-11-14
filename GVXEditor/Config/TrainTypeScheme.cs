using ToolsCore.XML;

namespace GVXEditor.Config;

/// <summary>
/// Farby stlpca Druh v zozname vlakov podla skupiny druhu (polozky <see cref="GvxColors" />).
/// </summary>
public sealed class TrainTypeScheme
{
    /// <summary>Osobne a ostatne vlaky.</summary>
    public ColorSetting Os { get; } = new();

    /// <summary>Rychliky.</summary>
    public ColorSetting R { get; } = new();

    /// <summary>Vlaky vyssej kvality.</summary>
    public ColorSetting X { get; } = new();

    /// <summary>Sluzobne a nakladne vlaky.</summary>
    public ColorSetting Sl { get; } = new();
}
