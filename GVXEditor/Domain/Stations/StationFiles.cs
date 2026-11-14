using ToolsCore.Iniss2.Grafikon;
using ToolsCore.Iniss2.Tables;

namespace GVXEditor.Domain.Stations;

/// <summary>
/// Subory stanice, ktore server pri uprave vlaku prepisuje: kolaje, dodatky, radenie, jazyky, stanice v hlaseni, texty tabul
/// a MOS. Zaznamy su v poradi suboru - server pri hladani berie prvy vyhovujuci (dodatky vsetky).
/// </summary>
/// <param name="Positions">Positions.xml</param>
/// <param name="Additions">Dodatky.xml</param>
/// <param name="Compositions">Razeni.xml</param>
/// <param name="Languages">Foreign.xml</param>
/// <param name="ReportStations">StaHlasVl.xml</param>
/// <param name="TextSets">TTVlaky.xml</param>
/// <param name="Mos">MOS.xml</param>
internal sealed record StationFiles(
    IReadOnlyList<TrainPosition> Positions,
    IReadOnlyList<TrainAddition> Additions,
    IReadOnlyList<TrainComposition> Compositions,
    IReadOnlyList<TrainLanguages> Languages,
    IReadOnlyList<TrainReportStations> ReportStations,
    IReadOnlyList<TrainTextSet> TextSets,
    IReadOnlyList<MosTrain> Mos)
{
    /// <summary>
    /// Nazvy suborov v priecinku stanice v poradi zloziek.
    /// </summary>
    public static IReadOnlyList<string> FileNames { get; } =
    [
        PositionsFile.FileName, DodatkyFile.FileName, RazeniFile.FileName, ForeignFile.FileName, TrainReportStationsFile.FileName,
        TrainTextsFile.FileName, MosFile.FileName
    ];

    /// <summary>
    /// Nacita subory stanice; chybajuci subor je prazdny zoznam.
    /// </summary>
    /// <param name="stationDir">priecinok stanice</param>
    public static StationFiles Load(string stationDir) => new(
        PositionsFile.Read(stationDir),
        DodatkyFile.Read(stationDir),
        RazeniFile.Read(stationDir),
        ForeignFile.Read(stationDir),
        TrainReportStationsFile.Read(stationDir),
        TrainTextsFile.Read(stationDir),
        MosFile.Read(stationDir));

    /// <summary>
    /// Zapisy suborov, ktorych zoznam sa od <paramref name="saved" /> zmenil (iny odkaz). Novy subor vznikne len s obsahom.
    /// </summary>
    /// <param name="stationDir">priecinok stanice</param>
    /// <param name="saved">stav pri nacitani alebo poslednom ulozeni</param>
    public IEnumerable<FileWrite> Changes(string stationDir, StationFiles saved)
    {
        ArgumentNullException.ThrowIfNull(saved);
        string File(string name) => Path.Combine(stationDir, name);

        if (!ReferenceEquals(Positions, saved.Positions))
            yield return new FileWrite(File(PositionsFile.FileName), Positions.Count > 0, () => PositionsFile.Write(stationDir, Positions));
        if (!ReferenceEquals(Additions, saved.Additions))
            yield return new FileWrite(File(DodatkyFile.FileName), Additions.Count > 0, () => DodatkyFile.Write(stationDir, Additions));
        if (!ReferenceEquals(Compositions, saved.Compositions))
            yield return new FileWrite(File(RazeniFile.FileName), Compositions.Count > 0, () => RazeniFile.Write(stationDir, Compositions));
        if (!ReferenceEquals(Languages, saved.Languages))
            yield return new FileWrite(File(ForeignFile.FileName), Languages.Count > 0, () => ForeignFile.Write(stationDir, Languages));
        if (!ReferenceEquals(ReportStations, saved.ReportStations))
            yield return new FileWrite(File(TrainReportStationsFile.FileName), ReportStations.Count > 0,
                () => TrainReportStationsFile.Write(stationDir, ReportStations));
        if (!ReferenceEquals(TextSets, saved.TextSets))
            yield return new FileWrite(File(TrainTextsFile.FileName), TextSets.Count > 0, () => TrainTextsFile.Write(stationDir, TextSets));
        if (!ReferenceEquals(Mos, saved.Mos))
            yield return new FileWrite(File(MosFile.FileName), Mos.Count > 0, () => MosFile.Write(stationDir, Mos));
    }
}
