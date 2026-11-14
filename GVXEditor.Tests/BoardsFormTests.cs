using System.Reflection;
using System.Windows.Forms;
using GVXEditor.Config;
using GVXEditor.Domain;
using GVXEditor.UI.Boards;
using ToolsCore;
using ToolsCore.CodeEditor;
using ToolsCore.XML;

namespace GVXEditor.Tests;

/// <summary>
/// Okno Tabule bez zobrazenia: okno bez zmeny vrati povodny grafikon, zmena v paneli casti a v tabulke udajov je vo vysledku.
/// </summary>
[TestClass]
public class BoardsFormTests
{
    private const BindingFlags Any = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private static void Sta(Action test)
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try
            {
                test();
            }
            catch (Exception e)
            {
                failure = e;
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
        if (failure is not null)
            throw new AssertFailedException(failure.ToString());
    }

    private static T Field<T>(Form form, string name) => (T)form.GetType().GetField(name, Any)!.GetValue(form)!;

    private static EditorContext Context(TestInstallation installation)
    {
        var styles = Styles<GVXEditorStyle>.CreateDefault();
        var session = new AppSession<GVXEditorConfig, GVXEditorStyle>(new GVXEditorConfig(), styles, styles.StyleList[0]);
        session.UsingStyle = styles.StyleList[0];
        var context = new EditorContext(session);
        context.Open(Iniss2Workspace.Load(installation.DataDir));
        return context;
    }

    [TestMethod]
    public void Tabule_ZmenaPodmienkyAUdaja() => Sta(() =>
    {
        using var installation = BoardsTests.Installation();
        var context = Context(installation);
        var group = context.Workspace!.Groups[0];
        using var form = new FBoards(context, 0, 0);
        form.CreateControl();
        Assert.AreSame(group, form.Apply(group));
        Assert.IsTrue(Field<Button>(form, "bOk").Enabled);

        var tree = Field<TreeView>(form, "tvBoards");
        Assert.HasCount(2, tree.Nodes);
        tree.SelectedNode = tree.Nodes[0].Nodes[0];
        var condition = Field<CodeEditorBox>(form, "codeCondition");
        Assert.AreEqual(".Druh <> \"Os\"", condition.Code);
        condition.Scintilla.Text = ".Druh = \"R\"";

        Field<ListBox>(form, "lbTemplates").SelectedIndex = 0;
        var columns = Field<DataGridView>(form, "dgvColumns");
        columns.Rows[0].Cells["cColumnFont"].Value = "17";

        var boards = form.Apply(group).Stations[0].Boards!;
        Assert.AreEqual(".Druh = \"R\"", boards.Tables[0].Parts[0].Condition);
        Assert.AreEqual(17, boards.Catalog[0].Columns[0].Font);
        Assert.AreSame(group.Stations[0].Boards!.Tables[1], boards.Tables[1]);
    });
}
