using System.ComponentModel;
using GVXEditor.Domain;
using GVXEditor.Domain.Analysis;
using GVXEditor.Domain.Server;
using GVXEditor.Mcp.Results;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using ToolsCore.Iniss2;

namespace GVXEditor.Mcp;

/// <summary>Nastroje MCP nad celou otvorenou instalaciou: prehlad, kontrola a overenie kodom servera.</summary>
[McpServerToolType]
internal static class InstallationTools
{
    [McpServerTool(Name = "get_installation", Title = "Installation", ReadOnly = true, Idempotent = true, OpenWorld = false, UseStructuredContent = true)]
    [Description("The INISS2 installation open in GVXEditor: data folder, whether its server runs, unsaved files, timetable groups with their "
                 + "stations. Call it first - group and station names are parameters of the other tools.")]
    public static Task<InstallationInfo> GetInstallation(EditorAccess editor) =>
        editor.ReadAsync(context =>
        {
            var workspace = McpGuards.Workspace(context);
            var running = workspace.IsServerRunning();
            var server = running ? "running" : Iniss2Server.IsRunning() ? "stoppedOtherRunning" : "stopped";
            var unsaved = workspace.UnsavedFiles();
            var groups = workspace.Groups.Select(group => new GroupInfo(group.Group.Name, group.Timetable?.Trains.Count ?? 0, group.Error,
                group.Stations.Select(station => new StationInfo(station.Station.Station, workspace.StationName(station.Station.Station),
                    station.Station.Directory)).ToList())).ToList();
            var labels = workspace.History.UndoLabels;
            return new InstallationInfo(workspace.DataDir, server, unsaved.Count > 0, unsaved, labels.Count > 0 ? labels[0] : null,
                ServerVerifier.IsAvailable(workspace), groups);
        });

    [McpServerTool(Name = "check_installation", Title = "Check installation", ReadOnly = true, Idempotent = true, OpenWorld = false,
        UseStructuredContent = true)]
    [Description("The installation check of GVXEditor over the open installation including unsaved changes: errors that would stop the "
                 + "server or a file from loading, warnings where the server uses the data differently than expected and information "
                 + "such as station records no train uses. Fast; it does not compile the VB.NET code - use verify_with_server for that.")]
    public static Task<FindingList> CheckInstallation(EditorAccess editor) =>
        editor.ReadAsync(context =>
        {
            var workspace = McpGuards.Workspace(context);
            return Findings(workspace, InstallationChecks.Run(workspace));
        });

    [McpServerTool(Name = "verify_with_server", Title = "Verify with server code", ReadOnly = true, Idempotent = true, OpenWorld = false,
        UseStructuredContent = true)]
    [Description("Loads a temporary copy of the installation with unsaved changes using the IniSrv.exe of the installation, exactly as the "
                 + "server does when it starts: every file by the server's reader and the VB.NET code (state diagrams, LogText.vb, TabTab, "
                 + "board conditions) by its compiler. Returns errors that would stop the server (with the line and place of compile "
                 + "errors) and warnings the server writes to Error.log. Takes a few seconds; never writes to the installation.")]
    public static async Task<FindingList> VerifyWithServer(EditorAccess editor)
    {
        var workspace = await editor.ReadAsync(context =>
        {
            var workspace = McpGuards.Workspace(context);
            return ServerVerifier.IsAvailable(workspace)
                ? workspace
                : throw new McpException("verify.unavailable: The installation has no IniSrv.exe or GVXEditor lacks IniSrvBridge; use check_installation.");
        }).ConfigureAwait(false);

        try
        {
            var verification = await Task.Run(() => ServerVerifier.Run(workspace)).ConfigureAwait(false);
            return await editor.ReadAsync(_ => Findings(workspace, ServerFindings.Of(verification))).ConfigureAwait(false);
        }
        catch (ServerVerificationException e)
        {
            throw new McpException($"verify.failed: The server code could not be run: {e.Message}");
        }
    }

    private static FindingList Findings(Iniss2Workspace workspace, List<Finding> findings)
    {
        var ordered = findings.OrderBy(finding => finding.Severity).ToList();
        return new FindingList(
            ordered.Count(finding => finding.Severity == FindingSeverity.Error),
            ordered.Count(finding => finding.Severity == FindingSeverity.Warning),
            ordered.Count(finding => finding.Severity == FindingSeverity.Info),
            ordered.Select(finding => new FindingInfo(
                finding.Severity switch { FindingSeverity.Error => "error", FindingSeverity.Warning => "warning", _ => "info" },
                finding.Place, finding.Message,
                finding.Group >= 0 && finding.Group < workspace.Groups.Count ? workspace.Groups[finding.Group].Group.Name : null,
                finding.Train >= 0 ? finding.Train : null)).ToList());
    }
}
