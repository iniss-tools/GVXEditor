using System.ComponentModel;
using GVXEditor.Domain.Setup.Drafts;

namespace GVXEditor.Domain.Setup;

/// <summary>
/// Nastavenie grafikonu v okne Nastavenie grafikonu: nazvy stanic a predvolene stanice hlasenia, k nim nastavenie stanic
/// podla poradia (vznika az pri prvom zobrazeni stanice).
/// </summary>
internal sealed class GroupSetupEdit
{
    private readonly Dictionary<int, StationSetupEdit?> _stations = [];

    private GroupSetupEdit(GroupDocument group) => Group = group;

    /// <summary>Grafikon pred upravou.</summary>
    public GroupDocument Group { get; }

    /// <summary>Nazvy stanic; prazdny, ak sa nastavenie grafikonu nedalo nacitat.</summary>
    public BindingList<StationNameDraft> Names { get; } = [];

    /// <summary>Predvolene stanice hlasenia do smeru.</summary>
    public BindingList<DefaultStationDraft> DefaultStations { get; } = [];

    /// <summary>Upravovane stanice (zobrazene aspon raz).</summary>
    public IEnumerable<StationSetupEdit> Stations => _stations.Values.OfType<StationSetupEdit>();

    /// <summary>
    /// Koncepty nastavenia grafikonu.
    /// </summary>
    public static GroupSetupEdit Of(GroupDocument group)
    {
        ArgumentNullException.ThrowIfNull(group);
        var edit = new GroupSetupEdit(group);
        foreach (var station in group.Setup?.Names ?? [])
            edit.Names.Add(StationNameDraft.Of(station));
        foreach (var station in group.Setup?.DefaultStations ?? [])
            edit.DefaultStations.Add(DefaultStationDraft.Of(station));
        return edit;
    }

    /// <summary>
    /// Koncepty nastavenia stanice podla poradia; <see langword="null" />, ak sa nastavenie stanice nedalo nacitat.
    /// </summary>
    public StationSetupEdit? Station(int index)
    {
        if (!_stations.TryGetValue(index, out var edit))
            _stations[index] = edit = StationSetupEdit.Of(index, Group.Stations[index]);
        return edit;
    }

    /// <summary>
    /// Grafikon s nastavenim z konceptov. Nezmenene subory ostanu tie iste objekty, bez zmeny vrati <paramref name="group" />.
    /// </summary>
    /// <param name="group">aktualny grafikon (ten isty, z ktoreho okno vzniklo)</param>
    public GroupDocument Apply(GroupDocument group)
    {
        ArgumentNullException.ThrowIfNull(group);
        var result = group;
        if (group.Setup is { } setup)
        {
            var names = SetupEquality.Keep(setup.Names, [.. Names.Select(name => name.To())], SetupEquality.Equal);
            var defaults = SetupEquality.Keep(setup.DefaultStations, [.. DefaultStations.Select(station => station.To())], SetupEquality.Equal);
            if (!ReferenceEquals(names, setup.Names) || !ReferenceEquals(defaults, setup.DefaultStations))
                result = result with { Setup = new GroupSetup(names, defaults) };
        }

        foreach (var station in Stations)
        {
            var applied = station.Apply();
            if (!ReferenceEquals(applied, station.Setup))
                result = result.WithStationSetup(station.Index, applied);
        }

        return result;
    }
}
