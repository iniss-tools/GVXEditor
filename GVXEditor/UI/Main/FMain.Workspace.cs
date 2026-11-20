using System.Globalization;
using System.Xml;
using GVXEditor.Domain;
using GVXEditor.Domain.Views;
using GVXEditor.Properties;
using Serilog;
using ToolsCore.Iniss2;
using ToolsCore.Iniss2.Grafikon;
using ToolsCore.Tools;

namespace GVXEditor.UI.Main;

internal partial class FMain
{
    /// <summary>Co ukazuje prava cast okna.</summary>
    private enum View
    {
        /// <summary>Ziadna instalacia alebo vyber.</summary>
        Empty,

        /// <summary>Vlaky grafikonu.</summary>
        Trains,

        /// <summary>Vlaky stanice v den.</summary>
        Station
    }

    /// <summary>Uzol grafikonu v strome - poradie, lebo zmena vlakov vytvori novy <see cref="GroupDocument" />.</summary>
    /// <param name="Index">poradie grafikonu v DirList.xml</param>
    private sealed record GroupNode(int Index);

    /// <summary>Uzol stanice v strome.</summary>
    /// <param name="GroupIndex">poradie grafikonu stanice</param>
    /// <param name="Station">stanica</param>
    private sealed record StationNode(int GroupIndex, DirListItem Station);

    private StationTimetable? _timetable;

    private async void OpenInstallation()
    {
        if (!ConfirmClose())
            return;

        using var dialog = new FolderBrowserDialog { Description = Resources.Main_OpenFolder, UseDescriptionForTitle = true };
        if (_ctx.Workspace is { } workspace)
            dialog.InitialDirectory = workspace.DataDir;

        if (await dialog.ShowDialogAsync(this) == DialogResult.OK && !IsDisposed)
            Open(dialog.SelectedPath);
    }

    private void Reload()
    {
        if (_ctx.Workspace is not { } workspace)
            return;
        if (workspace.IsDirty && ToolsCore.Tools.Utils.ShowQuestion(Resources.Main_ReloadDirty) != DialogResult.Yes)
            return;

        var selected = tvData.SelectedNode?.FullPath;
        Open(workspace.DataDir);
        if (selected is not null && Find(tvData.Nodes, selected) is { } node)
            tvData.SelectedNode = node;
    }

    private static TreeNode? Find(System.Collections.IEnumerable nodes, string path)
    {
        foreach (TreeNode node in nodes)
        {
            if (node.FullPath == path)
                return node;
            if (Find(node.Nodes, path) is { } child)
                return child;
        }

        return null;
    }

