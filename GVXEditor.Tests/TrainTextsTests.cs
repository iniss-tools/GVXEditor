using System.Reflection;
using System.Windows.Forms;
using GVXEditor.Config;
using GVXEditor.Domain;
using GVXEditor.Domain.Stations.Texts;
using GVXEditor.UI.Boards;
using ToolsCore;
using ToolsCore.Iniss2.Tables;
using ToolsCore.XML;

namespace GVXEditor.Tests;

/// <summary>
/// Generovanie textov vlakov do TTVlaky.xml: stanice smeru zo stanic hlasenia ako server (zaznam pri vlaku, vzor,
/// predvolene stanice), bez cielovej a vychodzej stanice, varianty s poznamkou, zaznam len tam, kde by server ukazal nieco
/// ine, a bez prekrytia textov podla trasy pri doplnani.
/// </summary>
[TestClass]
public class TrainTextsTests
{
    private const int Buk = 5400015;
    private const int Dub = 5400030;
    private const int Vrch = 5400040;

    // Javor -> Lipovec (Z) -> Buk (len prejazd) -> Dub -> Horany (Z) -> Vrch
    private const string Train611 = """
        <v o="2030-12-15" d="2031-12-13"><v jd="denne">
          <z c="5400005" o="600" dcv="611" dv="R" />
          <z c="5400010" p="610" o="612" f="Z" dcv="611" dv="R" />
          <z c="5400015" f="P" />
          <z c="5400030" p="630" o="631" />
          <z c="5400020" p="640" o="642" f="Z" />
          <z c="5400040" p="700" />
        </v></v>
        """;

    private const string Catalog = """
        <CATALOG>
          <t k="Odchody">
            <div k="Cíl" inf="CilNazev" />
            <div k="Směr" inf="PoNazev" />
          </t>
          <t k="Príchody">
            <div k="Odkud" inf="VychoziNazev" />
            <div k="Ze směru" inf="PredNazev" />
          </t>
        </CATALOG>
        """;

    private static TestInstallation Installation(string trains = Train611)
    {
        var installation = new TestInstallation();
        installation.Write(@"Server\Data\2030\GV.stanice.XML", """
            <ST>
              <s k="5400005" n="Javor" />
              <s k="5400010" n="Lipovec" />
              <s k="5400015" n="Buk" />
              <s k="5400030" n="Dub" />
              <s k="5400020" n="Horany" />
              <s k="5400040" n="Vrch" />
            </ST>
            """);
        installation.Write(@"Server\Data\2030\01 Lipovec\TCatalog.xml", Catalog);
        installation.Write(@"Server\Data\2030\02 Horany\TCatalog.xml", Catalog);
        installation.Trains(trains);
        return installation;
    }

    private static (Iniss2Workspace Workspace, GroupDocument Group) Load(TestInstallation installation)
    {
        var workspace = Iniss2Workspace.Load(installation.DataDir);
        return (workspace, workspace.Groups[0]);
    }

    private static List<GeneratedText> Generate(TestInstallation installation, int station, GeneratedTextKind kind, bool longReport = false,
        bool defaultsFile = true)
    {
        var (workspace, group) = Load(installation);
        var choice = TrainTextGenerating.Choices(group.Stations[station]).First(item => item.Kind == kind);
        return TrainTextGenerating.Generate(group, station, kind, choice.Target, longReport, defaultsFile, workspace.StationName);
    }

    [TestMethod]
    public void SmerDo_PredvoleneStaniceBezCielovejAPrejazdu()
    {
        using var installation = Installation();
        installation.Write(@"Server\Data\2030\Default.Stations.xml", $"""
            <STANICE><s st="{Dub}" /><s st="{Buk}" /><s st="{TestInstallation.Horany}" k="false" /></STANICE>
            """);

        Assert.AreEqual("Dub", Generate(installation, 0, GeneratedTextKind.StationsTo).Single().Text);
        Assert.AreEqual("Dub#Horany", Generate(installation, 0, GeneratedTextKind.StationsTo, longReport: true).Single().Text);
    }

    [TestMethod]
    public void SmerDo_BezPredvolenychKratkeLenCielDlheVsetky()
    {
        using var installation = Installation();

        var shortText = Generate(installation, 0, GeneratedTextKind.StationsTo, defaultsFile: false).Single();
        Assert.AreEqual("", shortText.Text, "kratke hlasenie povie len cielovu stanicu");
        Assert.AreEqual("Dub#Horany", Generate(installation, 0, GeneratedTextKind.StationsTo, longReport: true, defaultsFile: false).Single().Text);
    }

