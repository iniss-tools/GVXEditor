using System.ComponentModel;
using System.Globalization;
using GVXEditor.Domain.Setup.Drafts;
using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Setup;

/// <summary>
/// Hlasenia obsluhy (ConstReports.xml) v okne Hlasenia obsluhy.
/// </summary>
internal sealed class ConstReportsEdit
{
    private ConstReportsEdit(IReadOnlyList<ConstReport> reports) => Original = reports;

    /// <summary>Hlasenia pred upravou.</summary>
    public IReadOnlyList<ConstReport> Original { get; }

    /// <summary>Hlasenia v poradi suboru.</summary>
    public BindingList<ConstReportDraft> Reports { get; } = [];

    /// <summary>Koncepty hlaseni.</summary>
    public static ConstReportsEdit Of(IReadOnlyList<ConstReport> reports)
    {
        ArgumentNullException.ThrowIfNull(reports);
        var edit = new ConstReportsEdit(reports);
        foreach (var report in reports)
            edit.Reports.Add(ConstReportDraft.Of(report));
        return edit;
    }

    /// <summary>
    /// Hlasenia z konceptov; bez zmeny vrati <see cref="Original" />.
    /// </summary>
    public IReadOnlyList<ConstReport> Apply() => SetupEquality.Keep(Original, [.. Reports.Select(report => report.To())], SetupEquality.Same);

    /// <summary>
    /// Najmensie cislo pre nove hlasenie - ako server za najvyssim pouzitym.
    /// </summary>
    public int NextKey() => Reports.Select(report => report.Number ?? 0).DefaultIfEmpty(0).Max() + 1;

    /// <summary>
    /// Cisla hlaseni obsluhy, na ktore sa odkazuju dodatky stanic, vyluky a odklony grafikonov.
    /// </summary>
    public static HashSet<int> UsedKeys(IEnumerable<GroupDocument> groups)
    {
        ArgumentNullException.ThrowIfNull(groups);
        var keys = new List<string>();
        foreach (var group in groups)
        {
            keys.AddRange(group.Stations.SelectMany(station => station.Files?.Additions ?? []).Select(addition => addition.Key));
            keys.AddRange((group.Lockouts ?? []).Select(lockout => lockout.Key));
            keys.AddRange((group.Diversions ?? []).Select(diversion => diversion.Key));
        }

        var used = new HashSet<int>();
        foreach (var key in keys)
        {
            if (int.TryParse(key.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out var number))
                used.Add(number);
        }

        return used;
    }
}
