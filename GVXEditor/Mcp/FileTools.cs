using System.ComponentModel;
using GVXEditor.Mcp.Results;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using ToolsCore.Iniss2.Grafikon;
using ToolsCore.Iniss2.Grafikon.Script;

namespace GVXEditor.Mcp;

/// <summary>Nastroje MCP nad subormi dat instalacie tak, ako by ich GVXEditor ulozil.</summary>
[McpServerToolType]
internal static class FileTools
{
    private const int MaxLines = 5000;

    [McpServerTool(Name = "get_file", Title = "Data file", ReadOnly = true, Idempotent = true, OpenWorld = false, UseStructuredContent = true)]
    [Description("Text of a data file of the installation as GVXEditor would save it now - with the editor's unsaved changes; files without "
                 + "changes are read from disk. The path is relative to the data folder, e.g. 'ConstReports.xml', 'LogText.vb', "
                 + "'2030/GV.XML', '2030/Default.Stations.xml' or '2030/01 Lipovec/statedgm.xml' (group and station folders from "
                 + "get_installation). Long files are returned in pages of lines.")]
    public static Task<FileContent> GetFile(
        EditorAccess editor,
        [Description("Path relative to the data folder.")] string path,
        [Description("First line to return, from 0.")] int offset = 0,
        [Description("Most lines to return, 1-5000.")] int limit = 1000) =>
        editor.ReadAsync(context =>
        {
            if (limit is < 1 or > MaxLines)
                throw new McpException($"file.limit: The limit must be 1 to {MaxLines}.");
            var workspace = McpGuards.Workspace(context);
            var relative = path.Trim().Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar);
            var root = Path.GetFullPath(workspace.DataDir) + Path.DirectorySeparatorChar;
            var full = Path.GetFullPath(Path.Combine(workspace.DataDir, relative));
            if (relative.Length == 0 || !full.StartsWith(root, StringComparison.OrdinalIgnoreCase))
                throw new McpException("file.outside: The path must be a file inside the data folder.");

            var (text, changed) = workspace.CurrentText(Path.GetRelativePath(workspace.DataDir, full));
            if (text is null)
                return new FileContent(path, false, false, 0, 0, "");
            var lines = text.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n');
            var start = Math.Clamp(offset, 0, lines.Length);
            return new FileContent(path, true, changed, lines.Length, start, string.Join("\n", lines.Skip(start).Take(limit)));
        });

    [McpServerTool(Name = "list_script_functions", Title = "Announcement script functions", ReadOnly = true, Idempotent = true, OpenWorld = false,
        UseStructuredContent = true)]
    [Description("Functions of class LogText in the announcement script LogText.vb (with unsaved changes): names, attributes such as Dodatek "
                 + "or VzorVyluky with their descriptions and line ranges. Read a function with get_file('LogText.vb', offset, limit).")]
    public static Task<ScriptFunctions> ListScriptFunctions(EditorAccess editor) =>
        editor.ReadAsync(context =>
        {
            var workspace = McpGuards.Workspace(context);
            var text = workspace.History.Current.LogText ?? LogTextFile.ReadText(workspace.DataDir)
                       ?? throw new McpException("script.missing: The installation has no LogText.vb.");
            var script = LogTextScript.Parse(text);
            return new ScriptFunctions(script.Lines.Count, script.Members.Select(member => new ScriptFunction(member.Name, member.Keyword,
                [.. member.Attributes.Select(attribute => attribute.Name)],
                member.Attributes.Select(attribute => attribute.Description).FirstOrDefault(description => !string.IsNullOrEmpty(description)),
                member.Start + 1, member.End + 1)).ToList());
        });
}
