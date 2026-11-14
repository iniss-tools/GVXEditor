using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Stations.Texts;

/// <summary>
/// Odkial server berie stanice hlasenia vlaku v stanici.
/// </summary>
/// <param name="Records">stanice hlasenia pri vlakoch (StaHlasVl.xml stanice)</param>
/// <param name="ShortPatterns">vzory kratkeho hlasenia (StaHlasB.xml stanice)</param>
/// <param name="LongPatterns">vzory dlheho hlasenia (StaHlasC.xml stanice)</param>
/// <param name="Defaults">predvolene stanice hlasenia grafikonu (Default.Stations.xml)</param>
/// <param name="DefaultsFile">Default.Stations.xml v grafikone je - bez neho server pri dlhom hlaseni povie vsetky stanice</param>
internal sealed record ReportStationSource(
    IReadOnlyList<TrainReportStations> Records,
    IReadOnlyList<IReadOnlyList<ReportPatternStop>> ShortPatterns,
    IReadOnlyList<IReadOnlyList<ReportPatternStop>> LongPatterns,
    IReadOnlyList<DefaultStation> Defaults,
    bool DefaultsFile);
