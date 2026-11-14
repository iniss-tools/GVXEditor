using System.Globalization;
using System.Xml;
using GVXEditor.Domain.Setup;
using GVXEditor.Domain.Stations;
using GVXEditor.Properties;
using GVXEditor.UI.Boards;
using GVXEditor.UI.Setup;
using Serilog;
using ToolsCore.Iniss2.Grafikon;
using ToolsCore.Tools;

namespace GVXEditor.UI.Main;

// nastavenie grafikonu a stanic, hlasenia obsluhy, novy grafikon a nova stanica
internal partial class FMain
{
    private void CreateSetupCommands()
    {
        _commands.Add(GvxCommands.GroupSetup, GroupSetup, () => SelectedGroupIndex >= 0).Bind(tsmimGroupSetup, tsbGroupSetup, tsmitGroupSetup);
        _commands.Add(GvxCommands.ConstReports, ConstReports, () => _ctx.Workspace?.History.Current.ConstReports is not null)
            .Bind(tsmimConstReports, tsbConstReports);
        _commands.Add(GvxCommands.NewGroup, NewGroup, () => _ctx.Workspace?.Groups.Any(group => group.Timetable is not null) == true).Bind(tsmimNewGroup);
        _commands.Add(GvxCommands.AddStation, AddStation, () => SelectedGroupIndex >= 0).Bind(tsmimAddStation, tsmitAddStation);
        _commands.Add(GvxCommands.GenerateTexts, GenerateTexts,
                () => SelectedGroupIndex is >= 0 and var index && _ctx.Workspace?.Groups[index].Timetable is not null)
            .Bind(tsmimGenerateTexts, tsmitGenerateTexts);
    }

    // texty vlakov do sady TTVlaky.xml stanice ako jeden krok historie
    private void GenerateTexts()
    {
        var index = SelectedGroupIndex;
        if (_ctx.Workspace is not { } workspace || index < 0)
            return;

        var group = workspace.Groups[index];
        var defaultsFile = File.Exists(Path.Combine(workspace.DataDir, group.Group.Name, DefaultStationsFile.FileName));
        using var form = new FGenerateTexts(group, SelectedStationIndex(), StationName, defaultsFile);
        if (form.ShowDialog(this) != DialogResult.OK)
            return;

        var (updated, written, set) = form.Apply(group);
        if (ReferenceEquals(updated, group))
            return;

        var label = string.Format(CultureInfo.CurrentCulture, Resources.History_GenerateTexts, group.Stations[form.StationIndex].Station.Directory, set);
        Log.Information("Grafikon {Group}: {Change}, zapisanych textov {Written}", group.Group.Name, label, written);
        workspace.Change(index, updated, label);
        RefreshView();
    }

    // pravy klik vyberie uzol, ku ktoremu sa ponuka otvori
    private void tvData_NodeMouseClick(object? sender, TreeNodeMouseClickEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
            tvData.SelectedNode = e.Node;
    }

    // poradie stanice vybranej v strome v jej grafikone; -1 = vybrany je grafikon
    private int SelectedStationIndex()
    {
        if (_ctx.Workspace is not { } workspace || tvData.SelectedNode?.Tag is not StationNode node)
            return -1;
        var stations = workspace.Groups[node.GroupIndex].Stations;
        for (var i = 0; i < stations.Count; i++)
        {
            if (ReferenceEquals(stations[i].Station, node.Station))
                return i;
        }

        return -1;
    }

    private void GroupSetup()
    {
        var index = SelectedGroupIndex;
        if (_ctx.Workspace is not { } workspace || index < 0)
            return;

        var group = workspace.Groups[index];
        using var form = new FSetup(group, StationName, SelectedStationIndex(), workspace.Bank);
        if (form.ShowDialog(this) != DialogResult.OK)
            return;

        var updated = form.Apply(group);
        if (ReferenceEquals(updated, group))
            return;

        var label = string.Format(CultureInfo.CurrentCulture, Resources.History_GroupSetup, group.Group.Name);
        Log.Information("Grafikon {Group}: {Change}", group.Group.Name, label);
        workspace.Change(index, updated, label);
        RefreshView();
    }

    private void ConstReports()
    {
        if (_ctx.Workspace is not { } workspace || workspace.History.Current.ConstReports is not { } reports)
            return;

        var keys = new KeyCatalog(workspace.Functions, workspace.FunctionDescriptions, reports);
        using var form = new FConstReports(reports, keys, ConstReportsEdit.UsedKeys(workspace.Groups), workspace.StationNames);
        if (form.ShowDialog(this) != DialogResult.OK)
            return;

        var updated = form.Apply();
        if (ReferenceEquals(updated, reports))
            return;

        Log.Information("Instalacia {DataDir}: {Change}", workspace.DataDir, Resources.History_ConstReports);
        workspace.ChangeConstReports(updated, Resources.History_ConstReports);
        RefreshView();
    }

    // novy grafikon a stanica sa zapisu hned - instalacia nesmie mat neulozene zmeny a server nesmie bezat
    private static bool CanChangeStructure(Domain.Iniss2Workspace workspace)
    {
        if (workspace.IsDirty)
        {
            Utils.ShowWarning(Resources.Structure_Dirty);
            return false;
        }

        if (workspace.IsServerRunning())
        {
            Utils.ShowWarning(Resources.Structure_ServerRunning);
            return false;
        }

        return true;
    }

    private void NewGroup()
    {
        if (_ctx.Workspace is not { } workspace || !CanChangeStructure(workspace))
            return;

        using var form = new FNewGroup(workspace, SelectedGroupIndex);
        if (form.ShowDialog(this) != DialogResult.OK)
            return;

        var name = form.GroupName;
        if (!ChangeStructure(workspace, () => StructureEdits.NewGroup(workspace, name, form.Template, form.CopyTrains), name))
            return;
        ReloadAt(workspace.DataDir, name);
    }

    private void AddStation()
    {
        var index = SelectedGroupIndex;
        if (_ctx.Workspace is not { } workspace || index < 0 || !CanChangeStructure(workspace))
            return;
        if (!workspace.Groups.Any(group => group.Stations.Count > 0))
        {
            Utils.ShowWarning(Resources.Structure_NoStations);
            return;
        }

        using var form = new FAddStation(workspace, index, SelectedStationIndex());
        if (form.ShowDialog(this) != DialogResult.OK)
            return;

        var station = form.Station;
        var group = workspace.Groups[index].Group.Name;
        if (!ChangeStructure(workspace, () => StructureEdits.AddStation(workspace, station), group))
            return;
        ReloadAt(workspace.DataDir, Path.Combine(group, station.Directory));
    }

    private static bool ChangeStructure(Domain.Iniss2Workspace workspace, Action change, string what)
    {
        try
        {
            change();
            return true;
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException or XmlException or FormatException)
        {
            Log.Error(e, "Zmena struktury instalacie {DataDir} ({What}) zlyhala", workspace.DataDir, what);
            Utils.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.Structure_Failed, e.Message));
            return false;
        }
    }

    // instalacia sa po zmene struktury nacita znova a vyberie sa novy uzol stromu
    private void ReloadAt(string dataDir, string path)
    {
        Open(dataDir);
        if (Find(tvData.Nodes, path) is { } node)
            tvData.SelectedNode = node;
    }
}
