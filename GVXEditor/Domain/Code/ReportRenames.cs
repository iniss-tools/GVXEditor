using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Code;

/// <summary>
/// Premenovanie funkcii skriptu hlaseni v datach, ktore na ne odkazuju: typy hlaseni (ReportKeys.xml), stavy
/// (statedgm.xml), hlasenia obsluhy (ConstReports.xml), dodatky stanic, vyluky a odklony. AGV.xml je stav servera
/// a nemeni sa. Nazov sa porovnava bez ohladu na velkost pismen (ako VB.NET); co sa nemeni, ostava ten isty objekt.
/// </summary>
internal static class ReportRenames
{
    /// <summary>
    /// Stav instalacie s premenovanymi odkazmi; bez odkazu vrati <paramref name="state" />.
    /// </summary>
    /// <param name="state">stav instalacie</param>
    /// <param name="renames">povodny nazov → novy nazov</param>
    public static WorkspaceState Apply(WorkspaceState state, IReadOnlyDictionary<string, string> renames)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(renames);
        if (renames.Count == 0)
            return state;

        var map = new Dictionary<string, string>(renames, StringComparer.OrdinalIgnoreCase);
        var result = state;
        for (var g = 0; g < state.Groups.Count; g++)
        {
            var group = Group(state.Groups[g], map);
            if (!ReferenceEquals(group, state.Groups[g]))
                result = result.With(g, group);
        }

        if (state.ConstReports is { } reports && Rename(reports, report => report.Function, (report, name) => report with { Function = name }, map) is { } renamed)
            result = result with { ConstReports = renamed };
        return result;
    }

    private static GroupDocument Group(GroupDocument group, Dictionary<string, string> map)
    {
        var result = group;
        for (var s = 0; s < group.Stations.Count; s++)
        {
            var station = group.Stations[s];
            if (station.Setup is { } setup && Rename(setup.ReportKeys, key => key.Name, (key, name) => key with { Name = name }, map) is { } keys)
                result = result.WithStationSetup(s, setup with { ReportKeys = keys });
            if (station.Code is { Diagram: { } diagram } code && Diagram(diagram, map) is { } renamedDiagram)
                result = result.WithStationCode(s, code with { Diagram = renamedDiagram });
            if (station.Files is { } files && Rename(files.Additions, addition => addition.Key, (addition, name) => addition with { Key = name }, map) is { } additions)
                result = result.WithStation(s, files with { Additions = additions });
        }

        if (group.Lockouts is { } lockouts && Rename(lockouts, lockout => lockout.Key, (lockout, name) => lockout with { Key = name }, map) is { } renamedLockouts)
            result = result with { Lockouts = renamedLockouts };
        if (group.Diversions is { } diversions
            && Rename(diversions, diversion => diversion.Key, (diversion, name) => diversion with { Key = name }, map) is { } renamedDiversions)
            result = result with { Diversions = renamedDiversions };
        return result;
    }

    private static StateDiagram? Diagram(StateDiagram diagram, Dictionary<string, string> map)
    {
        var changed = false;
        var graphs = diagram.Graphs.Select(graph =>
        {
            if (Rename(graph.Nodes, node => node.Report, (node, name) => node with { Report = name }, map) is not { } nodes)
                return graph;
            changed = true;
            return graph with { Nodes = nodes };
        }).ToList();
        return changed ? diagram with { Graphs = graphs } : null;
    }

    // novy zoznam s premenovanymi polozkami; null, ak sa nic nezmenilo
    private static List<T>? Rename<T>(IReadOnlyList<T> items, Func<T, string> name, Func<T, string, T> with, Dictionary<string, string> map)
    {
        if (!items.Any(item => map.ContainsKey(name(item).Trim())))
            return null;
        return [.. items.Select(item => map.TryGetValue(name(item).Trim(), out var renamed) ? with(item, renamed) : item)];
    }
}
