using System.Globalization;
using GVXEditor.Properties;

namespace GVXEditor.Domain.Code;

/// <summary>
/// Odkazy z dat na funkciu skriptu hlaseni: typy hlaseni (ReportKeys.xml), stavy (statedgm.xml), hlasenia obsluhy
/// (ConstReports.xml), dodatky stanic, vyluky a odklony grafikonov. Klient ponuka hlasenia aj bez odkazu (podla oznacenia).
/// </summary>
/// <param name="ReportKeys">typy hlaseni stanic s tymto nazvom</param>
/// <param name="StateNodes">stavy stavovych diagramov s tymto hlasenim</param>
/// <param name="ConstReports">hlasenia obsluhy s tymto vzorom</param>
/// <param name="Additions">zaznamy dodatkov s tymto klucom</param>
/// <param name="Lockouts">zaznamy vyluk s tymto klucom</param>
/// <param name="Diversions">zaznamy odklonov s tymto klucom</param>
internal sealed record ReportReferences(int ReportKeys, int StateNodes, int ConstReports, int Additions, int Lockouts, int Diversions)
{
    /// <summary>Pocet vsetkych odkazov.</summary>
    public int Total => ReportKeys + StateNodes + ConstReports + Additions + Lockouts + Diversions;

    /// <summary>
    /// Text pre obsluhu, napr. <c>typy hlaseni 2, stavy 6</c>; bez odkazov prazdny.
    /// </summary>
    public string Describe()
    {
        var parts = new List<string>();
        Add(Resources.Reports_RefReportKeys, ReportKeys);
        Add(Resources.Reports_RefStateNodes, StateNodes);
        Add(Resources.Reports_RefConstReports, ConstReports);
        Add(Resources.Reports_RefAdditions, Additions);
        Add(Resources.Reports_RefLockouts, Lockouts);
        Add(Resources.Reports_RefDiversions, Diversions);
        return string.Join(", ", parts);

        void Add(string format, int count)
        {
            if (count > 0)
                parts.Add(string.Format(CultureInfo.CurrentCulture, format, count));
        }
    }

    /// <summary>
    /// Odkazy podla nazvu funkcie (bez ohladu na velkost pismen ako VB.NET).
    /// </summary>
    /// <param name="state">stav instalacie</param>
    public static IReadOnlyDictionary<string, ReportReferences> Of(WorkspaceState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        var counts = new Dictionary<string, int[]>(StringComparer.OrdinalIgnoreCase);
        foreach (var group in state.Groups)
        {
            foreach (var station in group.Stations)
            {
                foreach (var key in station.Setup?.ReportKeys ?? [])
                    Count(key.Name, 0);
                foreach (var node in station.Code?.Diagram?.Graphs.SelectMany(graph => graph.Nodes) ?? [])
                    Count(node.Report, 1);
                foreach (var addition in station.Files?.Additions ?? [])
                    Count(addition.Key, 3);
            }

            foreach (var lockout in group.Lockouts ?? [])
                Count(lockout.Key, 4);
            foreach (var diversion in group.Diversions ?? [])
                Count(diversion.Key, 5);
        }

        foreach (var report in state.ConstReports ?? [])
            Count(report.Function, 2);

        return counts.ToDictionary(pair => pair.Key, pair => new ReportReferences(pair.Value[0], pair.Value[1], pair.Value[2], pair.Value[3],
            pair.Value[4], pair.Value[5]), StringComparer.OrdinalIgnoreCase);

        void Count(string name, int place)
        {
            var key = name.Trim();
            if (key.Length == 0)
                return;
            if (!counts.TryGetValue(key, out var values))
                counts[key] = values = new int[6];
            values[place]++;
        }
    }
}