    [TestMethod]
    public void SmerDo_ZaznamPriVlakuMaPrednost()
    {
        using var installation = Installation();
        installation.Write(@"Server\Data\2030\01 Lipovec\StaHlasVl.xml", $"""
            <STAHLASVL><v cv="611"><d>{TestInstallation.Horany}</d><d>{Vrch}</d></v></STAHLASVL>
            """);

        Assert.AreEqual("Horany", Generate(installation, 0, GeneratedTextKind.StationsTo).Single().Text);
    }

    [TestMethod]
    public void SmerZo_VzorAleboLenVychodzia()
    {
        using var installation = Installation();
        Assert.AreEqual("", Generate(installation, 1, GeneratedTextKind.StationsFrom).Single().Text, "bez vzoru server povie len vychodziu stanicu");

        installation.Write(@"Server\Data\2030\02 Horany\StaHlasB.xml", $"""
            <STAHLAS><vzor><z s="{TestInstallation.Javor}" /><z s="{TestInstallation.Lipovec}" h="true" /><z s="{Dub}" h="true" /><z s="{TestInstallation.Horany}" /></vzor></STAHLAS>
            """);
        Assert.AreEqual("Lipovec#Dub", Generate(installation, 1, GeneratedTextKind.StationsFrom).Single().Text);
    }

    [TestMethod]
    public void Ciel_RovnakyAkoServerSaNezapise()
    {
        using var installation = Installation();

        var text = Generate(installation, 0, GeneratedTextKind.Destination).Single();

        Assert.AreEqual(("611", "Vrch", true), (text.Number, text.Text, text.IsRedundant));
        Assert.IsFalse(TrainTextGenerating.WillWrite(text, false, true));
    }

    [TestMethod]
    public void Doplnenie_NeprekryjeTextPodlaTrasy()
    {
        using var installation = Installation();
        installation.Write(@"Server\Data\2030\Default.Stations.xml", $"""<STANICE><s st="{Dub}" /></STANICE>""");
        installation.Write(@"Server\Data\2030\01 Lipovec\TTTrasy.xml", $"""
            <TTEXTSTRC><b n="TrasaDoSmeru"><r cat="Odchody" div="Směr" />
              <t t="cez Dub"><s k="{TestInstallation.Lipovec}" /><s k="{Dub}" /><s k="{TestInstallation.Horany}" /><s k="{Vrch}" /></t>
            </b></TTEXTSTRC>
            """);

        var text = Generate(installation, 0, GeneratedTextKind.StationsTo).Single();

        Assert.AreEqual(("Dub", "cez Dub", true), (text.Text, text.ServerText, text.HasFileText));
        Assert.IsFalse(TrainTextGenerating.WillWrite(text, false, false), "doplnenie text podla trasy neprekryje");
        Assert.IsTrue(TrainTextGenerating.WillWrite(text, false, true), "nahradenie vsetkych ano");
    }

    [TestMethod]
    public void Varianty_RozneTextySPoznamkou()
    {
        // druha varianta ide bez Dubu - text kazdej varianty s jej poznamkou, poznamky pred textom bez poznamky
        using var installation = Installation("""
            <v o="2030-12-15" d="2031-12-13">
              <v jd="v 6"><z c="5400010" o="612" f="Z" dcv="611" /><z c="5400030" p="630" o="631" /><z c="5400040" p="700" /></v>
              <v jd="kromě 6"><z c="5400010" o="612" f="Z" dcv="611" /><z c="5400040" p="650" /></v>
            </v>
            """);
        installation.Write(@"Server\Data\2030\Default.Stations.xml", $"""<STANICE><s st="{Dub}" /></STANICE>""");

        var texts = Generate(installation, 0, GeneratedTextKind.StationsTo);

        CollectionAssert.AreEqual(new[] { ("v 6", "Dub"), ("kromě 6", "") }, texts.Select(text => (text.Validity, text.Text)).ToArray());
    }

