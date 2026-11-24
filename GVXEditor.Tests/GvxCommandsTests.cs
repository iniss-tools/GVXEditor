using System.Windows.Forms;
using GVXEditor.UI;

namespace GVXEditor.Tests;

/// <summary>
/// Prikazy hlavneho okna a ich skratky v config.xml.
/// </summary>
[TestClass]
public class GvxCommandsTests
{
    [TestMethod]
    public void All_IdentifikatoryASkratkyBezDuplicit()
    {
        var ids = GvxCommands.All.Select(command => command.Id).ToList();
        CollectionAssert.AreEquivalent(new[]
        {
            "Open", "Save", "Check", "VerifyServer", "Reload", "Undo", "Redo", "AddTrain", "EditTrain", "CopyTrain", "DeleteTrain", "RenameNumber", "ShiftValidity", "ViewInstallation", "ViewAnalysis", "ResetLayout", "GroupSetup", "ConstReports",
            "StateDiagram", "TabTab", "Reports", "LogText", "Boards", "GenerateTexts", "NewGroup", "AddStation", "AppSettings", "InfoApp",
            "CheckUpdates", "ShowLogs", "ReportBug", "SuggestFeature"
        }, ids);

        var shortcuts = GvxCommands.All.Where(command => command.DefaultKeys != Keys.None).Select(command => command.DefaultKeys).ToList();
        Assert.HasCount(shortcuts.Count, shortcuts.Distinct().ToList());
    }
}
