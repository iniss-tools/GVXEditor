using GVXEditor.Properties;
using ToolsCore.Commands;

namespace GVXEditor.UI;

/// <summary>
/// Prikazy hlavneho okna s predvolenymi klavesovymi skratkami. Identifikator je nazov prvku skratky v config.xml -
/// nesmie sa menit, inak by sa stratili skratky nastavene pouzivatelom.
/// </summary>
internal static class GvxCommands
{
    public static CommandInfo Open => new("Open", Resources.Cmd_Open, Shortcut.CtrlO);
    public static CommandInfo Save => new("Save", Resources.Cmd_Save, Shortcut.CtrlS);
    public static CommandInfo Check => new("Check", Resources.Cmd_Check, Shortcut.CtrlShiftA);
    public static CommandInfo VerifyServer => new("VerifyServer", Resources.Cmd_VerifyServer, Shortcut.None);
    public static CommandInfo Reload => new("Reload", Resources.Cmd_Reload, Shortcut.F5);

    public static CommandInfo Undo => new("Undo", Resources.Cmd_Undo, Shortcut.CtrlZ);
    public static CommandInfo Redo => new("Redo", Resources.Cmd_Redo, Shortcut.CtrlY);
    public static CommandInfo AddTrain => new("AddTrain", Resources.Cmd_New, Shortcut.Ins);
    public static CommandInfo EditTrain => new("EditTrain", Resources.Cmd_Edit, Shortcut.ShiftIns);
    public static CommandInfo CopyTrain => new("CopyTrain", Resources.Cmd_Copy, Shortcut.CtrlD);
    public static CommandInfo DeleteTrain => new("DeleteTrain", Resources.Cmd_Delete, Shortcut.Del);
    public static CommandInfo RenameNumber => new("RenameNumber", Resources.Cmd_RenameNumber, Shortcut.F2);
    public static CommandInfo ShiftValidity => new("ShiftValidity", Resources.Cmd_ShiftValidity, Shortcut.None);

    public static CommandInfo ViewInstallation => new("ViewInstallation", Resources.Cmd_ViewInstallation, Shortcut.None);
    public static CommandInfo ViewAnalysis => new("ViewAnalysis", Resources.Cmd_ViewAnalysis, Shortcut.None);
    public static CommandInfo ResetLayout => new("ResetLayout", Resources.Cmd_ResetLayout, Shortcut.None);

    public static CommandInfo GroupSetup => new("GroupSetup", Resources.Cmd_GroupSetup, Shortcut.None);
    public static CommandInfo ConstReports => new("ConstReports", Resources.Cmd_ConstReports, Shortcut.None);
    public static CommandInfo GenerateTexts => new("GenerateTexts", Resources.Cmd_GenerateTexts, Shortcut.None);
    public static CommandInfo StateDiagram => new("StateDiagram", Resources.Cmd_StateDiagram, Shortcut.None);
    public static CommandInfo TabTab => new("TabTab", Resources.Cmd_TabTab, Shortcut.None);
    public static CommandInfo Reports => new("Reports", Resources.Cmd_Reports, Shortcut.None);
    public static CommandInfo LogText => new("LogText", Resources.Cmd_LogText, Shortcut.None);
    public static CommandInfo Boards => new("Boards", Resources.Cmd_Boards, Shortcut.None);
    public static CommandInfo NewGroup => new("NewGroup", Resources.Cmd_NewGroup, Shortcut.None);
    public static CommandInfo AddStation => new("AddStation", Resources.Cmd_AddStation, Shortcut.None);

    public static CommandInfo AppSettings => new("AppSettings", Resources.Cmd_AppSettings, Shortcut.CtrlShiftN);

    // text = identifikator: v strome skratiek sa ukaze text polozky ponuky
    public static CommandInfo InfoApp => new(nameof(InfoApp), nameof(InfoApp), Shortcut.None);
    public static CommandInfo CheckUpdates => new(nameof(CheckUpdates), nameof(CheckUpdates), Shortcut.None);
    public static CommandInfo ShowLogs => new(nameof(ShowLogs), nameof(ShowLogs), Shortcut.None);

    /// <summary>
    /// Vsetky prikazy hlavneho okna pre nastavenia skratiek (strom ich zoradi podla ponuky okna).
    /// </summary>
    public static IReadOnlyList<CommandInfo> All =>
        [Open, Save, Check, VerifyServer, Reload, Undo, Redo, AddTrain, EditTrain, CopyTrain, DeleteTrain, RenameNumber, ShiftValidity,
         ViewInstallation, ViewAnalysis, ResetLayout, GroupSetup, ConstReports, StateDiagram, TabTab, Reports, LogText, Boards, GenerateTexts, NewGroup, AddStation,
         AppSettings, InfoApp,
         CheckUpdates, ShowLogs];
}
