using GVXEditor.Domain;
using GVXEditor.Domain.Views;
using GVXEditor.Properties;

namespace GVXEditor.Tests;

/// <summary>
/// Otvorenie instalacie a riadky hlavneho okna: vlaky grafikonu, varianty, trasa a vlaky stanice v den.
/// </summary>
[TestClass]
public class WorkspaceTests
{
    // osobak 1001 z Javora do Lipovca, tam sa zmeni na rychlik 2002 do Horian; v soboty ide neskor a cez polnoc
    private const string Train = """
        <v o="2030-12-15" d="2031-12-13">
          <v jd="jede v 6" d="Fiktivní dráhy" f="D">
            <z c="5400005" o="1420" dcv="1001" dv="Os" nv="Javor" />
            <z c="5400010" p="1435" o="1437" f="Z" dcv="2002" dv="Sp" ids="S1" />
            <z c="5400020" p="15" cp="1" f="Z" />
          </v>
          <v p="hlavna varianta">
            <z c="5400005" o="480" dcv="1001" dv="Os" nv="Javor" />
            <z c="5400010" p="495" o="497" f="Z" dcv="2002" dv="Sp" />
            <z c="5400020" p="510" f="Z" />
          </v>
        </v>
        """;

    private static Iniss2Workspace Open(TestInstallation installation) => Iniss2Workspace.Load(installation.DataDir);

    [TestMethod]
    public void Instalacia_GrafikonyANazvyStanic()
    {
        using var installation = new TestInstallation();
        installation.Trains(Train);

        var workspace = Open(installation);

        Assert.AreEqual(Path.Combine(installation.Root, "Server"), workspace.ServerDir);
        Assert.HasCount(1, workspace.Groups);
        Assert.IsNull(workspace.Groups[0].Error);
        Assert.HasCount(1, workspace.Groups[0].Timetable!.Trains);
        Assert.AreEqual("Javor", workspace.StationName(TestInstallation.Javor));
        Assert.AreEqual("5400099", workspace.StationName(5400099));
        Assert.IsFalse(workspace.IsServerRunning());
    }

    [TestMethod]
    public void Instalacia_ChybnyGrafikonOstaneSChybou()
    {
        using var installation = new TestInstallation();
        installation.Trains("""<v><v f="NEZNAMY"><z c="5400010" /></v></v>""");

        var group = Open(installation).Groups.Single();

        Assert.IsNull(group.Timetable);
        Assert.IsFalse(string.IsNullOrEmpty(group.Error));
    }

    [TestMethod]
    public void Vlak_CislaTrasaAPlatnostZPrvejVarianty()
    {
        using var installation = new TestInstallation();
        installation.Trains(Train);
        var workspace = Open(installation);

        var row = TrainRow.Create(workspace.Groups[0].Timetable!.Trains[0], workspace.StationName);

        Assert.AreEqual(("1001 / 2002", "Os", "Javor"), (row.Numbers, row.Type, row.Name));
        Assert.AreEqual(("Javor", "23:40", "Horany", "00:15 +1"), (row.From, row.Departure, row.To, row.Arrival));
        Assert.AreEqual(2, row.Variants);
        Assert.AreEqual("Fiktivní dráhy", row.Operator);
        StringAssert.Contains(row.Validity, new DateOnly(2031, 12, 13).ToString("d", System.Globalization.CultureInfo.CurrentCulture));
    }

    [TestMethod]
    public void Varianty_DniBezDniSkorsichVariant()
    {
        using var installation = new TestInstallation();
        installation.Trains(Train);
        var train = Open(installation).Groups[0].Timetable!.Trains[0];

        var rows = VariantRow.Create(train);
        var all = train.To.DayNumber - train.From.DayNumber + 1;

        Assert.AreEqual(("jede v 6", "diaľkový"), (rows[0].Validity, rows[0].Flags));
        Assert.AreEqual((Resources.Validity_Daily, all - rows[0].Days), (rows[1].Validity, rows[1].Days));
        Assert.AreEqual("hlavna varianta", rows[1].Note);
    }

    [TestMethod]
    public void Trasa_VsetkyUdajeBodu()
    {
        using var installation = new TestInstallation();
        installation.Trains(Train);
        var workspace = Open(installation);

        var stops = StopRow.Create(workspace.Groups[0].Timetable!.Trains[0].Variants[0], workspace.StationName);

        Assert.HasCount(3, stops);
        Assert.AreEqual(("Lipovec", "5400010", "23:55", "23:57", "záujmová"), (stops[1].Station, stops[1].Code, stops[1].Arrival, stops[1].Departure, stops[1].Flags));
        Assert.AreEqual(("2002", "Sp", "S1"), (stops[1].Number, stops[1].Type, stops[1].Line));
        Assert.AreEqual("00:15 +1", stops[2].Arrival);
    }

    [TestMethod]
    public void StanicaNaDen_PlatnaVariantaCislaAKolaj()
    {
        using var installation = new TestInstallation();
        installation.Trains(Train);
        var workspace = Open(installation);
        var group = workspace.Groups[0];
        var timetable = workspace.StationTimetable(group, group.Group.Items[0]);

        // 2031-01-04 je sobota, 2031-01-06 pondelok
        var saturday = StationDayRow.Create(timetable.On(new DateOnly(2031, 1, 4)), workspace.StationName).Single();
        var monday = StationDayRow.Create(timetable.On(new DateOnly(2031, 1, 6)), workspace.StationName).Single();

        Assert.AreEqual(("23:55", "23:57", "1001", "2002", "2", 1), (saturday.Arrival, saturday.Departure, saturday.ArrivalNumber, saturday.DepartureNumber,
            saturday.Track, saturday.Variant));
        Assert.AreEqual(("08:15", "1", 2), (monday.Arrival, monday.Track, monday.Variant));
        Assert.AreEqual(("Javor", "Horany"), (monday.From, monday.To));
    }

    [TestMethod]
    public void StanicaNaDen_PrichodPoPolnociVDalsiDen()
    {
        using var installation = new TestInstallation();
        installation.Trains(Train);
        var workspace = Open(installation);
        var group = workspace.Groups[0];
        var timetable = workspace.StationTimetable(group, group.Group.Items[1]);

        // sobotny vlak pride do Horian v nedelu po polnoci
        var sunday = StationDayRow.Create(timetable.On(new DateOnly(2031, 1, 5)), workspace.StationName);

        Assert.AreEqual("00:15", sunday.Single(row => row.Arrival == "00:15").Arrival);
        Assert.AreEqual("", sunday.Single(row => row.Arrival == "00:15").Departure);
    }

    [TestMethod]
    [DataRow(0, 0, "00:00")]
    [DataRow(495, 0, "08:15")]
    [DataRow(15, 1, "00:15 +1")]
    public void Cas_HodinyMinutyADen(int minutes, int day, string expected) => Assert.AreEqual(expected, TimeText.Format(minutes, day));
}
