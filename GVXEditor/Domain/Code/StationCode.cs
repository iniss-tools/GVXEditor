using ToolsCore.Iniss2.Grafikon;
using ToolsCore.Iniss2.Tables;

namespace GVXEditor.Domain.Code;

/// <summary>
/// Kod stanice, ktory server len cita a preklada pri starte: stavovy diagram (statedgm.xml) a tabulky TabTab stanice
/// (TabTab.xml).
/// </summary>
/// <param name="Diagram">stavovy diagram; <see langword="null" />, ak subor chyba (server sa bez neho nespusti)</param>
/// <param name="TabTabs">tabulky TabTab stanice; prazdny zoznam, ak subor chyba</param>
internal sealed record StationCode(StateDiagram? Diagram, IReadOnlyList<TabTabTable> TabTabs)
{
    /// <summary>
    /// Nazvy suborov v priecinku stanice.
    /// </summary>
    public static IReadOnlyList<string> FileNames { get; } = [StateDiagramFile.FileName, TabTabXml.StationFileName];

    /// <summary>
    /// Nacita kod stanice.
    /// </summary>
    /// <param name="stationDir">priecinok stanice</param>
    public static StationCode Load(string stationDir) =>
        new(StateDiagramFile.Read(stationDir), TabTabXml.Read(Path.Combine(stationDir, TabTabXml.StationFileName)));

    /// <summary>
    /// Zapisy suborov, ktore sa od <paramref name="saved" /> zmenili (iny odkaz). Novy subor tabuliek vznikne len s obsahom.
    /// </summary>
    public IEnumerable<FileWrite> Changes(string stationDir, StationCode saved)
    {
        ArgumentNullException.ThrowIfNull(saved);
        if (Diagram is { } diagram && !ReferenceEquals(diagram, saved.Diagram))
            yield return new FileWrite(Path.Combine(stationDir, StateDiagramFile.FileName), true, () => StateDiagramFile.Write(stationDir, diagram));
        if (!ReferenceEquals(TabTabs, saved.TabTabs))
        {
            var file = Path.Combine(stationDir, TabTabXml.StationFileName);
            yield return new FileWrite(file, TabTabs.Count > 0, () => TabTabXml.Write(file, TabTabs));
        }
    }
}
