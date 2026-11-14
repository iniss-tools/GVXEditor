using ExControls;
using GVXEditor.Properties;
using GVXEditor.UI.Analysis;
using ToolsCore.Tools;

namespace GVXEditor.UI.Main;

// ukotvitelne panely (ExDockManager): strom instalacie vlavo, kontrola instalacie dole, vlaky a stanica v dokumente
internal partial class FMain
{
    // kluce panelov v ulozenom rozlozeni (config.xml) - nemenit
    private const string InstallationPanelKey = "Installation";
    private const string AnalysisPanelKey = "Analysis";

    private readonly ExDockManager _dock = AppDock.Create();
    private readonly ExDockPanel _installationPanel = new(InstallationPanelKey, Resources.Panel_Installation);
    private readonly ExDockPanel _analysisPanel = new(AnalysisPanelKey, Resources.Panel_Analysis);
    private readonly AnalysisView _analysisView;
    private string _defaultLayout = "";

    // instalacia sa zmenila, ked bol panel kontroly skryty - po zobrazeni sa kontrola zopakuje
    private bool _analysisStale = true;

    /// <summary>
    /// Presunie strom a dokument z navrhu okna do panelov; rozlozenie z nastaveni sa obnovi az po teme a pismach.
    /// </summary>
    private void InitDocking()
    {
        SuspendLayout();
        Controls.Remove(tvData);
        Controls.Remove(pDocument);
        tvData.Dock = DockStyle.Fill;
        // ram kresli panel, okraj stromu by bol druhy
        tvData.BorderStyle = BorderStyle.None;
        _installationPanel.Controls.Add(tvData);
        _analysisView.Dock = DockStyle.Fill;
        _analysisPanel.Controls.Add(_analysisView);
        _dock.Document.Controls.Add(pDocument);

        _dock.AddPanel(_installationPanel, ExDockSide.Left);
        _dock.AddPanel(_analysisPanel, ExDockSide.Bottom);
        _dock.SetZoneSize(ExDockSide.Left, 260);
        _dock.SetZoneSize(ExDockSide.Bottom, 190);
        Controls.Add(_dock);
        // pred panelmi ukotvenymi k okraju (zahlavie, panel nastrojov, stavovy riadok), aby vyplnil zvysok okna
        Controls.SetChildIndex(_dock, 0);
        ResumeLayout(true);

        _defaultLayout = _dock.SaveLayout();
        _dock.PanelStateChanged += (_, e) =>
        {
            if (e.Panel == _analysisPanel && _analysisPanel.IsShown && _analysisStale)
                RefreshAnalysis();
        };
        GridColumns.Attach(_analysisView.Grid, "Findings", () => _ctx.Config, SaveConfig);
    }

    private void CreateViewCommands()
    {
        _commands.Add(GvxCommands.ViewInstallation, () => _dock.ShowPanel(_installationPanel)).Bind(tsmimViewInstallation);
        _commands.Add(GvxCommands.ViewAnalysis, () => _dock.ShowPanel(_analysisPanel)).Bind(tsmimViewAnalysis);
        _commands.Add(GvxCommands.ResetLayout, () => _dock.LoadLayout(_defaultLayout)).Bind(tsmimResetLayout);
    }

    /// <summary>
    /// Po zmene instalacie zopakuje kontrolu (skryty panel az po zobrazeni).
    /// </summary>
    private void ScheduleAnalysis()
    {
        if (_analysisPanel.IsShown)
            _analysisView.ScheduleRefresh();
        else
            _analysisStale = true;
    }

    private void RefreshAnalysis()
    {
        _analysisStale = false;
        _analysisView.RunChecks();
    }

    // rozlozenie panelov sa uklada pri zatvoreni okna spolu s ostatnymi nastaveniami
    private void SaveDockLayout()
    {
        _ctx.Config.DockLayout = _dock.SaveLayout();
        SaveConfig();
    }
}
