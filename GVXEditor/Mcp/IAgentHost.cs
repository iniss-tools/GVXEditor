using GVXEditor.Domain;

namespace GVXEditor.Mcp;

/// <summary>Hlavne okno GVXEditora pri zmene instalacie agentom.</summary>
internal interface IAgentHost
{
    /// <summary>
    /// Ak sa instalacia teraz menit neda (otvorene okno uprav, dialog, overovanie), hodi
    /// <see cref="ModelContextProtocol.McpException" /> s kodom <c>editor.busy</c> - rozpisana uprava v okne by sa inak
    /// po jeho potvrdeni stratila alebo prepisala zmenu agenta.
    /// </summary>
    void EnsureIdle();

    /// <summary>Novy stav ako krok spat s popisom <paramref name="step" />, potom obnovi zobrazenie.</summary>
    void Change(WorkspaceState state, string step);
}
