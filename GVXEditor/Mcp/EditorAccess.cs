using System.Globalization;
using GVXEditor.Domain;
using GVXEditor.Mcp.Results;
using GVXEditor.Properties;
using ModelContextProtocol;
using Serilog;

namespace GVXEditor.Mcp;

/// <summary>
/// Pristup nastrojov MCP k otvorenej instalacii: kazde citanie aj zmena bezi na vlakne hlavneho okna, kde stav meni aj
/// pouzivatel, takze nastroj nikdy nevidi polovicnu upravu. Bez okna (<paramref name="owner" /> null, testy) pristupuje
/// priamo; bez <paramref name="host" /> sa zmena nekontroluje voci oknu a nevzniknu udalosti okna.
/// </summary>
internal sealed class EditorAccess(EditorContext context, Control? owner, IAgentHost? host = null)
{
    /// <summary>Popis parametra <c>dryRun</c> nastrojov so zmenou.</summary>
    public const string DryRunDescription = "Only report the result; the installation stays as it is.";

    /// <summary>Vysledok <paramref name="read" /> nad kontextom editora z vlakna okna.</summary>
    public Task<T> ReadAsync<T>(Func<EditorContext, T> read)
    {
        if (owner is null)
            return Task.FromResult(read(context));
        var result = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        if (owner.IsDisposed || !owner.IsHandleCreated)
        {
            result.SetException(new McpException("editor.closed: GVXEditor is closing."));
            return result.Task;
        }

        owner.BeginInvoke(() =>
        {
            try
            {
                result.SetResult(read(context));
            }
            catch (Exception e)
            {
                result.SetException(e);
            }
        });
        return result.Task;
    }

    /// <summary>
    /// Zmena otvorenej instalacie agentom: <paramref name="change" /> z aktualneho stavu vypocita novy (stav je nemenny, preto
    /// chyba nic neporusi). Zmena = jeden krok spat „popis (agent)“ v GVXEditore, nic sa neuklada; stav bez zmeny musi byt
    /// ten isty objekt. Pri skuske nanecisto (<paramref name="dryRun" />) sa stav nezmeni a vysledok je rovnaky.
    /// </summary>
    public Task<(T Result, ChangeInfo Change)> WriteAsync<T>(bool dryRun, Func<Iniss2Workspace, (WorkspaceState State, string Label, T Result)> change) =>
        ReadAsync(_ =>
        {
            var workspace = McpGuards.Workspace(context);
            // rozpisana uprava v okne by po potvrdeni prepisala zmenu agenta - aj skuska nanecisto by klamala
            host?.EnsureIdle();
            var current = workspace.History.Current;
            var (state, label, result) = change(workspace);
            var changed = !ReferenceEquals(state, current);
            if (!changed || dryRun)
                return (result, new ChangeInfo(dryRun, changed, null));

            var step = string.Format(CultureInfo.CurrentCulture, Resources.History_Agent, label);
            Log.Information("MCP: agent zmenil instalaciu {DataDir} - {Step}", workspace.DataDir, step);
            if (host is null)
                workspace.ChangeState(state, step);
            else
                host.Change(state, step);
            return (result, new ChangeInfo(false, true, step));
        });
}
