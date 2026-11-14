using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
using GVXEditor.Config;
using GVXEditor.Domain;
using GVXEditor.UI.EditTrain;
using ToolsCore;
using ToolsCore.Iniss2.Grafikon;
using ToolsCore.XML;
using static GVXEditor.Tests.EditingTests;

namespace GVXEditor.Tests;

/// <summary>
/// Okno Upravit vlak bez zobrazenia: tabulky su naviazane na koncept a OK vrati vlak so zmenami.
/// </summary>
[TestClass]
public class EditTrainFormTests
{
    private const BindingFlags Any = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private static readonly IReadOnlySet<int> Interest = new HashSet<int> { TestInstallation.Lipovec, TestInstallation.Horany };

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

    private static FEditTrain Form()
    {
        var styles = Styles<GVXEditorStyle>.CreateDefault();
        var session = new AppSession<GVXEditorConfig, GVXEditorStyle>(new GVXEditorConfig(), styles, styles.StyleList[0]);
        // inicializator vlastnosti obide setter - tema okien cita styl z GlobSettings
        session.UsingStyle = styles.StyleList[0];
        var context = new EditorContext(session);
        var draft = Draft(Stop(TestInstallation.Lipovec, "", "8:00", "1001"), Stop(TestInstallation.Horany, "8:15", ""));
        var group = new GroupDocument(new DirListGroup("2030", [
            new DirListItem("01 Lipovec", TestInstallation.Lipovec, "", "", DirListItemFlags.None),
            new DirListItem("02 Horany", TestInstallation.Horany, "", "", DirListItemFlags.None)
        ]), new GvFile([], []), null);
        var form = new FEditTrain(context, draft, group, -1, number => number.ToString(CultureInfo.InvariantCulture), "test");
        form.CreateControl();
        // bez zobrazenia nepride Load - v nom okno vyberie prvu variantu
        form.GetType().GetMethod("OnLoad", Any)!.Invoke(form, [EventArgs.Empty]);
        Application.DoEvents();
        return form;
    }

    private static T Field<T>(Form form, string name) => (T)form.GetType().GetField(name, Any)!.GetValue(form)!;

    private static void Click(Form form, string handler) =>
        form.GetType().GetMethod(handler, Any)!.Invoke(form, [null, EventArgs.Empty]);

    [TestMethod]
    public void UpravitVlak_ZmenaCasuVTabulkeJeVoVysledku() => Sta(() =>
    {
        using var form = Form();
        var stops = Field<DataGridView>(form, "dgvStops");
        Assert.AreEqual(2, stops.Rows.Count);

        stops.Rows[0].Cells["cDeparture"].Value = "8:10";
        Click(form, "bOk_Click");

        Assert.AreEqual(DialogResult.OK, form.DialogResult);
        Assert.AreEqual(490, form.Result!.Variants[0].Stops[0].Departure);
    });

    [TestMethod]
    public void UpravitVlak_ChybaVTabulkeZakazeOk() => Sta(() =>
    {
        using var form = Form();
        var stops = Field<DataGridView>(form, "dgvStops");

        stops.Rows[1].Cells["cArrival"].Value = "8:75";

        Assert.IsFalse(Field<Button>(form, "bOk").Enabled);
        Assert.AreEqual(1, Field<ListBox>(form, "lbProblems").Items.Count);
    });

    [TestMethod]
    public void UpravitVlak_KopiaVariantyPredVybranu() => Sta(() =>
    {
        using var form = Form();
        var variants = Field<DataGridView>(form, "dgvVariants");

        Click(form, "tsbVarCopy_Click");
        Field<TextBox>(form, "tbValidity").Text = "jede v 6";
        Click(form, "bOk_Click");

        Assert.AreEqual(2, variants.Rows.Count);
        var train = form.Result!;
        Assert.AreEqual(("jede v 6", ""), (train.Variants[0].Validity, train.Variants[1].Validity));
    });

    // priznaky slovami sa v uzkom aj sirokom okne zalomia - posledny riadok nesmie skoncit pod poznamkou
    [TestMethod]
    public void UpravitVlak_PriznakyVariantyNadPoznamkou() => Sta(() =>
    {
        using var form = Form();
        var flags = Field<FlowLayoutPanel>(form, "flpFlags");
        var note = Field<TextBox>(form, "tbNote");

        foreach (var width in new[] { 800, 1400, 1000 })
        {
            form.Width = width;
            var last = flags.Controls.Cast<Control>().Max(control => control.Bottom);
            Assert.IsLessThanOrEqualTo(flags.Height, last, $"posledny priznak je vidno (sirka {width})");
            Assert.IsGreaterThanOrEqualTo(flags.Bottom, note.Top, $"poznamka je pod priznakmi (sirka {width})");
        }
    });
}
