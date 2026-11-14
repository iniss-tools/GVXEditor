using GVXEditor.Domain.Server;
using GVXEditor.Properties;
using ToolsCore.Tools;

namespace GVXEditor.UI.Main;

// overenie kodom servera pred ulozenim (most IniSrvBridge, par sekund)
internal partial class FMain
{
    private void VerifyServer()
    {
        _dock.ShowPanel(_analysisPanel);
        _analysisStale = false;
        _ = _analysisView.VerifyServerAsync();
    }

    /// <summary>
    /// Overi neulozene zmeny kodom servera na pozadi; okno je zatial nedostupne. Bez mosta alebo IniSrv.exe vrati
    /// <see langword="null" />, rovnako pri zlyhani mosta (s upozornenim).
    /// </summary>
    private async Task<ServerVerification?> VerifyAsync()
    {
        if (_ctx.Workspace is not { } workspace || !ServerVerifier.IsAvailable(workspace))
            return null;

        SetBusy(true);
        try
        {
            return await Task.Run(() => ServerVerifier.Run(workspace));
        }
        catch (ServerVerificationException)
        {
            Utils.ShowWarning(Resources.Check_ServerFailed);
            return null;
        }
        finally
        {
            SetBusy(false);
        }
    }

    // pri zatvarani okna sa neda cakat asynchronne - overenie bezi s kurzorom cakania
    private ServerVerification? VerifyBlocking()
    {
        if (_ctx.Workspace is not { } workspace || !ServerVerifier.IsAvailable(workspace))
            return null;

        SetBusy(true);
        try
        {
            return ServerVerifier.Run(workspace);
        }
        catch (ServerVerificationException)
        {
            Utils.ShowWarning(Resources.Check_ServerFailed);
            return null;
        }
        finally
        {
            SetBusy(false);
        }
    }

    private void SetBusy(bool busy)
    {
        UseWaitCursor = busy;
        Enabled = !busy;
        tsslCount.Text = busy ? Resources.Main_Verifying : "";
        // po overeni sa vrati pocet vlakov zoznamu
        if (!busy)
            UpdateTrainCount();
        Application.DoEvents();
    }
}
