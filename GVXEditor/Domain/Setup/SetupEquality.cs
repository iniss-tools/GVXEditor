using ToolsCore.Iniss2.Grafikon;
using ToolsCore.Iniss2.Tables;

namespace GVXEditor.Domain.Setup;

/// <summary>
/// Rovnost zaznamov nastavenia vratane zoznamov v nich (record porovnava zoznamy len odkazom) a zachovanie povodneho zoznamu.
/// </summary>
internal static class SetupEquality
{
    /// <summary>
    /// Povodny zoznam, ak ma upraveny rovnake prvky - podla odkazu sa zisti, ze subor netreba zapisat.
    /// </summary>
    public static IReadOnlyList<T> Keep<T>(IReadOnlyList<T> original, IReadOnlyList<T> edited, Func<T, T, bool> same)
    {
        ArgumentNullException.ThrowIfNull(original);
        ArgumentNullException.ThrowIfNull(edited);
        return original.Count == edited.Count && original.Zip(edited).All(pair => same(pair.First, pair.Second)) ? original : edited;
    }

    /// <summary>Rovnost hodnotou (record bez zoznamov).</summary>
    public static bool Equal<T>(T a, T b) => EqualityComparer<T>.Default.Equals(a, b);

    public static bool Same(IReadOnlyList<ReportPatternStop> a, IReadOnlyList<ReportPatternStop> b) => a.SequenceEqual(b);

    public static bool Same(RouteText a, RouteText b) => a.Text == b.Text && a.Font == b.Font && a.Stations.SequenceEqual(b.Stations);

    public static bool Same(RouteTextSet a, RouteTextSet b) =>
        a.Name == b.Name && a.Fonet == b.Fonet && a.Targets.SequenceEqual(b.Targets) && a.Routes.Count == b.Routes.Count &&
        a.Routes.Zip(b.Routes).All(pair => Same(pair.First, pair.Second));

    public static bool Same(StationTextSet a, StationTextSet b) =>
        a.Name == b.Name && a.Fonet == b.Fonet && a.Targets.SequenceEqual(b.Targets) && a.Stations.SequenceEqual(b.Stations);

    public static bool Same(ConstReport a, ConstReport b) =>
        a.Key == b.Key && a.Name == b.Name && a.Function == b.Function && a.Stations.SequenceEqual(b.Stations);
}