    [TestMethod]
    public void Zapis_DoplnenieANahradenie()
    {
        GeneratedText New(string number, string text) => new(number, "", text);
        var choice = new TextSetChoice("TrasaDoSmeru1", [new TextTarget("Odchody", "Směr")], GeneratedTextKind.StationsTo, true);
        IReadOnlyList<TrainTextSet> sets =
        [
            new TrainTextSet("TrasaDoSmeru1")
            {
                Targets = choice.Targets,
                Trains = [new TrainText("611", "", "rucne", TrainTextSet.NoFont), new TrainText("613", "", "rucne", TrainTextSet.NoFont)]
            }
        ];
        GeneratedText[] generated = [New("611", "Dub"), New("613", ""), New("615", "Buk")];

        var (missing, added) = TrainTextGenerating.Apply(sets, choice, generated, false);
        CollectionAssert.AreEqual(new[] { ("611", "rucne"), ("613", "rucne"), ("615", "Buk") }, missing[0].Trains.Select(text => (text.Number, text.Text)).ToArray());
        Assert.AreEqual(1, added);

        var (all, written) = TrainTextGenerating.Apply(sets, choice, generated, true);
        CollectionAssert.AreEqual(new[] { ("613", "rucne"), ("611", "Dub"), ("615", "Buk") }, all[0].Trains.Select(text => (text.Number, text.Text)).ToArray(),
            "prazdny vygenerovany text zaznam ponecha");
        Assert.AreEqual(2, written);

        var (same, none) = TrainTextGenerating.Apply(all, choice, generated, true);
        Assert.AreSame(all, same, "rovnake texty nie su zmena");
        Assert.AreEqual(0, none);
    }

    [TestMethod]
    public void Sady_ExistujuceANoveZKatalogu()
    {
        using var installation = Installation();
        installation.Write(@"Server\Data\2030\01 Lipovec\TTTrasy.xml", """
            <TTEXTSTRC><b n="TrasaDoSmeruDlouhe"><r cat="Odchody" div="Směr" /></b></TTEXTSTRC>
            """);
        var (_, group) = Load(installation);

        var choices = TrainTextGenerating.Choices(group.Stations[0]);

        CollectionAssert.AreEqual(new[] { "TrasaDoSmeruDlouhe", "CiloveStanice1", "VychoziStanice1", "TrasaZeSmeru1" }, choices.Select(choice => choice.Name).ToArray());
        CollectionAssert.AreEqual(new[] { false, true, true, true }, choices.Select(choice => choice.IsNew).ToArray());
        Assert.AreEqual(GeneratedTextKind.StationsTo, choices[0].Kind);
    }

    [TestMethod]
    public void Okno_NahladAZapisDoNovejSady()
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try
            {
                OknoNahladAZapis();
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

    private static void OknoNahladAZapis()
    {
        var styles = Styles<GVXEditorStyle>.CreateDefault();
        // inicializator vlastnosti obide setter - tema okien cita styl z GlobSettings
        _ = new AppSession<GVXEditorConfig, GVXEditorStyle>(new GVXEditorConfig(), styles, styles.StyleList[0]) { UsingStyle = styles.StyleList[0] };
        using var installation = Installation();
        installation.Write(@"Server\Data\2030\Default.Stations.xml", $"""<STANICE><s st="{Dub}" /></STANICE>""");
        var (workspace, group) = Load(installation);
        using var form = new FGenerateTexts(group, 0, workspace.StationName, true);
        form.CreateControl();
        T Field<T>(string name) => (T)typeof(FGenerateTexts).GetField(name, BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(form)!;

        // predvolene prva sada s podporovanym udajom - ciel, ktory server ukaze aj bez zaznamu
        Assert.IsFalse(Field<Button>("bOk").Enabled);

        var sets = Field<ComboBox>("cboSet");
        sets.SelectedIndex = sets.Items.Cast<object>().ToList().FindIndex(item => item.ToString()!.Contains("TrasaDoSmeru1", StringComparison.Ordinal));
        var rows = (List<TextPreviewRow>)Field<DataGridView>("dgvPreview").DataSource!;
        Assert.AreEqual(("611", "Dub", "pribudne"), (rows.Single().Number, rows.Single().Text, rows.Single().Change));

        var (updated, written, set) = form.Apply(group);

        Assert.AreEqual((1, "TrasaDoSmeru1"), (written, set));
        var texts = updated.Stations[0].Files!.TextSets.Single();
        Assert.AreEqual(("TrasaDoSmeru1", "Odchody", "Směr"), (texts.Name, texts.Targets.Single().Catalog, texts.Targets.Single().Column));
        Assert.AreEqual(("611", "Dub"), (texts.Trains.Single().Number, texts.Trains.Single().Text));
    }
}
