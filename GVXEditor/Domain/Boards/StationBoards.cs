using ToolsCore.Iniss2.Tables;

namespace GVXEditor.Domain.Boards;

/// <summary>
/// Tabule stanice, ktore server len cita: tabule a ich casti (Tables.xml) a katalogove predlohy (TCatalog.xml).
/// </summary>
/// <param name="Tables">tabule stanice v poradi suboru</param>
/// <param name="Catalog">katalogove predlohy v poradi suboru</param>
internal sealed record StationBoards(IReadOnlyList<TableDefinition> Tables, IReadOnlyList<CatalogTable> Catalog)
{
    /// <summary>
    /// Nazvy suborov v priecinku stanice.
    /// </summary>
    public static IReadOnlyList<string> FileNames { get; } = [TablesFile.FileName, TableCatalogFile.FileName];

    /// <summary>
    /// Nacita tabule stanice; chybajuci subor je prazdny zoznam.
    /// </summary>
    /// <param name="stationDir">priecinok stanice</param>
    public static StationBoards Load(string stationDir) => new(TablesFile.Read(stationDir), TableCatalogFile.Read(stationDir));

    /// <summary>
    /// Zapisy suborov, ktore sa od <paramref name="saved" /> zmenili (iny odkaz). Novy subor vznikne len s obsahom.
    /// </summary>
    public IEnumerable<FileWrite> Changes(string stationDir, StationBoards saved)
    {
        ArgumentNullException.ThrowIfNull(saved);
        if (!ReferenceEquals(Tables, saved.Tables))
            yield return new FileWrite(Path.Combine(stationDir, TablesFile.FileName), Tables.Count > 0, () => TablesFile.Write(stationDir, Tables));
        if (!ReferenceEquals(Catalog, saved.Catalog))
            yield return new FileWrite(Path.Combine(stationDir, TableCatalogFile.FileName), Catalog.Count > 0, () => TableCatalogFile.Write(stationDir, Catalog));
    }
}
