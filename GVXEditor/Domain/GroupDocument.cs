using GVXEditor.Domain.Boards;
using GVXEditor.Domain.Code;
using GVXEditor.Domain.Setup;
using GVXEditor.Domain.Stations;
using ToolsCore.Iniss2.Grafikon;
using ToolsCore.Iniss2.Tables;

namespace GVXEditor.Domain;

/// <summary>
/// Grafikon instalacie: vlaky, subory jeho stanic a vyluky a odklony. Zmena vytvori novy zaznam, nezmenene casti ostanu tie
/// iste objekty - podla odkazov sa pri ulozeni zisti, ktore subory treba zapisat.
/// </summary>
/// <param name="Group">grafikon z DirList.xml</param>
/// <param name="Timetable">vlaky zo suboru GV.XML tak, ako su v subore; <see langword="null" />, ak sa subor neda nacitat</param>
/// <param name="Error">preco sa subor neda nacitat; <see langword="null" /> = nacitany</param>
internal sealed record GroupDocument(DirListGroup Group, GvFile? Timetable, string? Error)
{
    /// <summary>Stanice grafikonu v poradi DirList.xml.</summary>
    public IReadOnlyList<StationDocument> Stations { get; init; } = [];

    /// <summary>Vyluky.xml; <see langword="null" />, ak sa subor neda nacitat.</summary>
    public IReadOnlyList<TrainLockout>? Lockouts { get; init; } = [];

    /// <summary>Odklony.xml; <see langword="null" />, ak sa subor neda nacitat.</summary>
    public IReadOnlyList<TrainDiversion>? Diversions { get; init; } = [];

    /// <summary>Preco sa Vyluky.xml alebo Odklony.xml neda nacitat.</summary>
    public string? NoticesError { get; init; }

    /// <summary>Nazvy stanic a predvolene stanice hlasenia; <see langword="null" />, ak sa nedaju nacitat.</summary>
    public GroupSetup? Setup { get; init; }

    /// <summary>Preco sa nastavenie grafikonu neda nacitat.</summary>
    public string? SetupError { get; init; }

    /// <summary>Spolocne tabulky TabTab (TabTabGlobal.xml); <see langword="null" />, ak sa nedaju nacitat.</summary>
    public IReadOnlyList<TabTabTable>? GlobalTabTabs { get; init; } = [];

    /// <summary>Preco sa TabTabGlobal.xml neda nacitat.</summary>
    public string? GlobalTabTabsError { get; init; }

    /// <summary>
    /// Cisla stanic grafikonu z DirList.xml - v nich vlak riadi server (kluc <c>Z</c>).
    /// </summary>
    public IReadOnlySet<int> InterestStations => Group.Items.Select(item => item.Station).ToHashSet();

    /// <summary>
    /// Zapisy suborov grafikonu a jeho stanic zmenenych od <paramref name="saved" />.
    /// </summary>
    /// <param name="dataDir">priecinok dat</param>
    /// <param name="saved">grafikon pri nacitani alebo poslednom ulozeni</param>
    public IEnumerable<FileWrite> Changes(string dataDir, GroupDocument saved)
    {
        ArgumentNullException.ThrowIfNull(saved);
        var groupDir = Path.Combine(dataDir, Group.Name);

        if (Timetable is { } timetable && !ReferenceEquals(timetable, saved.Timetable))
            yield return new FileWrite(Path.Combine(groupDir, GvFile.FileName), true, () => timetable.Write(groupDir));
        if (Lockouts is { } lockouts && !ReferenceEquals(lockouts, saved.Lockouts))
            yield return new FileWrite(Path.Combine(groupDir, VylukyFile.FileName), lockouts.Count > 0, () => VylukyFile.Write(groupDir, lockouts));
        if (Diversions is { } diversions && !ReferenceEquals(diversions, saved.Diversions))
            yield return new FileWrite(Path.Combine(groupDir, OdklonyFile.FileName), diversions.Count > 0, () => OdklonyFile.Write(groupDir, diversions));

        if (Setup is { } setup && saved.Setup is { } savedSetup && !ReferenceEquals(setup, savedSetup))
        {
            foreach (var write in setup.Changes(groupDir, savedSetup))
                yield return write;
        }

        if (GlobalTabTabs is { } tabTabs && !ReferenceEquals(tabTabs, saved.GlobalTabTabs))
        {
            var file = Path.Combine(groupDir, TabTabXml.GlobalFileName);
            yield return new FileWrite(file, tabTabs.Count > 0, () => TabTabXml.Write(file, tabTabs));
        }

        for (var i = 0; i < Stations.Count && i < saved.Stations.Count; i++)
        {
            var stationDir = Path.Combine(groupDir, Stations[i].Station.Directory);
            if (Stations[i].Files is { } files && saved.Stations[i].Files is { } before && !ReferenceEquals(files, before))
            {
                foreach (var write in files.Changes(stationDir, before))
                    yield return write;
            }

            if (Stations[i].Setup is { } stationSetup && saved.Stations[i].Setup is { } savedStationSetup && !ReferenceEquals(stationSetup, savedStationSetup))
            {
                foreach (var write in stationSetup.Changes(stationDir, savedStationSetup))
                    yield return write;
            }

            if (Stations[i].Code is { } code && saved.Stations[i].Code is { } savedCode && !ReferenceEquals(code, savedCode))
            {
                foreach (var write in code.Changes(stationDir, savedCode))
                    yield return write;
            }

            if (Stations[i].Boards is { } boards && saved.Stations[i].Boards is { } savedBoards && !ReferenceEquals(boards, savedBoards))
            {
                foreach (var write in boards.Changes(stationDir, savedBoards))
                    yield return write;
            }
        }
    }

    /// <summary>
    /// Grafikon so zmenenymi subormi stanice.
    /// </summary>
    /// <param name="index">poradie stanice</param>
    /// <param name="files">nove subory stanice</param>
    public GroupDocument WithStation(int index, StationFiles files)
    {
        var stations = Stations.ToArray();
        stations[index] = stations[index] with { Files = files };
        return this with { Stations = stations };
    }

    /// <summary>
    /// Grafikon so zmenenym nastavenim stanice.
    /// </summary>
    /// <param name="index">poradie stanice</param>
    /// <param name="setup">nove nastavenie stanice</param>
    public GroupDocument WithStationSetup(int index, StationSetup setup)
    {
        var stations = Stations.ToArray();
        stations[index] = stations[index] with { Setup = setup };
        return this with { Stations = stations };
    }

    /// <summary>
    /// Grafikon so zmenenym kodom stanice.
    /// </summary>
    /// <param name="index">poradie stanice</param>
    /// <param name="code">novy kod stanice</param>
    public GroupDocument WithStationCode(int index, StationCode code)
    {
        var stations = Stations.ToArray();
        stations[index] = stations[index] with { Code = code };
        return this with { Stations = stations };
    }

    /// <summary>
    /// Grafikon so zmenenymi tabulami stanice.
    /// </summary>
    /// <param name="index">poradie stanice</param>
    /// <param name="boards">nove tabule a predlohy stanice</param>
    public GroupDocument WithStationBoards(int index, StationBoards boards)
    {
        var stations = Stations.ToArray();
        stations[index] = stations[index] with { Boards = boards };
        return this with { Stations = stations };
    }
}
