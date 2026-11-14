using GVXEditor.Domain;
using GVXEditor.Domain.Stations;
using GVXEditor.Domain.Stations.Drafts;
using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Tests;

/// <summary>
/// Zaznamy vlaku v suboroch stanic: cisla vlaku v staniciach ako server, uprava len zaznamov vlaku a zapis len zmenenych suborov.
/// </summary>
[TestClass]
public class StationsTests
{
    // osobak 1001 z Javora (mimo grafikonu) do Lipovca, odtial ako rychlik 2002 do Horian
    private const string Trains = """
        <v o="2030-12-15" d="2031-12-13">
          <v>
            <z c="5400005" o="480" dcv="1001" dv="Os" />
            <z c="5400010" p="495" o="497" f="Z" dcv="2002" dv="Sp" />
            <z c="5400020" p="510" f="Z" />
          </v>
        </v>
        """;

    private static Iniss2Workspace Open(TestInstallation installation)
    {
        installation.Trains(Trains);
        installation.Write(@"Server\Data\2030\01 Lipovec\Dodatky.xml", """
            <DODATKY>
              <!-- dodatky rychlika -->
              <v cv="2002" k="D1" rep="Ll" />
              <v cv="3003" k="D2" />
            </DODATKY>
            """);
        installation.Write(@"Server\Data\2030\Vyluky.xml", """<VYLUKY><v cv="2002" k="7" od="2031-05-01" do="2031-05-31" /></VYLUKY>""");
        return Iniss2Workspace.Load(installation.DataDir);
    }

    private static string Text(TestInstallation installation, string file) =>
        File.ReadAllText(Path.Combine(installation.DataDir, "2030", file), System.Text.Encoding.Latin1);

    [TestMethod]
    public void Kluce_CisloPriOdchodeVStaniciachGrafikonu()
    {
        using var installation = new TestInstallation();
        var group = Open(installation).Groups[0];
        var train = group.Timetable!.Trains[0];

        var keys = TrainStations.Keys(train, group.Group);

        CollectionAssert.AreEqual(new[] { ("01 Lipovec", "2002"), ("02 Horany", "2002") },
            keys.Select(key => (key.Station.Directory, key.Number)).ToList());
        Assert.AreEqual("2002", TrainStations.LockoutNumber(train, group.InterestStations));
    }

    [TestMethod]
    public void Uprava_BezZmenyNechaRovnakeSubory()
    {
        using var installation = new TestInstallation();
        var group = Open(installation).Groups[0];
        var key = TrainStations.Keys(group.Timetable!.Trains[0], group.Group)[0];
        var files = group.Stations[0].Files!;

        var edit = StationEdit.Of(key, group.Stations[0]);

        Assert.HasCount(2, edit.Positions);
        Assert.HasCount(1, edit.Additions);
        Assert.AreSame(files, edit.Apply(files));
    }

    [TestMethod]
    public void Uprava_ZmeniLenZaznamyVlakuAZapiseLenZmenenySubor()
    {
        using var installation = new TestInstallation();
        var workspace = Open(installation);
        var group = workspace.Groups[0];
        var key = TrainStations.Keys(group.Timetable!.Trains[0], group.Group)[0];
        var positionsBefore = Text(installation, @"01 Lipovec\Positions.xml");

        var edit = StationEdit.Of(key, group.Stations[0]);
        edit.Additions[0].Reports = "Pp";
        edit.Additions.Add(new AdditionDraft { Key = "D9", Validity = "jede v 6" });
        workspace.Change(0, group.WithStation(0, edit.Apply(group.Stations[0].Files!)), "dodatky");

        Assert.AreEqual(SaveOutcome.Saved, workspace.Save().Outcome);
        var additions = Text(installation, @"01 Lipovec\Dodatky.xml");
        StringAssert.Contains(additions, "<!-- dodatky rychlika -->");
        StringAssert.Contains(additions, """<v cv="2002" k="D1" rep="Pp" />""");
        // novy zaznam vlaku ide za jeho povodny zaznam, zaznam ineho vlaku ostane za nimi
        Assert.IsLessThan(additions.IndexOf("D2", StringComparison.Ordinal), additions.IndexOf("D9", StringComparison.Ordinal));
        Assert.AreEqual(positionsBefore, Text(installation, @"01 Lipovec\Positions.xml"));
        Assert.IsFalse(File.Exists(Path.Combine(installation.DataDir, "2030", "02 Horany", "Dodatky.xml")));
    }

    [TestMethod]
    public void Zaznamy_NoveIduNaMiestaPovodnych()
    {
        string[] all = ["A1", "B1", "A2", "B2"];
        bool Mine(string record) => record[0] == 'A';

        CollectionAssert.AreEqual(all, RecordLists.Replace(all, Mine, ["A1", "A2"], string.Equals).ToList());
        Assert.AreSame(all, RecordLists.Replace(all, Mine, ["A1", "A2"], string.Equals));
        CollectionAssert.AreEqual(new[] { "A1", "B1", "A9", "A3", "B2" }, RecordLists.Replace(all, Mine, ["A1", "A9", "A3"], string.Equals).ToList());
        CollectionAssert.AreEqual(new[] { "A1", "B1", "B2" }, RecordLists.Replace(all, Mine, ["A1"], string.Equals).ToList());
        CollectionAssert.AreEqual(new[] { "B1", "B2", "C1" }, RecordLists.Replace(["B1", "B2"], Mine, ["C1"], string.Equals).ToList());
    }

    [TestMethod]
    public void Vyluky_ZaznamyVlakuPodCislomVGrafikone()
    {
        using var installation = new TestInstallation();
        var workspace = Open(installation);
        var group = workspace.Groups[0];
        var train = group.Timetable!.Trains[0];
        var notices = NoticesEdit.Of(TrainStations.LockoutNumber(train, group.InterestStations), TrainStations.Keys(train, group.Group), group);

        Assert.HasCount(1, notices.Lockouts);
        Assert.AreSame(group.Lockouts, notices.ApplyLockouts(group.Lockouts!));
        notices.Lockouts[0].To = new DateOnly(2031, 6, 30).ToString("d", System.Globalization.CultureInfo.CurrentCulture);
        notices.Diversions.Add(new DiversionDraft { Station = notices.Keys[1], Key = "O1" });
        workspace.Change(0, group with
        {
            Lockouts = notices.ApplyLockouts(group.Lockouts!),
            Diversions = notices.ApplyDiversions(group.Diversions!)
        }, "vyluky");

        Assert.AreEqual(SaveOutcome.Saved, workspace.Save().Outcome);
        StringAssert.Contains(Text(installation, "Vyluky.xml"), "do=\"2031-06-30\"");
        var diversion = OdklonyFile.Read(Path.Combine(installation.DataDir, "2030")).Single();
        Assert.AreEqual(("2002", TestInstallation.Horany, "O1"), (diversion.Number, diversion.Station, diversion.Key));
    }
}
