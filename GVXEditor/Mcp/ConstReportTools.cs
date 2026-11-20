using System.ComponentModel;
using System.Globalization;
using GVXEditor.Domain;
using GVXEditor.Domain.Rules;
using GVXEditor.Domain.Setup;
using GVXEditor.Domain.Setup.Drafts;
using GVXEditor.Mcp.Results;
using GVXEditor.Properties;
using ModelContextProtocol;
using ModelContextProtocol.Server;

namespace GVXEditor.Mcp;

/// <summary>
/// Nastroje MCP so zmenou hlaseni obsluhy (ConstReports.xml, spolocne pre instalaciu) s pravidlami okna Hlasenia obsluhy.
/// </summary>
[McpServerToolType]
internal static class ConstReportTools
{
    [McpServerTool(Name = "set_const_report", Title = "Set operator report", ReadOnly = false, Destructive = true, Idempotent = true,
        OpenWorld = false, UseStructuredContent = true)]
    [Description("Changes or adds an operator report of ConstReports.xml (shared by the installation, read it with get_file). Supplements, "
                 + "lockouts and diversions of trains refer to it by number. The pattern is a function of LogText.vb, usually with "
                 + "attribute VzorDodatku, VzorOdklonu or VzorVyluky (list_script_functions); stations are passed to the pattern, for a "
                 + "lockout they are its section. A change that would add an error of the Operator reports window is refused, warnings "
                 + "are returned.")]
    public static async Task<ConstReportChangeResult> SetConstReport(
        EditorAccess editor,
        [Description("Report name for the operator.")] string name,
        [Description("Pattern: function name in LogText.vb.")] string function,
        [Description("Station numbers SR70 passed to the pattern, in order.")] IReadOnlyList<int>? stations = null,
        [Description("Report number; null = a new report after the highest number, like the server adds it.")] int? number = null,
        [Description(EditorAccess.DryRunDescription)] bool dryRun = false)
    {
        var (result, change) = await editor.WriteAsync(dryRun, workspace =>
        {
            var edit = ConstReportsEdit.Of(Reports(workspace));
            var key = number ?? edit.NextKey();
            var draft = edit.Reports.FirstOrDefault(report => report.Number == key);
            if (draft is null)
            {
                draft = new ConstReportDraft { Key = key.ToString(CultureInfo.InvariantCulture) };
                edit.Reports.Add(draft);
            }

            draft.Name = name.Trim();
            draft.Function = function.Trim();
            draft.Stations = string.Join(" ", (stations ?? []).Select(station => station.ToString(CultureInfo.InvariantCulture)));
            return Apply(workspace, edit, key);
        }).ConfigureAwait(false);
        return result with { Change = change };
    }

    [McpServerTool(Name = "delete_const_report", Title = "Delete operator report", ReadOnly = false, Destructive = true, Idempotent = true,
        OpenWorld = false, UseStructuredContent = true)]
    [Description("Deletes an operator report from ConstReports.xml. A warning tells when supplements, lockouts or diversions of trains still "
                 + "refer to its number - the server would then fail to compose that announcement.")]
    public static async Task<ConstReportChangeResult> DeleteConstReport(
        EditorAccess editor,
        [Description("Report number.")] int number,
        [Description(EditorAccess.DryRunDescription)] bool dryRun = false)
    {
        var (result, change) = await editor.WriteAsync(dryRun, workspace =>
        {
            var edit = ConstReportsEdit.Of(Reports(workspace));
            var draft = edit.Reports.FirstOrDefault(report => report.Number == number)
                        ?? throw new McpException($"report.notFound: Operator report {number} does not exist; read ConstReports.xml with get_file.");
            edit.Reports.Remove(draft);
            return Apply(workspace, edit, number);
        }).ConfigureAwait(false);
        return result with { Change = change };
    }

    private static IReadOnlyList<ToolsCore.Iniss2.Grafikon.ConstReport> Reports(Iniss2Workspace workspace) =>
        workspace.History.Current.ConstReports ?? throw new McpException("report.missing: The installation has no ConstReports.xml that could be read.");

    // zmena nesmie pridat chybu pravidiel okna (starsia chyba v inom hlaseni ju nezastavi)
    private static (Domain.WorkspaceState, string, ConstReportChangeResult) Apply(Iniss2Workspace workspace, ConstReportsEdit edit, int number)
    {
        var used = ConstReportsEdit.UsedKeys(workspace.Groups);
        var before = SetupRules.Check(ConstReportsEdit.Of(edit.Original), workspace.Functions, used)
            .Where(problem => problem.IsError).Select(problem => problem.Message).ToHashSet();
        var problems = SetupRules.Check(edit, workspace.Functions, used);
        var added = problems.Where(problem => problem.IsError && !before.Contains(problem.Message)).ToList();
        if (added.Count > 0)
            throw new McpException("report.invalid: " + string.Join(" ", added.Select(problem => problem.Message)));

        var reports = edit.Apply();
        var state = ReferenceEquals(reports, edit.Original) ? workspace.History.Current : workspace.History.Current with { ConstReports = reports };
        var warnings = problems.Where(problem => !problem.IsError).Select(problem => new ProblemInfo("warning", problem.Message, null, null)).ToList();
        return (state, Resources.History_ConstReports, new ConstReportChangeResult(number, warnings, new ChangeInfo(false, false, null)));
    }
}
