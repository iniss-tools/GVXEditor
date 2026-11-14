using GVXEditor.Domain;
using GVXEditor.Domain.Code;
using GVXEditor.Domain.Server;
using Serilog;

namespace GVXEditor.UI.Code;

/// <summary>
/// Preklad kodu kodom servera s oneskorenim: po poslednej zmene v okne sa stav s konceptom okna zapise do docasnej kopie
/// dat a most ho prelozi. Starsi vysledok, ktory dobehne po novej zmene, sa zahodi.
/// </summary>
internal sealed class CodeCheck : IDisposable
{
    private readonly Iniss2Workspace _workspace;
    private readonly Control _owner;
    private readonly Func<WorkspaceState> _state;
    private readonly Action<CodeCheckResult> _done;
    private readonly System.Windows.Forms.Timer _timer = new() { Interval = 1500 };
    private int _version;
    private bool _running;

    /// <param name="workspace">otvorena instalacia</param>
    /// <param name="owner">okno - vysledok pride na jeho vlakne, po zatvoreni sa zahodi</param>
    /// <param name="state">stav instalacie s konceptom okna (vola sa na vlakne okna)</param>
    /// <param name="done">vysledok prekladu na vlakne okna</param>
    public CodeCheck(Iniss2Workspace workspace, Control owner, Func<WorkspaceState> state, Action<CodeCheckResult> done)
    {
        _workspace = workspace;
        _owner = owner;
        _state = state;
        _done = done;
        _timer.Tick += (_, _) => _ = RunAsync();
    }

    /// <summary>Preklad je mozny: instalacia ma program servera a most je pri programe.</summary>
    public bool IsAvailable => ServerVerifier.IsAvailable(_workspace);

    /// <summary>Preklad prave bezi alebo caka na koniec pisania.</summary>
    public bool IsBusy => _running || _timer.Enabled;

    /// <summary>
    /// Naplanuje preklad po chvili bez dalsej zmeny.
    /// </summary>
    public void Schedule()
    {
        if (!IsAvailable)
            return;
        _version++;
        _timer.Stop();
        _timer.Start();
    }

    private async Task RunAsync()
    {
        _timer.Stop();
        if (_running)
        {
            // zmena pocas prekladu - preklad sa zopakuje po dobehnuti
            _timer.Start();
            return;
        }

        _running = true;
        var version = _version;
        CodeCheckResult result;
        try
        {
            var state = _state();
            var verification = await Task.Run(() => ServerVerifier.RunCode(_workspace, state)).ConfigureAwait(true);
            result = new CodeCheckResult([.. verification.CompileErrors.Select(CodeErrorPlace.Of)], verification.Fatal, verification.Stage, null);
        }
        catch (ServerVerificationException e)
        {
            Log.Warning(e, "Preklad kodu instalacie {DataDir} mostom zlyhal", _workspace.DataDir);
            result = new CodeCheckResult([], null, null, e.Message);
        }
        finally
        {
            _running = false;
        }

        if (!_owner.IsDisposed && version == _version)
            _done(result);
    }

    /// <inheritdoc />
    public void Dispose() => _timer.Dispose();
}
