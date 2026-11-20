using System.ComponentModel;
using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Mcp.Results;

/// <summary>Zaznamy vlaku v suboroch jednej stanice (podla cisla pri odchode zo stanice).</summary>
internal sealed record TrainStationRecords(
    [property: Description("Station number SR70.")] int Station,
    [property: Description("Station folder.")] string Directory,
    [property: Description("Train number the server looks the records up by (number when leaving the station).")] string Number,
    [property: Description("Tracks (Positions.xml); the first record valid on the day applies.")] IReadOnlyList<TrainPosition> Tracks,
    [property: Description("Supplementary announcements (Dodatky.xml); all valid ones apply.")] IReadOnlyList<TrainAddition> Additions,
    [property: Description("Train composition announcements (Razeni.xml).")] IReadOnlyList<TrainComposition> Compositions,
    [property: Description("Announcement languages (Foreign.xml); 1 = all languages of the sound bank.")] IReadOnlyList<TrainLanguages> Languages,
    [property: Description("Stations said in announcements (StaHlasVl.xml), SR70 numbers.")] IReadOnlyList<TrainReportStations> ReportStations,
    [property: Description("Board texts at the train (TTVlaky.xml).")] IReadOnlyList<TrainTextInfo> Texts,
    [property: Description("Days with train type MOs (MOS.xml).")] IReadOnlyList<MosTrain> Mos);
