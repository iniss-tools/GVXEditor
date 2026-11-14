using ToolsCore.Iniss2.Tables;

namespace GVXEditor.Domain.Stations.Texts;

/// <summary>
/// Sada textov vlakov, do ktorej sa da generovat: existujuca sada z TTVlaky.xml, TTTrasy.xml alebo TTStanice.xml stanice
/// (server pomenuje sadu vlakov podla sady, z ktorej text pochadza), alebo nova sada pre udaj katalogovej predlohy.
/// </summary>
/// <param name="Name">nazov sady</param>
/// <param name="Targets">udaje tabul, v ktorych sa texty sady prejavia</param>
/// <param name="Kind">co sa generuje podla obsahu udaja (<c>inf</c>); <see langword="null" />, ak obsah generovanie nepodporuje</param>
/// <param name="IsNew">sada este v ziadnom subore textov stanice nie je</param>
internal sealed record TextSetChoice(string Name, IReadOnlyList<TextTarget> Targets, GeneratedTextKind? Kind, bool IsNew)
{
    /// <summary>Priznak <c>FONET</c> sady (prenesie sa do novej sady vlakov).</summary>
    public bool Fonet { get; init; }

    /// <summary>Udaj, podla ktoreho sa urcil <see cref="Kind" /> (porovnanie s textom servera bez zaznamu).</summary>
    public TextTarget? Target { get; init; }
}
