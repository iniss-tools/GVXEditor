using System.Reflection;
using GVXEditor.Config;
using Microsoft.Extensions.DependencyInjection;
using ToolsCore.Mcp;

namespace GVXEditor.Mcp;

/// <summary>
/// Server MCP vnutri GVXEditora (spolocny server <see cref="McpServerHost" />) - agent cita otvorenu instalaciu INISS2
/// aj s neulozenymi zmenami cez <see cref="EditorAccess" />.
/// </summary>
internal sealed class GvxMcpServer : IMcpServerStatus, IDisposable
{
    /// <summary>Pokyny pre agenta pri pripojeni.</summary>
    private const string Instructions =
        "GVXEditor edits the data of an INISS2 server (IniSrv) installation - the station information system of Slovak and Czech railways "
        + "(departure boards and announcements). An installation has timetable groups (grafikon) from DirList.xml, each with one GV.XML "
        + "(trains with variants and routes) and station folders with station files; the server reads all files only when it starts. "
        + "Tools read the installation open in the running editor including unsaved changes: get_installation first, then list_trains "
        + "and get_train (a train is identified by its index in GV.XML), station_day for the trains a station has on a day as the server "
        + "sees them, get_file for any data file with the editor's changes. File formats are documented at "
        + "https://iniss-tools.fvat.sk/docs/category/iniss2-formaty-suborov - read them instead of guessing. Validity texts are Czech date remarks "
        + "like 'jede v 6,7' (explain_validity expands them to days). check_installation and verify_with_server find problems that "
        + "would stop the server or make it use the data differently.";

    /// <summary>Pokyny podla povolenia zmien.</summary>
    private const string ReadOnlyInstructions = " Changes are made by the user in GVXEditor, which writes the files only while the server is stopped.";

    private const string WriteInstructions =
        " Tools that change data (trains of GV.XML, operator reports, generated board texts) work in the editor's memory only: each call "
        + "is one undo step the user can revert in GVXEditor and nothing is saved - the user reviews the changes and saves them while the "
        + "server is stopped. They apply the checks of the editor's windows: errors refuse the change, warnings are returned. Call a change "
        + "with dryRun first when its effect is not obvious; when the editor has a window open the change is refused with editor.busy.";

    private readonly McpServerHost _host =
        new(new McpServerDefinition("GVXEditor", Instructions, ReadOnlyInstructions, WriteInstructions, typeof(GvxMcpServer).Assembly));

    /// <inheritdoc />
    public string? Address => _host.Address;

    /// <inheritdoc />
    public string? Error => _host.Error;

    /// <inheritdoc />
    public McpSettings? Settings => _host.Settings;

    /// <summary>
    /// Server podla <paramref name="settings" />: zastavi beziaci a pri zapnutom nastaveni spusti novy. Chybu nehadze -
    /// je v <see cref="Error" /> (GVXEditor bez servera funguje dalej).
    /// </summary>
    public Task ApplyAsync(EditorAccess editor, GvxMcpSettings settings) =>
        _host.ApplyAsync(settings, services => services.AddSingleton(editor));

    /// <summary>Staticke metody nastrojov GVXEditora.</summary>
    public static IReadOnlyList<MethodInfo> ToolMethods() => McpServerHost.ToolMethods(typeof(GvxMcpServer).Assembly);

    /// <summary>Nastroj data nemeni.</summary>
    public static bool IsReadOnly(MethodInfo method) => McpServerHost.IsReadOnly(method);

    public void Dispose() => _host.Dispose();
}