    /// <summary>
    /// Otvori instalaciu INISS2 z priecinka instalacie, servera alebo dat.
    /// </summary>
    private void Open(string path)
    {
        if (Iniss2Installation.DataDirectory(path) is not { } dataDir)
        {
            Log.Information("Priecinok {Path} nie je instalacia INISS2", path);
            ToolsCore.Tools.Utils.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.Main_NotIniss2, path));
            return;
        }

        Iniss2Workspace workspace;
        try
        {
            Log.Information("Otvara sa instalacia INISS2 {DataDir}", dataDir);
            workspace = Iniss2Workspace.Load(dataDir);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException or XmlException or FormatException)
        {
            Log.Error(e, "Instalaciu INISS2 {DataDir} sa nepodarilo otvorit", dataDir);
            ToolsCore.Tools.Utils.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.Main_OpenFailed, dataDir) + Environment.NewLine + e.Message);
            return;
        }

        _ctx.Open(workspace);
        AppRegistry.SetUsageOfProject(dataDir);
        AppRegistry.SetLastProject(dataDir);
        SetRecent();

        FillTree(workspace);
        UpdateServerState();
        _serverTimer.Start();
        UpdateDirtyState();
    }

    private void SetRecent()
    {
        tsmimRecent.DropDownItems.Clear();
        tssbRecent.DropDownItems.Clear();

        foreach (var project in AppRegistry.GetOpenedProjects().Where(project => Directory.Exists(project.Path)))
        {
            foreach (var menu in new ToolStripDropDownItem[] { tsmimRecent, tssbRecent })
            {
                var entry = new ToolStripMenuItem(project.Path);
                entry.Click += (_, _) => Open(project.Path);
                entry.ApplyThemeAndFont();
                menu.DropDownItems.Add(entry);
            }
        }

        tsmimRecent.Enabled = tssbRecent.Enabled = tsmimRecent.DropDownItems.Count > 0;
    }

    private void FillTree(Iniss2Workspace workspace)
    {
        tvData.BeginUpdate();
        tvData.Nodes.Clear();
        for (var index = 0; index < workspace.Groups.Count; index++)
        {
            var group = workspace.Groups[index];
            var node = new TreeNode(group.Error is null ? group.Group.Name : string.Format(CultureInfo.CurrentCulture, Resources.Main_GroupNodeFailed, group.Group.Name))
            {
                Tag = new GroupNode(index),
                ToolTipText = group.Error is null ? "" : string.Format(CultureInfo.CurrentCulture, Resources.Main_GroupFailed, group.Group.Name, group.Error)
            };

            foreach (var station in group.Group.Items)
                node.Nodes.Add(new TreeNode(station.Directory) { Tag = new StationNode(index, station) });

            tvData.Nodes.Add(node);
        }

        tvData.ExpandAll();
        tvData.EndUpdate();

        if (tvData.Nodes.Count > 0)
            tvData.SelectedNode = tvData.Nodes[0];
        else
            ShowView(View.Empty);
    }

    private void tvData_AfterSelect(object sender, TreeViewEventArgs e)
    {
        switch (e.Node?.Tag)
        {
            case GroupNode group:
                ShowTrains(group.Index);
                break;
            case StationNode station:
                ShowStation(station);
                break;
            default:
                ShowView(View.Empty);
                break;
        }

        UpdateCommandStates();
    }

    private void ShowView(View view)
    {
        pTrains.Visible = view == View.Trains;
        pStation.Visible = view == View.Station;
        lEmpty.Visible = view == View.Empty;
        if (view == View.Empty)
            tsslCount.Text = "";
    }

    private string StationName(int number) => _ctx.Workspace?.StationName(number) ?? number.ToString(CultureInfo.InvariantCulture);

    private void ShowTrains(int index)
    {
        if (_ctx.Workspace is not { } workspace)
            return;

        var group = workspace.Groups[index];
        if (group.Timetable is not { } timetable)
        {
            lEmpty.Text = string.Format(CultureInfo.CurrentCulture, Resources.Main_GroupFailed, group.Group.Name, group.Error);
            ShowView(View.Empty);
            return;
        }

        FillTrains([.. timetable.Trains.Select((train, position) => TrainRow.Create(train, StationName, position))]);
        ShowView(View.Trains);
        UpdateTrainCount();
        _shownTrain = dgvTrains.CurrentRow?.DataBoundItem as TrainRow;
        ShowVariants();
    }

    private void ShowVariants()
    {
        dgvVariants.DataSource = _shownTrain is { } row ? VariantRow.Create(row.Train) : null;
        ShowStops();
    }

    private void dgvVariants_SelectionChanged(object sender, EventArgs e) => ShowStops();

    private void ShowStops() =>
        dgvStops.DataSource = Selected<VariantRow>(dgvVariants) is { } row ? StopRow.Create(row.Variant, StationName) : null;

    // pocas SelectionChanged este CurrentRow ukazuje na predchadzajuci riadok - plati vybrany riadok
    private static T? Selected<T>(DataGridView grid) where T : class =>
        (grid.SelectedRows.Count > 0 ? grid.SelectedRows[0] : grid.CurrentRow)?.DataBoundItem as T;

    private void ShowStation(StationNode node)
    {
        ShowView(View.Station);
        lStation.Text = string.Create(CultureInfo.CurrentCulture, $"{StationName(node.Station.Station)} ({node.Station.Station})");
        _timetable = null;

        if (_ctx.Workspace is not { } workspace)
            return;

        var group = workspace.Groups[node.GroupIndex];
        try
        {
            _timetable = workspace.StationTimetable(group, node.Station);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException or XmlException or FormatException or OverflowException)
        {
            Log.Warning(e, "Vlaky stanice {Station} grafikonu {Group} sa nepodarilo nacitat", node.Station.Directory, group.Group.Name);
            lStation.Text = string.Format(CultureInfo.CurrentCulture, Resources.Main_StationFailed, node.Station.Directory, e.Message);
        }

        ShowStationDay();
    }

    private void dtpDay_ValueChanged(object sender, EventArgs e) => ShowStationDay();

    private void ShowStationDay()
    {
        if (_timetable is null)
        {
            dgvStation.DataSource = null;
            tsslCount.Text = "";
            return;
        }

        var calls = _timetable.On(DateOnly.FromDateTime(dtpDay.Value));
        dgvStation.DataSource = StationDayRow.Create(calls, StationName);
        tsslCount.Text = string.Format(CultureInfo.CurrentCulture, Resources.Status_Calls, calls.Count);
    }

    /// <summary>
    /// Stav servera instalacie v stavovom riadku. Server tejto instalacie drzi jej data v pamati.
    /// </summary>
    private void UpdateServerState()
    {
        if (_ctx.Workspace is not { } workspace)
        {
            tsslServer.Text = "";
            tsslServer.IconName = null;
            return;
        }

        var running = workspace.IsServerRunning();
        var other = !running && Iniss2Server.IsRunning();
        tsslServer.Text = running ? Resources.Server_Running : other ? Resources.Server_Other : Resources.Server_Stopped;
        tsslServer.IconName = running ? "status-warning" : "status-ok";
        tsslServer.ToolTipText = Resources.Server_RunningHint;
    }
}
