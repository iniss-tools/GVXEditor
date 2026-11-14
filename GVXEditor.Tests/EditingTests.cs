using System.Globalization;
using GVXEditor.Domain.Editing;
using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Tests;

/// <summary>
/// Koncept vlaku z okna Upravit vlak (dni cez polnoc, kluc Z) a zmeny vlakov grafikonu.
/// </summary>
[TestClass]
public class EditingTests
{
    private static readonly IReadOnlySet<int> Interest = new HashSet<int> { TestInstallation.Lipovec, TestInstallation.Horany };

    internal static StopDraft Stop(int station, string arrival, string departure, string number = "") => new()
    {
        Station = station.ToString(CultureInfo.InvariantCulture),
        Arrival = arrival,
        Departure = departure,
        Number = number
    };

    internal static TrainDraft Draft(params StopDraft[] stops)
    {
        var draft = TrainDraft.New(new DateOnly(2030, 12, 15), new DateOnly(2031, 12, 13));
        draft.Variants[0].Stops.Clear();
        foreach (var stop in stops)
            draft.Variants[0].Stops.Add(stop);
        return draft;
    }

    private static GvTrain Train(string number, DateOnly from, DateOnly to) => new(from, to, [
        new GvVariant
        {
            Validity = "jede v 1-5",
            Stops = [new GvStop { Station = TestInstallation.Lipovec, Departure = 480, Number = number }, new GvStop { Station = TestInstallation.Horany, Arrival = 495 }]
        }
    ]);

    [TestMethod]
    public void Koncept_DniCezPolnocAKlucZPodlaStanicGrafikonu()
    {
        var draft = Draft(Stop(TestInstallation.Javor, "", "23:40", "1001"), Stop(TestInstallation.Lipovec, "23:55", "0:05"),
            Stop(TestInstallation.Horany, "015", ""));

        var stops = draft.ToTrain(Interest).Variants[0].Stops;

        Assert.AreEqual((null, 1420, 0, GvStopFlags.None), (stops[0].Arrival, stops[0].Departure, stops[0].DepartureDay, stops[0].Flags));
        Assert.AreEqual((1435, 0, 5, 1), (stops[1].Arrival, stops[1].ArrivalDay, stops[1].Departure, stops[1].DepartureDay));
        Assert.AreEqual(GvStopFlags.Interest, stops[1].Flags);
        Assert.AreEqual((15, 1, GvStopFlags.Interest), (stops[2].Arrival, stops[2].ArrivalDay, stops[2].Flags));
    }

    [TestMethod]
    public void Koncept_BezKoncaPlatnosti()
    {
        var draft = Draft(Stop(TestInstallation.Lipovec, "", "8:00", "1"), Stop(TestInstallation.Horany, "8:15", ""));
        draft.NoEnd = true;

        var train = draft.ToTrain(Interest);

        Assert.AreEqual(DateOnly.MaxValue, train.To);
        Assert.AreEqual(train.From.AddDays(364), TrainDraft.Of(train).To);
        Assert.IsTrue(TrainDraft.Of(train).NoEnd);
    }

    [TestMethod]
    public void Koncept_VlakZoSuboruSaPrevedieSpatBezZmeny()
    {
        using var installation = new TestInstallation();
        installation.Trains("""
            <v o="2030-12-15" d="2031-12-13">
              <v jd="jede v 6" d="Fiktivní dráhy" f="D" p="sobota">
                <z c="5400005" o="1420" dcv="1001" dv="Os" nv="Javor" />
                <z c="5400010" p="1435" o="1437" f="Z PR X" dcv="2002" cv="2002A" dv="Sp" l="X5" ids="S1" />
                <z c="5400030" f="P DB" />
                <z c="5400020" p="15" cp="1" f="Z" />
              </v>
            </v>
            """);
        var original = Domain.Iniss2Workspace.Load(installation.DataDir).Groups[0];
        var train = original.Timetable!.Trains[0];

        var back = TrainDraft.Of(train).ToTrain(original.InterestStations);

        Assert.AreEqual((train.From, train.To), (back.From, back.To));
        var (expected, actual) = (train.Variants[0], back.Variants[0]);
        Assert.AreEqual((expected.Validity, expected.Operator, expected.Flags, expected.Note), (actual.Validity, actual.Operator, actual.Flags, actual.Note));
        CollectionAssert.AreEqual(expected.Stops.ToList(), actual.Stops.ToList());
        Assert.IsTrue(TimetableEditing.SameTrain(train, back));
        Assert.IsFalse(TimetableEditing.SameTrain(train, back with { To = back.To.AddDays(1) }));
    }

    [TestMethod]
    public void Zmeny_NovyNaKoniecKopiaZaVlakOdstranenie()
    {
        var first = Train("1", new DateOnly(2030, 12, 15), new DateOnly(2031, 12, 13));
        var second = Train("2", new DateOnly(2030, 12, 15), new DateOnly(2031, 12, 13));
        var timetable = new GvFile([TestInstallation.Lipovec], [first]);

        var added = TimetableEditing.Add(timetable, second);
        var copied = TimetableEditing.Copy(added, 0);
        var removed = TimetableEditing.Remove(copied, 2);

        Assert.HasCount(1, timetable.Trains);
        CollectionAssert.AreEqual(new[] { first, second }, added.Trains.ToList());
        CollectionAssert.AreEqual(new[] { first, first, second }, copied.Trains.ToList());
        CollectionAssert.AreEqual(new[] { first, first }, removed.Trains.ToList());
        Assert.AreSame(second, TimetableEditing.Replace(timetable, 0, second).Trains[0]);
    }

    [TestMethod]
    public void Zmeny_PosunPlatnostiNechaPoznamkyABezKonca()
    {
        var bounded = Train("1", new DateOnly(2030, 12, 15), new DateOnly(2031, 12, 13));
        var open = Train("2", new DateOnly(2030, 12, 15), DateOnly.MaxValue);
        var timetable = new GvFile([TestInstallation.Lipovec], [bounded, open]);

        var shifted = TimetableEditing.ShiftValidity(timetable, 364).Trains;

        Assert.AreEqual((new DateOnly(2031, 12, 14), new DateOnly(2032, 12, 11)), (shifted[0].From, shifted[0].To));
        Assert.AreEqual((new DateOnly(2031, 12, 14), DateOnly.MaxValue), (shifted[1].From, shifted[1].To));
        Assert.AreEqual("jede v 1-5", shifted[0].Variants[0].Validity);
    }
}
