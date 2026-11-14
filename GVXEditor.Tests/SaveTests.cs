using GVXEditor.Domain;
using GVXEditor.Domain.Editing;
using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Tests;

/// <summary>
/// Zmeny ako kroky historie a ulozenie: len pri zastavenom serveri, subor zmeneny inym programom len so suhlasom,
/// zapis na mieste so zachovanim komentarov.
/// </summary>
[TestClass]
public class SaveTests
{
    private const string Trains = """
        <!-- osobak rano -->
        <v o="2030-12-15" d="2031-12-13">
          <v>
            <z c="5400010" o="480" f="Z" dcv="1001" dv="Os" />
            <z c="5400020" p="495" f="Z" />
          </v>
        </v>
        <!-- osobak vecer -->
        <v o="2030-12-15" d="2031-12-13">
          <v>
            <z c="5400010" o="1080" f="Z" dcv="1003" dv="Os" />
            <z c="5400020" p="1095" f="Z" />
          </v>
        </v>
        """;

    private static string File(TestInstallation installation) => Path.Combine(installation.DataDir, "2030", GvFile.FileName);

    // posunie odchod prveho vlaku na 8:10 a zaznamena krok
    private static void ChangeDeparture(Iniss2Workspace workspace)
    {
        var timetable = workspace.Groups[0].Timetable!;
        var draft = TrainDraft.Of(timetable.Trains[0]);
        draft.Variants[0].Stops[0].Departure = "8:10";
        workspace.Change(0, TimetableEditing.Replace(timetable, 0, draft.ToTrain(workspace.Groups[0].InterestStations)), "uprava");
    }

    [TestMethod]
    public void Ulozenie_ZapiseZmenuNaMiesteSKomentarmi()
    {
        using var installation = new TestInstallation();
        installation.Trains(Trains);
        var workspace = Iniss2Workspace.Load(installation.DataDir);

        ChangeDeparture(workspace);
        Assert.IsTrue(workspace.IsDirty);
        CollectionAssert.AreEqual(new[] { 0 }, workspace.DirtyGroups());

        Assert.AreEqual(SaveOutcome.Saved, workspace.Save().Outcome);

        Assert.IsFalse(workspace.IsDirty);
        var text = System.IO.File.ReadAllText(File(installation), System.Text.Encoding.Latin1);
        StringAssert.Contains(text, "<!-- osobak rano -->");
        StringAssert.Contains(text, "<!-- osobak vecer -->");
        StringAssert.Contains(text, """<z c="5400010" o="490" f="Z" dcv="1001" dv="Os" />""");
        Assert.AreEqual(490, Iniss2Workspace.Load(installation.DataDir).Groups[0].Timetable!.Trains[0].Variants[0].Stops[0].Departure);
    }

    [TestMethod]
    public void Ulozenie_SpatPoUlozeniJeNeulozenaZmena()
    {
        using var installation = new TestInstallation();
        installation.Trains(Trains);
        var workspace = Iniss2Workspace.Load(installation.DataDir);
        ChangeDeparture(workspace);
        workspace.Save();

        workspace.History.Undo();

        Assert.IsTrue(workspace.IsDirty);
        Assert.AreEqual(480, workspace.Groups[0].Timetable!.Trains[0].Variants[0].Stops[0].Departure);
        workspace.History.Redo();
        Assert.IsFalse(workspace.IsDirty);
    }

    [TestMethod]
    public void Ulozenie_BezZmienNicNezapise()
    {
        using var installation = new TestInstallation();
        installation.Trains(Trains);
        var before = System.IO.File.GetLastWriteTimeUtc(File(installation));

        Assert.AreEqual(SaveOutcome.NothingToSave, Iniss2Workspace.Load(installation.DataDir).Save().Outcome);
        Assert.AreEqual(before, System.IO.File.GetLastWriteTimeUtc(File(installation)));
    }

    [TestMethod]
    public void Ulozenie_PriBeziacomServeriNezapise()
    {
        using var installation = new TestInstallation();
        installation.Trains(Trains);
        installation.Write(@"Server\IniSrv.exe", "");
        var original = System.IO.File.ReadAllBytes(File(installation));
        var workspace = Iniss2Workspace.Load(installation.DataDir);
        ChangeDeparture(workspace);

        SaveOutcome outcome;
        using (new FileStream(Path.Combine(installation.Root, "Server", "IniSrv.exe"), FileMode.Open, FileAccess.Read, FileShare.Read))
            outcome = workspace.Save().Outcome;

        Assert.AreEqual(SaveOutcome.ServerRunning, outcome);
        CollectionAssert.AreEqual(original, System.IO.File.ReadAllBytes(File(installation)));
        Assert.IsTrue(workspace.IsDirty);
        Assert.AreEqual(SaveOutcome.Saved, workspace.Save().Outcome);
    }

    [TestMethod]
    public void Ulozenie_SuborZmenenyInymProgramomLenSoSuhlasom()
    {
        using var installation = new TestInstallation();
        installation.Trains(Trains);
        var workspace = Iniss2Workspace.Load(installation.DataDir);
        ChangeDeparture(workspace);
        System.IO.File.SetLastWriteTimeUtc(File(installation), DateTime.UtcNow.AddMinutes(5));

        var result = workspace.Save();

        Assert.AreEqual(SaveOutcome.ChangedOnDisk, result.Outcome);
        CollectionAssert.AreEqual(new[] { File(installation) }, result.ChangedFiles.ToList());
        Assert.IsTrue(workspace.IsDirty);
        Assert.AreEqual(SaveOutcome.Saved, workspace.Save(overwriteChanged: true).Outcome);
        Assert.AreEqual(SaveOutcome.NothingToSave, workspace.Save().Outcome);
    }

    [TestMethod]
    public void StanicaNaDen_NeulozeneZmenyUzVidno()
    {
        using var installation = new TestInstallation();
        installation.Trains(Trains);
        var workspace = Iniss2Workspace.Load(installation.DataDir);
        ChangeDeparture(workspace);
        var group = workspace.Groups[0];

        var calls = workspace.StationTimetable(group, group.Group.Items[0]).On(new DateOnly(2031, 1, 6));

        CollectionAssert.AreEqual(new[] { 490, 1080 }, calls.Select(call => call.Minute).ToList());
    }
}
