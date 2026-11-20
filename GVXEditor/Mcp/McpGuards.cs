using System.Globalization;
using GVXEditor.Domain;
using GVXEditor.Domain.Stations;
using ModelContextProtocol;
using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Mcp;

/// <summary>
/// Spolocne kontroly nastrojov: otvorena instalacia, grafikon, stanica a vlak podla parametrov agenta. Chyba je
/// <see cref="McpException" /> s kodom a radou, co agent ma urobit.
/// </summary>
internal static class McpGuards
{
    /// <summary>Otvorena instalacia; bez nej chyba nastroja.</summary>
    public static Iniss2Workspace Workspace(EditorContext context) =>
        context.Workspace ?? throw new McpException("installation.notOpen: No INISS2 installation is open in GVXEditor; ask the user to open one.");

    /// <summary>
    /// Grafikon podla nazvu z DirList.xml; bez nazvu prvy grafikon instalacie.
    /// </summary>
    public static (int Index, GroupDocument Group) Group(Iniss2Workspace workspace, string? name)
    {
        var groups = workspace.Groups;
        if (groups.Count == 0)
            throw new McpException("group.none: The installation has no timetable group in DirList.xml.");
        if (string.IsNullOrWhiteSpace(name))
            return (0, groups[0]);
        for (var i = 0; i < groups.Count; i++)
        {
            if (string.Equals(groups[i].Group.Name, name.Trim(), StringComparison.OrdinalIgnoreCase))
                return (i, groups[i]);
        }

        throw new McpException($"group.notFound: Timetable group '{name}' does not exist; groups: {string.Join(", ", groups.Select(g => g.Group.Name))}.");
    }

    /// <summary>
    /// Grafikon s nacitanym GV.XML; chyba nacitania je chyba nastroja.
    /// </summary>
    public static GvFile Timetable(GroupDocument group) =>
        group.Timetable ?? throw new McpException($"group.timetable: GV.XML of group '{group.Group.Name}' could not be read: {group.Error}");

    /// <summary>
    /// Stanica grafikonu podla cisla SR70, priecinka alebo nazvu.
    /// </summary>
    public static (int Index, StationDocument Station) Station(Iniss2Workspace workspace, GroupDocument group, string station)
    {
        var text = station.Trim();
        var stations = group.Stations;
        for (var i = 0; i < stations.Count; i++)
        {
            var item = stations[i].Station;
            if (item.Station.ToString(CultureInfo.InvariantCulture) == text
                || string.Equals(item.Directory, text, StringComparison.OrdinalIgnoreCase)
                || string.Equals(workspace.StationName(item.Station), text, StringComparison.OrdinalIgnoreCase))
                return (i, stations[i]);
        }

        throw new McpException($"station.notFound: Station '{station}' is not in group '{group.Group.Name}'; stations: "
                               + string.Join(", ", stations.Select(s => $"{s.Station.Station} {s.Station.Directory}")) + ".");
    }

    /// <summary>Vlak podla poradia v GV.XML.</summary>
    public static GvTrain Train(GroupDocument group, int index)
    {
        var trains = Timetable(group).Trains;
        return index >= 0 && index < trains.Count
            ? trains[index]
            : throw new McpException($"train.notFound: Train {index} does not exist in group '{group.Group.Name}' ({trains.Count} trains); use list_trains.");
    }
}
