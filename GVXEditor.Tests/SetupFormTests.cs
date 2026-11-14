using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
using GVXEditor.Config;
using GVXEditor.Domain;
using GVXEditor.Domain.Setup;
using GVXEditor.Domain.Stations;
using GVXEditor.UI.Setup;
using ToolsCore;
using ToolsCore.XML;

namespace GVXEditor.Tests;

/// <summary>
/// Okna Nastavenie grafikonu, Hlasenia obsluhy a Pridat stanicu bez zobrazenia: tabulky su naviazane na koncepty, zmena
/// v tabulke je vo vysledku a okno bez zmeny vrati povodne data.
/// </summary>
[TestClass]
public class SetupFormTests
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

    private static void Click(Form form, string handler, object? sender = null) =>
        form.GetType().GetMethod(handler, Any)!.Invoke(form, [sender, EventArgs.Empty]);

    private static Iniss2Workspace Workspace(TestInstallation installation)
    {
        // tema okien cita styl z nastaveni relacie
        var styles = Styles<GVXEditorStyle>.CreateDefault();
        var session = new AppSession<GVXEditorConfig, GVXEditorStyle>(new GVXEditorConfig(), styles, styles.StyleList[0]);
        session.UsingStyle = styles.StyleList[0];
        installation.Trains("");
        installation.Write(@"Server\Data\ConstReports.xml", """<CONSTREPORTS><r k="4" n="Prestup" ik="Přestup"><s>5400020</s></r></CONSTREPORTS>""");
        installation.Write(@"Server\Data\2030\01 Lipovec\statedgm.xml", "<STATEDGM />");
        return Iniss2Workspace.Load(installation.DataDir);
    }

    private static string Name(int number) => number.ToString(CultureInfo.InvariantCulture);

    [TestMethod]
    public void NastavenieGrafikonu_ZmenaKolajeVTabulke() => Sta(() =>
    {
        using var installation = new TestInstallation();
        var group = Workspace(installation).Groups[0];
        using var form = new FSetup(group, Name, 0);
        form.CreateControl();

        var tracks = Field<DataGridView>(form, "dgvTracks");
        Assert.AreEqual(2, tracks.Rows.Count);
        Assert.AreSame(group, form.Apply(group));

        tracks.Rows[1].Cells["cTrackName"].Value = "2a";
        Click(form, "bOk_Click");

        Assert.AreEqual(DialogResult.OK, form.DialogResult);
        Assert.AreEqual("2a", form.Apply(group).Stations[0].Tracks[1].Name);
    });

    [TestMethod]
    public void NastavenieGrafikonu_DuplicitnaKolajNepustiOk() => Sta(() =>
    {
        using var installation = new TestInstallation();
        var group = Workspace(installation).Groups[0];
        using var form = new FSetup(group, Name, 0);
        form.CreateControl();

        Click(form, "tsbAdd_Click", Field<ToolStripButton>(form, "tsbTracksAdd"));
        var tracks = Field<DataGridView>(form, "dgvTracks");
        tracks.Rows[^1].Cells["cTrackKey"].Value = "1";

        Assert.IsFalse(Field<Button>(form, "bOk").Enabled);
        Assert.IsGreaterThan(0, Field<ListBox>(form, "lbProblems").Items.Count);
    });

    [TestMethod]
    public void HlaseniaObsluhy_NoveHlasenieDostaneDalsieCislo() => Sta(() =>
    {
        using var installation = new TestInstallation();
        var workspace = Workspace(installation);
        var reports = workspace.ConstReports;
        using var form = new FConstReports(reports, new KeyCatalog(null, null, reports), ConstReportsEdit.UsedKeys(workspace.Groups), workspace.StationNames);
        form.CreateControl();
        Assert.AreSame(reports, form.Apply());

        Click(form, "tsbAdd_Click");
        var grid = Field<DataGridView>(form, "dgvReports");
        grid.Rows[1].Cells["cRepFunction"].Value = "Odřeknutí";
        Click(form, "bOk_Click");

        Assert.AreEqual(DialogResult.OK, form.DialogResult);
        Assert.AreEqual((5, "Odřeknutí"), (form.Apply()[1].Key, form.Apply()[1].Function));
    });

    [TestMethod]
    public void PridatStanicu_NazovAPriecinokPodlaCisla() => Sta(() =>
    {
        using var installation = new TestInstallation();
        var workspace = Workspace(installation);
        using var form = new FAddStation(workspace, 0, 0);
        form.CreateControl();

        Field<TextBox>(form, "tbNumber").Text = TestInstallation.Javor.ToString(CultureInfo.InvariantCulture);

        Assert.AreEqual("Javor", Field<TextBox>(form, "tbName").Text);
        Assert.AreEqual("03 Javor", Field<TextBox>(form, "tbDirectory").Text);
        Assert.IsTrue(Field<Button>(form, "bOk").Enabled);
        Assert.AreEqual(new NewStation(0, "03 Javor", TestInstallation.Javor, "Javor", ToolsCore.Iniss2.Grafikon.DirListItemFlags.None, 0, 0), form.Station);
    });
}
