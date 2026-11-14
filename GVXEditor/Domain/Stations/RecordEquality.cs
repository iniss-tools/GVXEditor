using ToolsCore.Iniss2.Grafikon;
using ToolsCore.Iniss2.Tables;

namespace GVXEditor.Domain.Stations;

/// <summary>
/// Rovnost zaznamov suborov stanic vratane zoznamov v nich (record porovnava zoznamy len odkazom).
/// </summary>
internal static class RecordEquality
{
    public static bool Same(TrainPosition a, TrainPosition b) => a == b;

    public static bool Same(TrainAddition a, TrainAddition b) => a == b;

    public static bool Same(MosTrain a, MosTrain b) => a == b;

    public static bool Same(TrainLockout a, TrainLockout b) => a == b;

    public static bool Same(TrainDiversion a, TrainDiversion b) => a == b;

    public static bool Same(TrainText a, TrainText b) => a == b;

    public static bool Same(TrainLanguages a, TrainLanguages b) =>
        a.Number == b.Number && a.Validity == b.Validity && a.EditedForDay == b.EditedForDay && a.Languages.SequenceEqual(b.Languages);

    public static bool Same(TrainComposition a, TrainComposition b) =>
        a.Number == b.Number && a.EndStation == b.EndStation && a.Reports == b.Reports && a.Validity == b.Validity &&
        a.EditedForDay == b.EditedForDay && ReferenceEquals(a.Origin, b.Origin) && a.Sounds.SequenceEqual(b.Sounds);

    public static bool Same(TrainReportStations a, TrainReportStations b) =>
        a.Number == b.Number && a.Validity == b.Validity && a.EditedForDay == b.EditedForDay && a.ShortFrom.SequenceEqual(b.ShortFrom) &&
        a.LongFrom.SequenceEqual(b.LongFrom) && a.ShortTo.SequenceEqual(b.ShortTo) && a.LongTo.SequenceEqual(b.LongTo);

    public static bool Same(TrainTextSet a, TrainTextSet b) =>
        ReferenceEquals(a, b) || (a.Name == b.Name && a.Fonet == b.Fonet && a.Targets.SequenceEqual(b.Targets) && a.Trains.SequenceEqual(b.Trains));
}
