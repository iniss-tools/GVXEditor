using System.Reflection;
using System.Windows.Forms;
using GVXEditor.Config;
using GVXEditor.Domain;
using GVXEditor.UI.Code;
using ToolsCore;
using ToolsCore.CodeEditor;
using ToolsCore.XML;

namespace GVXEditor.Tests;

/// <summary>
/// Okna Stavovy diagram, Tabulky TabTab a Skript hlaseni bez zobrazenia: okno bez zmeny vrati povodne data, zmena v editore
/// kodu je vo vysledku.
/// </summary>
[TestClass]
public class CodeFormTests
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
    public void StavovyDiagram_ZmenaCasuAutomatiky() => Sta(() =>
    {
        using var installation = CodeTests.Installation();
        var context = Context(installation);
        var group = context.Workspace!.Groups[0];
        using var form = new FStateDiagram(context, 0, 0);
        form.CreateControl();
        Assert.AreSame(group, form.Apply(group));

        var tree = Field<TreeView>(form, "tvGraphs");
        tree.SelectedNode = tree.Nodes[0].Nodes[2];
        Assert.AreEqual(".PlanDobaPobytu < 5", Field<CodeEditorBox>(form, "codeCondition").Code);
        Field<CodeEditorBox>(form, "codeTime").Scintilla.Text = ".DatumCasOdj(True).AddSeconds(-900)";

        var automation = form.Apply(group).Stations[0].Code!.Diagram!.Graphs[0].Nodes[2].Automation!;
        Assert.AreEqual(".DatumCasOdj(True).AddSeconds(-900)", automation.Time);
    });

    [TestMethod]
    public void TabTab_ZmenaVyrazu() => Sta(() =>
    {
        using var installation = CodeTests.Installation();
        var context = Context(installation);
        var group = context.Workspace!.Groups[0];
        using var form = new FTabTabs(context, 0, -1);
        form.CreateControl();
        Assert.AreSame(group, form.Apply(group));

        Field<ListBox>(form, "lbTables").SelectedIndex = 1;
        Assert.AreEqual("Return sOrig", Field<CodeEditorBox>(form, "codeExpression").Code);
        Field<CodeEditorBox>(form, "codeExpression").Scintilla.Text = "Return \"x\"";

        Assert.AreEqual("\n      Return \"x\"\n    ", form.Apply(group).GlobalTabTabs![1].Expression);
    });

    [TestMethod]
    public void Editor_FarbyZoSchemyProgramu() => Sta(() =>
    {
        using var installation = CodeTests.Installation();
        var context = Context(installation);
        var style = context.UsingStyle;
        style.SetOverride(VisualBasicColors.Keys.First(key => key.Id == "VisualBasic.Keyword"), new ColorOverride(System.Drawing.Color.Orange));
        using var form = new FLogText(context);
        form.CreateControl();

        var editor = Field<CodeEditorBox>(form, "codeText").Scintilla;
        Assert.AreEqual(System.Drawing.Color.Orange.ToArgb(), editor.Styles[ScintillaNET.Style.Vb.Keyword].ForeColor.ToArgb());
        Assert.AreEqual(style.VisualBasicScheme.Text.BackColor.ToArgb(), editor.Styles[ScintillaNET.Style.Default].BackColor.ToArgb());
    });

    [TestMethod]
    public void SkriptHlaseni_BezZmenyTenIstyText() => Sta(() =>
    {
        using var installation = CodeTests.Installation();
        var context = Context(installation);
        var text = context.Workspace!.History.Current.LogText;
        using var form = new FLogText(context);
        form.CreateControl();

        Assert.AreSame(text, form.Result());
        Field<CodeEditorBox>(form, "codeText").Scintilla.AppendText("' koniec\r\n");
        StringAssert.EndsWith(form.Result(), "End Class\r\n' koniec\r\n");
    });
}
