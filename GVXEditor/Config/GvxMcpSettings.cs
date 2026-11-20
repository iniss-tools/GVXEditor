using ToolsCore.Mcp;

namespace GVXEditor.Config;

/// <summary>
/// Server MCP pre agentov (Claude Code, VS Code) - bezi v GVXEditore na tomto pocitaci a pristupuje sa k nemu klucom.
/// Port a zaciatok kluca sa lisia od GVDEditora, oba programy mozu bezat naraz.
/// </summary>
public sealed class GvxMcpSettings() : McpSettings("GVXEditor", "gvxeditor", Prefix, DefaultServerPort)
{
    /// <summary>Predvoleny port servera.</summary>
    public const int DefaultServerPort = 5491;

    /// <summary>Zaciatok kluca - odlisi ho od klucov GVDEditora (<c>gvd_</c>) a simulatora (<c>ts_</c>).</summary>
    public const string Prefix = "gvx_";
}
