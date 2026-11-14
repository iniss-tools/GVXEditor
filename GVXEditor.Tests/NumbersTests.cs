using GVXEditor.Domain;
using GVXEditor.Domain.Editing;
using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Tests;

/// <summary>
/// Cislo vlaku napriec subormi: premenovanie, kopia zaznamov pri spolocnom cisle a zmazanie nepouzivanych zaznamov.
/// </summary>
[TestClass]
public class NumbersTests
{
    private const string Train = """
        <v o="2030-12-15" d="2031-06-30">
          <v>
            <z c="5400010" o="480" f="Z" dcv="2002" dv="Sp" />
            <z c="5400020" p="495" f="Z" />
          </v>
        </v>
        """;

    // rovnake cislo v druhom polroku
    private const string Second = """
        <v o="2031-07-01" d="2031-12-13">
          <v>
            <z c="5400010" o="490" f="Z" dcv="2002" dv="Sp" />
            <z c="5400020" p="505" f="Z" />
          </v>
        </v>
        """;

    private static Iniss2Workspace Open(TestInstallation installation, string trains)
    {
        installation.Trains(trains);
        installation.Write(@"Server\Data\2030\01 Lipovec\Dodatky.xml", """<DODATKY><v cv="2002" k="D1" /><v cv="3003" k="D2" /></DODATKY>""");
        installation.Write(@"Server\Data\2030\Vyluky.xml", """<VYLUKY><v cv="2002" k="7" od="2031-05-01" do="2031-05-31" /></VYLUKY>""");
        return Iniss2Workspace.Load(installation.DataDir);
    }

    [TestMethod]
    public void Premenovanie_VoVsetkychSuboroch()
    {
        using var installation = new TestInstallation();
        var group = Open(installation, Train).Groups[0];

        Assert.AreEqual(4, TrainNumbers.RecordCount(group, "2002"));
        var renamed = TrainNumbers.Rename(group, 0, "2002", "2012", allTrains: false);

        Assert.AreEqual("2012", renamed.Timetable!.Trains[0].Variants[0].Stops[0].Number);
        Assert.AreEqual(0, TrainNumbers.RecordCount(renamed, "2002"));
        Assert.AreEqual(4, TrainNumbers.RecordCount(renamed, "2012"));
        CollectionAssert.AreEqual(new[] { "2012", "3003" }, renamed.Stations[0].Files!.Additions.Select(record => record.Number).ToList());
        // stanica bez zaznamov cisla ostane ten isty objekt - jej subory sa nezapisu
        Assert.AreSame(group.Stations[1].Files, renamed.Stations[1].Files);
    }

    [TestMethod]
    public void Premenovanie_SpolocneCisloSkopirujeZaznamy()
    {
        using var installation = new TestInstallation();
        var group = Open(installation, Train + Second).Groups[0];

        var renamed = TrainNumbers.Rename(group, 0, "2002", "2012", allTrains: false);

        Assert.AreEqual("2002", renamed.Timetable!.Trains[1].Variants[0].Stops[0].Number);
        Assert.AreEqual(4, TrainNumbers.RecordCount(renamed, "2002"));
        Assert.AreEqual(4, TrainNumbers.RecordCount(renamed, "2012"));
        CollectionAssert.AreEqual(new[] { "2002", "2012", "3003" }, renamed.Stations[0].Files!.Additions.Select(record => record.Number).ToList());

        var all = TrainNumbers.Rename(group, 0, "2002", "2012", allTrains: true);
        Assert.AreEqual("2012", all.Timetable!.Trains[1].Variants[0].Stops[0].Number);
        Assert.AreEqual(0, TrainNumbers.RecordCount(all, "2002"));
    }

    [TestMethod]
    public void Zmazanie_ZaznamyLenAkIchNepouzivaInyVlak()
    {
        using var installation = new TestInstallation();
        var group = Open(installation, Train + Second).Groups[0];
        var train = group.Timetable!.Trains[0];
        var without = group with { Timetable = group.Timetable with { Trains = [group.Timetable.Trains[1]] } };

        // druhy vlak ma to iste cislo v tych istych staniciach - zaznamy ostanu
        Assert.AreEqual(0, TrainNumbers.RemoveUnused(without, train).Removed);

        var empty = group with { Timetable = group.Timetable with { Trains = [] } };
        var (cleaned, removed) = TrainNumbers.RemoveUnused(empty, train);
        Assert.AreEqual(4, removed);
        Assert.AreEqual(0, TrainNumbers.RecordCount(cleaned, "2002"));
        Assert.AreEqual(1, cleaned.Stations[0].Files!.Additions.Count);
        Assert.IsEmpty(cleaned.Lockouts!);
    }
}
