using ToolsCore.Iniss2.Tables;

namespace GVXEditor.Domain.Boards;

/// <summary>
/// Rovnost tabul a predloh vratane zoznamov v nich (record porovnava zoznamy len odkazom).
/// </summary>
internal static class BoardEquality
{
    /// <summary>Tabule sa zhoduju vo vsetkom, co sa zapisuje.</summary>
    public static bool Same(TableDefinition a, TableDefinition b) =>
        a with { Parts = b.Parts } == b && a.Parts.Count == b.Parts.Count && a.Parts.Zip(b.Parts).All(pair => Same(pair.First, pair.Second));

    /// <summary>Casti tabule sa zhoduju vratane kolaji a ich priorit.</summary>
    public static bool Same(TablePart a, TablePart b) =>
        a with { Tracks = b.Tracks, TrackPriorities = b.TrackPriorities } == b && a.Tracks.SequenceEqual(b.Tracks) &&
        a.TrackPriorities.Count == b.TrackPriorities.Count &&
        a.TrackPriorities.All(pair => b.TrackPriorities.TryGetValue(pair.Key, out var priority) && priority == pair.Value);

    /// <summary>Predlohy sa zhoduju vratane udajov.</summary>
    public static bool Same(CatalogTable a, CatalogTable b) => a with { Columns = b.Columns } == b && a.Columns.SequenceEqual(b.Columns);
}
