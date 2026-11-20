using System.ComponentModel;
using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Mcp.Results;

/// <summary>Vysledok get_train.</summary>
internal sealed record TrainDetail(
    [property: Description("Timetable group.")] string Group,
    [property: Description("Train index in GV.XML.")] int Index,
    [property: Description("All train numbers.")] string Numbers,
    [property: Description("First day of the validity period.")] DateOnly From,
    [property: Description("Last day of the validity period; null = no end.")] DateOnly? To,
    [property: Description("Variants in order; on a day the first variant running that day applies.")] IReadOnlyList<VariantDetail> Variants,
    [property: Description("Records of the train in the station files of the group.")] IReadOnlyList<TrainStationRecords> Stations,
    [property: Description("Train number used for lockouts in the group.")] string LockoutNumber,
    [property: Description("Lockouts of the train (Vyluky.xml); Key is a LogText function or an operator report number.")] IReadOnlyList<TrainLockout> Lockouts,
    [property: Description("Diversions of the train (Odklony.xml).")] IReadOnlyList<TrainDiversion> Diversions);
