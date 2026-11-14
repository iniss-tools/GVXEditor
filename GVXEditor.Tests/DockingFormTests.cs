using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
using ExControls;
using GVXEditor.Config;
using GVXEditor.Domain;
using GVXEditor.Domain.Analysis;
using GVXEditor.Properties;
using GVXEditor.UI.Analysis;
using GVXEditor.UI.Main;
using ToolsCore;
using ToolsCore.Icons;
using ToolsCore.XML;

namespace GVXEditor.Tests;

/// <summary>
/// Ukotvitelne panely hlavneho okna (strom instalacie, kontrola instalacie) a panel kontroly bez zobrazenia.
/// </summary>
[TestClass]
public class DockingFormTests
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

    private static EditorContext Context(string? layout = null)
    {
        var styles = Styles<GVXEditorStyle>.CreateDefault();
        var session = new AppSession<GVXEditorConfig, GVXEditorStyle>(new GVXEditorConfig { DockLayout = layout }, styles, styles.StyleList[0]);
        // inicializator vlastnosti obide setter - tema okien cita styl z GlobSettings
        session.UsingStyle = styles.StyleList[0];
        return new EditorContext(session);
    }

    private static FMain MainWindow(EditorContext context)
    {
        var form = new FMain(context);
        form.CreateControl();
        return form;
    }

    private static T Field<T>(Control control, string name) => (T)control.GetType().GetField(name, Any)!.GetValue(control)!;

    private static ExDockPanel Panel(FMain form, string key) => Field<ExDockManager>(form, "_dock").Panels.Single(panel => panel.Key == key);

    [TestMethod]
    public void HlavneOkno_StromVPaneliAVlakyVDokumente() => Sta(() =>
    {
        using var form = MainWindow(Context());
        var dock = Field<ExDockManager>(form, "_dock");

        CollectionAssert.AreEqual(new[] { "Installation", "Analysis" }, dock.Panels.Select(panel => panel.Key).ToArray());
        Assert.AreSame(Panel(form, "Installation"), Field<TreeView>(form, "tvData").Parent);
        Assert.AreSame(dock.Document, Field<Panel>(form, "pDocument").Parent);
        Assert.IsTrue(dock.Panels.All(panel => panel.IsShown));
        Assert.AreEqual((ExDockSide.Left, ExDockSide.Bottom), (Panel(form, "Installation").Side, Panel(form, "Analysis").Side));
    });

    [TestMethod]
    public void Rozlozenie_ObnoviSaZNastaveniAPrikazomVratiPredvolene() => Sta(() =>
    {
        string layout;
        using (var first = MainWindow(Context()))
        {
            Field<ExDockManager>(first, "_dock").HidePanel(Panel(first, "Analysis"));
            layout = Field<ExDockManager>(first, "_dock").SaveLayout();
        }

        using var form = MainWindow(Context(layout));
        Assert.IsFalse(Panel(form, "Analysis").IsShown);

        var commands = Field<ToolsCore.Commands.CommandSet>(form, "_commands");
        commands["ResetLayout"].Execute();

        Assert.IsTrue(Panel(form, "Analysis").IsShown);
    });

    [TestMethod]
    public void Rozlozenie_NeplatneOstavaPredvolene() => Sta(() =>
    {
        using var form = MainWindow(Context("<nie je rozlozenie />"));

        Assert.IsTrue(Field<ExDockManager>(form, "_dock").Panels.All(panel => panel.IsShown));
    });

    [TestMethod]
    public void KontrolaInstalacie_FiltreZavaznostiSPoctom() => Sta(() =>
    {
        // bez TrTypes.xml, statedgm.xml a ReportKeys.xml: chyby povinnych suborov
        using var installation = new TestInstallation();
        installation.Trains("""
            <v o="2030-12-15" d="2031-12-13">
              <v>
                <z c="5400010" o="480" f="Z" dcv="2002" dv="Sp" />
                <z c="5400020" p="495" f="Z" />
              </v>
            </v>
            """);
        var context = Context();
        context.Open(Iniss2Workspace.Load(installation.DataDir));
        Finding? navigated = null;
        using var view = new AnalysisView(context, finding => navigated = finding);
        view.CreateControl();

        view.RunChecks();

        var errors = view.Count(FindingSeverity.Error);
        var all = errors + view.Count(FindingSeverity.Warning) + view.Count(FindingSeverity.Info);
        Assert.IsGreaterThan(0, errors);
        Assert.AreEqual(all, view.Grid.Rows.Count);
        var errorsButton = Field<IconToolStripButton>(view, "tsbErrors");
        Assert.AreEqual(string.Format(CultureInfo.CurrentCulture, Resources.Analysis_Errors, errors), errorsButton.Text);

        errorsButton.Checked = false;

        Assert.AreEqual(all - errors, view.Grid.Rows.Count);
        Assert.IsNull(navigated);
    });
}
