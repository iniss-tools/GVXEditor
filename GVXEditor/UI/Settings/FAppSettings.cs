using GVXEditor.Config;
using GVXEditor.Properties;
using ToolsCore.Commands;
using ToolsCore.Forms;
using ToolsCore.Iniss.Tools;
using ToolsCore.Mcp;
using ToolsCore.XML;

namespace GVXEditor.UI.Settings;

/// <summary>
/// Nastavenia programu - spolocne stranky ToolsCore a stranka Agenti (MCP).
/// </summary>
internal partial class FAppSettings : FAppSettingsBase
{
    private readonly EditorContext _ctx;
    private readonly IMcpServerStatus? _mcp;

    private new GVXEditorConfig Config => (GVXEditorConfig)base.Config;

    private new Styles<GVXEditorStyle> Styles => (Styles<GVXEditorStyle>)base.Styles;

    /// <inheritdoc />
    protected override string ReopenLastText => Resources.Settings_ReopenLast;

    /// <inheritdoc />
    protected override string EditorFontText => Resources.Settings_EditorFontHint;

    /// <param name="context">kontext editora</param>
    /// <param name="commands">prikazy hlavneho okna - strom skratiek podla jeho ponuky; null = podla katalogu</param>
    /// <param name="mcp">beziaci server MCP - jeho stav sa ukaze na stranke Agenti; null = bez stavu (harness snimok)</param>
    public FAppSettings(EditorContext context, IEnumerable<AppCommand>? commands = null, IMcpServerStatus? mcp = null)
        : base(context.Config, new Styles<GVXEditorStyle>(context.Session.Styles), context.UsingStyle)
    {
        _ctx = context;
        _mcp = mcp;
        InitializeComponent();
        mcpPage.HintText = Resources.Settings_McpHint;
        mcpPage.WritesText = Resources.Settings_McpWrites;
        mcpPage.WritesHintText = Resources.Settings_McpWritesHint;
        SetCommands(GvxCommands.All, commands);
    }

    /// <inheritdoc />
    protected override void OnLoad()
    {
        base.OnLoad();
        mcpPage.LoadSettings(Config.McpServer, _mcp);
    }

    /// <inheritdoc />
    protected override bool OnSaving()
    {
        mcpPage.SaveSettings();
        return base.OnSaving();
    }

    /// <inheritdoc />
    protected override void SaveData()
    {
        // kopie - okno po Pouzit ostava otvorene a dalsie upravy v nom nesmu menit nastavenia programu
        _ctx.Session.Config = Config with { };
        _ctx.Session.Styles = new Styles<GVXEditorStyle>(Styles);
        _ctx.Session.UsingStyle = _ctx.Session.Styles[UsingStyle.Name];

        var configsDir = ToolsCore.AppPaths.ConfigDir;
        Directory.CreateDirectory(configsDir);
        Styles<GVXEditorStyle>.WriteData(PathUtils.CombinePath(configsDir, ToolsCore.FileConsts.FileStyles)!, _ctx.Session.Styles);
        XmlSerialization.WriteData(PathUtils.CombinePath(configsDir, ToolsCore.FileConsts.FileConfig)!, _ctx.Config);
    }
}
