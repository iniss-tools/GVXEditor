using ExControls;
using GVXEditor.Properties;
using GVXEditor.UI.Analysis;
using GVXEditor.UI.Settings;
using Serilog;
using ToolsCore;
using ToolsCore.Commands;
using ToolsCore.Forms;
using ToolsCore.Iniss.Tools;
using ToolsCore.Tools;
using ToolsCore.Updates;
using ToolsCore.XML;

namespace GVXEditor.UI.Main;

/// <summary>
/// Hlavne okno: strom grafikonov a stanic otvorenej instalacie, vlaky grafikonu s variantmi a trasou a vlaky stanice v den.
/// </summary>
internal partial class FMain : ExForm
{
    private readonly EditorContext _ctx;
    private readonly CommandSet _commands = new();

    // stav servera sa meni mimo programu - zistuje sa pravidelne
    private readonly System.Windows.Forms.Timer _serverTimer = new() { Interval = 3000 };

    /// <param name="context">kontext editora</param>
    public FMain(EditorContext context)
    {
        _ctx = context;
        InitializeComponent();
        components?.Add(_serverTimer);
        _analysisView = new AnalysisView(_ctx, Navigate);
        InitDocking();

        foreach (var grid in new[] { dgvTrains, dgvVariants, dgvStops, dgvStation })
            grid.AutoGenerateColumns = false;

        this.ApplyThemeAndFonts();
        AppDock.Restore(_dock, _ctx.Config.DockLayout);
        ApplyToolbar();
        CreateCommands();
        _commands.ApplyShortcuts(_ctx.Config.Shortcuts);
        InitColumns();

        dtpDay.Value = DateTime.Today;
        _serverTimer.Tick += (_, _) => UpdateServerState();
        Disposed += (_, _) => _boldTypeFont?.Dispose();
        SetRecent();
        ShowView(View.Empty);
        UpdateDirtyState();
    }

    /// <summary>
    /// Prikazy hlavneho okna naviazane naraz na tlacidlo aj polozku ponuky.
    /// </summary>
    private void CreateCommands()
    {
        _commands.Add(GvxCommands.Open, OpenInstallation).Bind(tsmimOpen, tsbOpen);
        _commands.Add(GvxCommands.Reload, Reload, () => _ctx.Workspace is not null).Bind(tsmimReload, tsbReload);
        CreateEditCommands();
        CreateViewCommands();
        CreateSetupCommands();
        CreateCodeCommands();
        _commands.Add(GvxCommands.AppSettings, ShowAppSettings).Bind(tsmimAppSettings, tsbAppSettings);
        _commands.Add(GvxCommands.InfoApp, ShowInfoApp).Bind(tsmimInfoApp, tsbInfoApp);
        _commands.Add(GvxCommands.CheckUpdates, () => AppUpdates.CheckNow(this), () => AppUpdates.Available).Bind(tsmimCheckUpdates);
        _commands.Add(GvxCommands.ShowLogs, ErrorReport.OpenLogFolder).Bind(tsmimShowLogs);
    }

    private void UpdateCommandStates() => _commands.UpdateStates();

    /// <inheritdoc />
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData) =>
        _commands.ProcessShortcut(keyData) || base.ProcessCmdKey(ref msg, keyData);

    /// <summary>
    /// Tabulky si pamataju poradie, sirku a viditelnost stlpcov.
    /// </summary>
    private void InitColumns()
    {
        GridColumns.Attach(dgvTrains, "Trains", () => _ctx.Config, SaveConfig);
        GridColumns.Attach(dgvVariants, "Variants", () => _ctx.Config, SaveConfig);
        GridColumns.Attach(dgvStops, "Stops", () => _ctx.Config, SaveConfig);
        GridColumns.Attach(dgvStation, "StationDay", () => _ctx.Config, SaveConfig);
    }

    /// <summary>
    /// Zapise nastavenia programu (napr. po zmene stlpcov tabulky). Chyba zapisu sa len zaloguje.
    /// </summary>
    private void SaveConfig()
    {
        try
        {
            var dir = AppPaths.ConfigDir;
            Directory.CreateDirectory(dir);
            XmlSerialization.WriteData(PathUtils.CombinePath(dir, FileConsts.FileConfig)!, _ctx.Config);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException or InvalidOperationException)
        {
            Log.Warning(e, "Nastavenia programu sa nepodarilo ulozit");
        }
    }

    /// <summary>
    /// Panel nastrojov podla nastaveni (ponuka je vzdy).
    /// </summary>
    private void ApplyToolbar() => toolStripMain.Visible = _ctx.Config.ShowToolbar;

    private void ShowAppSettings()
    {
        using var form = new FAppSettings(_ctx, _commands);
        // po kazdom ulozeni (aj tlacidlom Pouzit pri otvorenom okne)
        form.Applied += (_, _) => UpdateMainUi();
        form.ShowDialog(this);
    }

    private void UpdateMainUi()
    {
        ApplyToolbar();
        this.ApplyThemeAndFonts();
        _commands.ApplyShortcuts(_ctx.Config.Shortcuts);
        Invalidate(true);
        AppInit.MsgBoxStyleInit(_ctx.UsingStyle, _ctx.Config);
    }

    private void ShowInfoApp()
    {
        using var form = new FAboutApp(Resources.AboutAppDescription, Resources.gvx);
        form.ShowDialog(this);
    }

    private void FMain_Load(object sender, EventArgs e)
    {
        AppRegistry.RegisterJumpList(GlobalResources.RRecentFiles);
        AppUpdates.CheckOnStartup(this, _ctx.Config);

        // cesta zadana ako argument ma prednost pred poslednou otvorenou instalaciou
        var path = ToolsCore.Tools.Utils.GetProjectPathFromArgs();
        if (path is null && _ctx.Config.Startup == StartupType.LastProject)
            path = AppRegistry.GetLastProject();

        if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
            Open(path);
    }

    private void FMain_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (!ConfirmClose())
        {
            e.Cancel = true;
            return;
        }

        _serverTimer.Stop();
        SaveDockLayout();
    }
}
