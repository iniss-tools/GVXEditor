using System.Globalization;
using GVXEditor.Domain.Analysis;
using GVXEditor.Domain.Server;
using GVXEditor.Properties;
using Serilog;
using ToolsCore.Icons;

namespace GVXEditor.UI.Analysis;

/// <summary>
/// Panel Kontrola instalacie v hlavnom okne (ako Zoznam chyb vo Visual Studiu): zistenia aktualneho stavu vratane
/// neulozenych zmien, filtre zavaznosti s poctom, opakovanie kontroly a overenie kodom servera. Dvojklik alebo Enter
/// prejde na grafikon a vlak zistenia. Po zmene instalacie hlavne okno kontrolu zopakuje (<see cref="ScheduleRefresh" />).
/// </summary>
internal partial class AnalysisView : UserControl
{
    private readonly EditorContext _ctx;
    private readonly Action<Finding> _navigate;

    // kontrola az po chvili bez dalsej zmeny (pisanie v okne, viac krokov za sebou)
    private readonly System.Windows.Forms.Timer _refreshTimer = new() { Interval = 400 };
    private List<Finding> _findings = [];
    private bool _verifying;

    /// <param name="context">kontext editora</param>
    /// <param name="navigate">prechod na miesto zistenia v hlavnom okne</param>
    public AnalysisView(EditorContext context, Action<Finding> navigate)
    {
        _ctx = context;
        _navigate = navigate;
        InitializeComponent();
        components?.Add(_refreshTimer);
        dgvFindings.AutoGenerateColumns = false;
        _refreshTimer.Tick += (_, _) => RunChecks();
        ShowFindings([], false);
    }

    /// <summary>
    /// Tabulka zisteni (sirky a poradie stlpcov si pamata hlavne okno).
    /// </summary>
    public DataGridView Grid => dgvFindings;

    /// <summary>
    /// Pocet zisteni podla zavaznosti (vratane skrytych filtrom).
    /// </summary>
    public int Count(FindingSeverity severity) => _findings.Count(finding => finding.Severity == severity);

    /// <summary>
    /// Zopakuje kontrolu po chvili bez dalsej zmeny; vysledok overenia kodom servera tym prestane platit.
    /// </summary>
    public void ScheduleRefresh()
    {
        if (_verifying)
            return;
        _refreshTimer.Stop();
        _refreshTimer.Start();
    }

    /// <summary>
    /// Skontroluje instalaciu hned.
    /// </summary>
    public void RunChecks()
    {
        _refreshTimer.Stop();
        if (_verifying)
            return;

        if (_ctx.Workspace is not { } workspace)
        {
            ShowFindings([], false);
            return;
        }

        ShowFindings(InstallationChecks.Run(workspace), false);
    }

    /// <summary>
    /// Kontrola a overenie kodom servera (na pozadi, par sekund).
    /// </summary>
    public async Task VerifyServerAsync()
    {
        _refreshTimer.Stop();
        if (_verifying || _ctx.Workspace is not { } workspace)
            return;

        Log.Information("Kontrola instalacie {DataDir} s overenim kodom servera", workspace.DataDir);
        var findings = InstallationChecks.Run(workspace);
        if (!ServerVerifier.IsAvailable(workspace))
        {
            findings.Add(new Finding(FindingSeverity.Info, -1, -1, Resources.Place_Server, Resources.Check_ServerMissing));
            ShowFindings(findings, false);
            return;
        }

        SetVerifying(true);
        try
        {
            findings.AddRange(ServerFindings.Of(await Task.Run(() => ServerVerifier.Run(workspace))));
        }
        catch (ServerVerificationException)
        {
            findings.Add(new Finding(FindingSeverity.Warning, -1, -1, Resources.Place_Server, Resources.Check_ServerFailed));
        }
        finally
        {
            SetVerifying(false);
        }

        // instalacia sa medzitym mohla zavriet alebo zmenit - vysledok plati len pre tu istu
        if (!IsDisposed && ReferenceEquals(_ctx.Workspace, workspace))
            ShowFindings(findings, true);
    }

    private void SetVerifying(bool verifying)
    {
        _verifying = verifying;
        tsbVerify.Enabled = tsbRefresh.Enabled = !verifying;
        UseWaitCursor = verifying;
        tslStatus.Text = verifying ? Resources.Main_Verifying : "";
    }

    private void ShowFindings(List<Finding> findings, bool withServer)
    {
        _findings = findings;
        tsbErrors.Text = string.Format(CultureInfo.CurrentCulture, Resources.Analysis_Errors, Count(FindingSeverity.Error));
        tsbWarnings.Text = string.Format(CultureInfo.CurrentCulture, Resources.Analysis_Warnings, Count(FindingSeverity.Warning));
        tsbInfos.Text = string.Format(CultureInfo.CurrentCulture, Resources.Analysis_Infos, Count(FindingSeverity.Info));
        tslStatus.Text = withServer ? Resources.Analysis_WithServer : "";
        Fill();
    }

    // riadky podla filtrov; vybrane zistenie ostane vybrane, ak po kontrole ostalo
    private void Fill()
    {
        var selected = dgvFindings.CurrentRow?.DataBoundItem is FindingRow current ? current.Finding : null;
        var rows = _findings.Where(PassesFilter).OrderBy(finding => finding.Severity).Select(finding => new FindingRow(finding)).ToList();
        dgvFindings.DataSource = rows;

        var index = selected is null ? -1 : rows.FindIndex(row => row.Finding == selected);
        if (index >= 0)
            dgvFindings.CurrentCell = dgvFindings.Rows[index].Cells[cPlace.Index];
        else
            dgvFindings.ClearSelection();
    }

    private bool PassesFilter(Finding finding) => finding.Severity switch
    {
        FindingSeverity.Error => tsbErrors.Checked,
        FindingSeverity.Warning => tsbWarnings.Checked,
        _ => tsbInfos.Checked
    };

    private void Navigate(int rowIndex)
    {
        if (rowIndex >= 0 && dgvFindings.Rows[rowIndex].DataBoundItem is FindingRow row && row.Finding.Group >= 0)
            _navigate(row.Finding);
    }

    // zavaznost ikonou ako v GVDEditore, slovom v popise bunky
    private void dgvFindings_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex != cSeverity.Index || dgvFindings.Rows[e.RowIndex].DataBoundItem is not FindingRow row)
            return;
        e.Value = IconImages.For(dgvFindings, row.SeverityIcon);
        dgvFindings.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = row.Severity;
    }

    private void dgvFindings_CellDoubleClick(object? sender, DataGridViewCellEventArgs e) => Navigate(e.RowIndex);

    private void dgvFindings_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter || dgvFindings.CurrentRow is not { } row)
            return;
        e.Handled = true;
        Navigate(row.Index);
    }

    private void Filter_CheckedChanged(object? sender, EventArgs e) => Fill();

    private void tsbRefresh_Click(object? sender, EventArgs e) => RunChecks();

    private async void tsbVerify_Click(object? sender, EventArgs e) => await VerifyServerAsync();
}
