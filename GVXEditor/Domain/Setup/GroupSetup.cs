using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Setup;

/// <summary>
/// Nastavenie grafikonu, ktore server len cita: nazvy stanic (GV.stanice.XML) a predvolene stanice hlasenia
/// (Default.Stations.xml).
/// </summary>
/// <param name="Names">nazvy stanic</param>
/// <param name="DefaultStations">predvolene stanice hlasenia do smeru</param>
internal sealed record GroupSetup(IReadOnlyList<GvStation> Names, IReadOnlyList<DefaultStation> DefaultStations)
{
    /// <summary>
    /// Nazvy suborov v priecinku grafikonu.
    /// </summary>
    public static IReadOnlyList<string> FileNames { get; } = [GvStationsFile.FileName, DefaultStationsFile.FileName];

    /// <summary>
    /// Nacita nastavenie grafikonu; chybajuci subor je prazdny zoznam.
    /// </summary>
    /// <param name="groupDir">priecinok grafikonu</param>
    public static GroupSetup Load(string groupDir) => new(GvStationsFile.Read(groupDir), DefaultStationsFile.Read(groupDir));

    /// <summary>
    /// Zapisy suborov, ktorych zoznam sa od <paramref name="saved" /> zmenil.
    /// </summary>
    public IEnumerable<FileWrite> Changes(string groupDir, GroupSetup saved)
    {
        ArgumentNullException.ThrowIfNull(saved);
        if (!ReferenceEquals(Names, saved.Names))
            yield return new FileWrite(Path.Combine(groupDir, GvStationsFile.FileName), Names.Count > 0, () => GvStationsFile.Write(groupDir, Names));
        if (!ReferenceEquals(DefaultStations, saved.DefaultStations))
        {
            yield return new FileWrite(Path.Combine(groupDir, DefaultStationsFile.FileName), DefaultStations.Count > 0,
                () => DefaultStationsFile.Write(groupDir, DefaultStations));
        }
    }
}
