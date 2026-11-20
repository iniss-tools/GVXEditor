using GVXEditor.Domain;
using GVXEditor.Mcp;
using ModelContextProtocol;

namespace GVXEditor.UI.Main;

/// <summary>
/// Hlavne okno ako hostitel zmien agenta MCP: zmena len pri volnom okne, potom krok spat a obnovene zobrazenie.
/// </summary>
internal partial class FMain : IAgentHost
{
    /// <inheritdoc />
    void IAgentHost.EnsureIdle()
    {
        // otvorene okno uprav alebo dialog hlavne okno vypne (CanFocus pozna aj systemove dialogy), overovanie kodom
        // servera ho vypne s cakacim kurzorom
        if (!CanFocus || !Enabled || UseWaitCursor)
            throw new McpException("editor.busy: GVXEditor has a window open or is busy; ask the user to close it, then try again.");
    }

    /// <inheritdoc />
    void IAgentHost.Change(WorkspaceState state, string step)
    {
        if (_ctx.Workspace is not { } workspace)
            return;
        workspace.ChangeState(state, step);
        RefreshView();
    }
}
