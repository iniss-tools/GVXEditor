using ToolsCore.Iniss2.Grafikon;
using ToolsCore.Iniss2.Tables;

namespace GVXEditor.Domain.Setup;

/// <summary>
/// Nastavenie stanice, ktore server len cita: kolaje (PositionsDef.xml), typy hlaseni (ReportKeys.xml), vzory stanic
/// v hlaseni (StaHlasB.xml a StaHlasC.xml) a texty tabul podla trasy (TTTrasy.xml) a stanice (TTStanice.xml).
/// </summary>
/// <param name="Tracks">kolaje v poradi pre obsluhu</param>
/// <param name="ReportKeys">typy hlaseni</param>
/// <param name="ShortPatterns">vzory stanic pre kratke hlasenie</param>
/// <param name="LongPatterns">vzory stanic pre dlhe hlasenie</param>
/// <param name="RouteTexts">sady textov podla trasy</param>
/// <param name="StationTexts">sady textov podla stanice</param>
internal sealed record StationSetup(
    IReadOnlyList<TrackDefinition> Tracks,
    IReadOnlyList<ReportKey> ReportKeys,
    IReadOnlyList<IReadOnlyList<ReportPatternStop>> ShortPatterns,
    IReadOnlyList<IReadOnlyList<ReportPatternStop>> LongPatterns,
    IReadOnlyList<RouteTextSet> RouteTexts,
    IReadOnlyList<StationTextSet> StationTexts)
{
    /// <summary>
    /// Nazvy suborov v priecinku stanice v poradi zloziek.
    /// </summary>
    public static IReadOnlyList<string> FileNames { get; } =
    [
        PositionsDefFile.FileName, ReportKeysFile.FileName, ReportPatternsFile.ShortFileName, ReportPatternsFile.LongFileName, RouteTextsFile.FileName,
        StationTextsFile.FileName
    ];

    /// <summary>
    /// Nacita nastavenie stanice; chybajuci subor je prazdny zoznam.
    /// </summary>
    /// <param name="stationDir">priecinok stanice</param>
    public static StationSetup Load(string stationDir) => new(
        File.Exists(Path.Combine(stationDir, PositionsDefFile.FileName)) ? PositionsDefFile.Read(stationDir) : [],
        ReportKeysFile.Read(stationDir),
        ReportPatternsFile.Read(stationDir, ReportPatternsFile.ShortFileName),
        ReportPatternsFile.Read(stationDir, ReportPatternsFile.LongFileName),
        RouteTextsFile.Read(stationDir),
        StationTextsFile.Read(stationDir));

    /// <summary>
    /// Zapisy suborov, ktorych zoznam sa od <paramref name="saved" /> zmenil (iny odkaz). Novy subor vznikne len s obsahom.
    /// </summary>
    public IEnumerable<FileWrite> Changes(string stationDir, StationSetup saved)
    {
        ArgumentNullException.ThrowIfNull(saved);
        string File(string name) => Path.Combine(stationDir, name);

        if (!ReferenceEquals(Tracks, saved.Tracks))
            yield return new FileWrite(File(PositionsDefFile.FileName), Tracks.Count > 0, () => PositionsDefFile.Write(stationDir, Tracks));
        if (!ReferenceEquals(ReportKeys, saved.ReportKeys))
            yield return new FileWrite(File(ReportKeysFile.FileName), ReportKeys.Count > 0, () => ReportKeysFile.Write(stationDir, ReportKeys));
        if (!ReferenceEquals(ShortPatterns, saved.ShortPatterns))
            yield return new FileWrite(File(ReportPatternsFile.ShortFileName), ShortPatterns.Count > 0,
                () => ReportPatternsFile.Write(stationDir, ReportPatternsFile.ShortFileName, ShortPatterns));
        if (!ReferenceEquals(LongPatterns, saved.LongPatterns))
            yield return new FileWrite(File(ReportPatternsFile.LongFileName), LongPatterns.Count > 0,
                () => ReportPatternsFile.Write(stationDir, ReportPatternsFile.LongFileName, LongPatterns));
        if (!ReferenceEquals(RouteTexts, saved.RouteTexts))
            yield return new FileWrite(File(RouteTextsFile.FileName), RouteTexts.Count > 0, () => RouteTextsFile.Write(stationDir, RouteTexts));
        if (!ReferenceEquals(StationTexts, saved.StationTexts))
            yield return new FileWrite(File(StationTextsFile.FileName), StationTexts.Count > 0, () => StationTextsFile.Write(stationDir, StationTexts));
    }
}
