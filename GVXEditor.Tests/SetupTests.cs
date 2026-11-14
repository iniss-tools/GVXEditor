using GVXEditor.Domain;
using GVXEditor.Domain.Rules;
using GVXEditor.Domain.Setup;
using GVXEditor.Domain.Setup.Drafts;
using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Tests;

/// <summary>
/// Nastavenie grafikonu a stanic, hlasenia obsluhy a zmeny struktury: bez zmeny ziadny zapis, zmena ako krok historie
/// so zapisom na mieste, kontrola podla servera, novy grafikon a nova stanica s navratom pri chybe.
/// </summary>
[TestClass]
public class SetupTests
{
    private const string Train = """
        <v o="2030-12-15" d="2031-12-13">
          <v>
            <z c="5400010" o="480" f="Z" dcv="1001" dv="Os" />
            <z c="5400020" p="495" f="Z" />
          </v>
        </v>
        """;

    private static TestInstallation Installation()
    {
        var installation = new TestInstallation();
        installation.Trains(Train);
        installation.Write(@"Server\Data\2030\01 Lipovec\ReportKeys.xml", """
            <ReportKeys>
              <!-- typy hlaseni -->
              <r n="Přijede" k="P" v="V P K" />
              <r n="Odjede" k="O" v="V P" />
            </ReportKeys>
            """);
        installation.Write(@"Server\Data\2030\01 Lipovec\StaHlasB.xml", """
            <STAHLAS>
              <vzor><z s="5400005" /><z s="5400010" h="true" /></vzor>
            </STAHLAS>
            """);
        installation.Write(@"Server\Data\2030\01 Lipovec\TTTrasy.xml", """
            <TTEXTSTRC>
              <b n="TrasaDoSmeru">
                <r cat="Odjezdová" div="Směr" />
                <t t="Horany"><s k="5400010" /><s k="5400020" f="S" /></t>
              </b>
            </TTEXTSTRC>
            """);
        installation.Write(@"Server\Data\2030\01 Lipovec\TTStanice.xml", """
            <TTEXTSST>
              <b n="CiloveStanice">
                <r cat="Odjezdová" div="Cíl" />
                <s k="5400020" t="Horany" />
              </b>
            </TTEXTSST>
            """);
        installation.Write(@"Server\Data\2030\01 Lipovec\TTVlaky.xml", """
            <TTEXTS>
              <b n="Poznamka">
                <r cat="Odjezdová" div="Poznámka" />
                <t v="1001"><t s="Jede přes Javor" /></t>
              </b>
            </TTEXTS>
            """);
        installation.Write(@"Server\Data\2030\01 Lipovec\statedgm.xml", "<STATEDGM />");
        installation.Write(@"Server\Data\2030\Default.Stations.xml", """<STANICE><s st="5400020" n="Horany" /></STANICE>""");
        installation.Write(@"Server\Data\ConstReports.xml", """
            <CONSTREPORTS>
              <!-- hlasenia obsluhy -->
              <r k="4" n="Prestup" ik="Přestup"><s>5400020</s></r>
            </CONSTREPORTS>
            """);
        return installation;
    }

    private static string Text(TestInstallation installation, string relativeFile) =>
        File.ReadAllText(Path.Combine(installation.DataDir, relativeFile), System.Text.Encoding.Latin1);

    [TestMethod]
    public void Nastavenie_BezZmenyVratiTenIstyGrafikon()
    {
        using var installation = Installation();
        var workspace = Iniss2Workspace.Load(installation.DataDir);
        var group = workspace.Groups[0];

        var edit = GroupSetupEdit.Of(group);
        Assert.HasCount(1, edit.Station(0)!.RouteSets);
        Assert.IsNotNull(edit.Station(1));

        Assert.AreSame(group, edit.Apply(group));
        Assert.IsEmpty(SetupRules.Check(edit));
        Assert.IsEmpty(SetupRules.Check(edit.Station(0)!, "Lipovec"));
    }

    [TestMethod]
    public void Nastavenie_ZmenaKolajeAVzoruSaUloziNaMieste()
    {
        using var installation = Installation();
        var workspace = Iniss2Workspace.Load(installation.DataDir);
        var group = workspace.Groups[0];
        var edit = GroupSetupEdit.Of(group);
        var station = edit.Station(0)!;

        station.Tracks[1].Name = "2a";
        station.ShortPatterns[0].Stops.Add(StationRefDraft.Of(TestInstallation.Horany, true));
        var updated = edit.Apply(group);
        workspace.Change(0, updated, "nastavenie");

        Assert.AreSame(group.Stations[0].Setup!.ReportKeys, updated.Stations[0].Setup!.ReportKeys);
        Assert.AreSame(group.Stations[1], updated.Stations[1]);
        Assert.IsTrue(workspace.IsDirty);
        Assert.AreEqual(SaveOutcome.Saved, workspace.Save().Outcome);

        StringAssert.Contains(Text(installation, @"2030\01 Lipovec\PositionsDef.xml"), """<pos k="2" n="2a" """);
        var patterns = ReportPatternsFile.Read(Path.Combine(installation.DataDir, "2030", "01 Lipovec"), ReportPatternsFile.ShortFileName);
        Assert.HasCount(3, patterns[0]);
        StringAssert.Contains(Text(installation, @"2030\01 Lipovec\ReportKeys.xml"), "<!-- typy hlaseni -->");
        Assert.IsFalse(workspace.IsDirty);
    }

    [TestMethod]
    public void NazvyStanic_PlatiaUzPredUlozenim()
    {
        using var installation = Installation();
        var workspace = Iniss2Workspace.Load(installation.DataDir);
        var group = workspace.Groups[0];
        var edit = GroupSetupEdit.Of(group);

        edit.Names[1].Name = "Lipovec mesto";
        workspace.Change(0, edit.Apply(group), "nazvy");

        Assert.AreEqual("Lipovec mesto", workspace.StationName(TestInstallation.Lipovec));
        workspace.History.Undo();
        Assert.AreEqual("Lipovec", workspace.StationName(TestInstallation.Lipovec));
        workspace.History.Redo();
        workspace.Save();
        var text = Text(installation, @"2030\GV.stanice.XML");
        StringAssert.Contains(text, "<!-- stanice siete -->");
        StringAssert.Contains(text, """<s k="5400010" n="Lipovec mesto" />""");
    }

    [TestMethod]
    public void HlaseniaObsluhy_ZmenaJeKrokHistorie()
    {
        using var installation = Installation();
        var workspace = Iniss2Workspace.Load(installation.DataDir);
        var edit = ConstReportsEdit.Of(workspace.ConstReports);
        Assert.AreSame(workspace.ConstReports, edit.Apply());

        edit.Reports.Add(new ConstReportDraft { Key = "5", Name = "Odrieknutie", Function = "Odřeknutí", Stations = "5400010 5400020" });
        workspace.ChangeConstReports(edit.Apply(), "hlasenia");

        Assert.IsTrue(workspace.IsDirty);
        workspace.History.Undo();
        Assert.IsFalse(workspace.IsDirty);
        workspace.History.Redo();
        Assert.AreEqual(SaveOutcome.Saved, workspace.Save().Outcome);
        var text = Text(installation, "ConstReports.xml");
        StringAssert.Contains(text, "<!-- hlasenia obsluhy -->");
        CollectionAssert.AreEqual(new[] { TestInstallation.Lipovec, TestInstallation.Horany }, ConstReportsFile.Read(installation.DataDir)[1].Stations.ToList());
        Assert.AreEqual(6, ConstReportsEdit.Of(workspace.ConstReports).NextKey());
    }

    [TestMethod]
    public void Kontrola_NastavenieStanice()
    {
        using var installation = Installation();
        var edit = GroupSetupEdit.Of(Iniss2Workspace.Load(installation.DataDir).Groups[0]);
        var station = edit.Station(0)!;

        station.Tracks.Add(new TrackDraft { Key = "1", Name = "druha 1" });
        station.Tracks.Add(new TrackDraft { Separator = true });
        station.ReportKeys.Add(new ReportKeyDraft { Name = "Vjíždí", Key = "VV", Categories = "P X" });
        station.ShortPatterns.Add(PatternDraft.Of([new ReportPatternStop(TestInstallation.Javor, false), new ReportPatternStop(TestInstallation.Lipovec, false)]));
        station.LongPatterns.Add(new PatternDraft { Stops = { new StationRefDraft { Station = "abc" } } });
        station.RouteSets[0].Routes[0].Font = "x";
        station.StationSets[0].Texts.Add(new StationTextDraft { Station = "5400020", Text = "Horany 2" });

        var problems = SetupRules.Check(station, "Lipovec");

        // chyby: duplicitna kolaj, neznama kategoria, zly vzor, pismo; upozornenia: dlhe pismeno, rovnaky vzor, druhy text stanice
        Assert.AreEqual(4, problems.Count(problem => problem.IsError), string.Join("\n", problems));
        Assert.AreEqual(3, problems.Count(problem => !problem.IsError), string.Join("\n", problems));
    }

    [TestMethod]
    public void Kontrola_DuplicitneStaniceGrafikonuSuChyba()
    {
        using var installation = Installation();
        var edit = GroupSetupEdit.Of(Iniss2Workspace.Load(installation.DataDir).Groups[0]);

        edit.Names.Add(new StationNameDraft { Station = "5400010", Name = "Lipovec 2" });
        edit.DefaultStations.Add(new DefaultStationDraft { Station = "5400020" });
        edit.DefaultStations.Add(new DefaultStationDraft { Station = "0" });

        var problems = SetupRules.Check(edit);
        Assert.HasCount(3, problems);
        Assert.IsTrue(problems.All(problem => problem.IsError));
    }

    [TestMethod]
    public void Kontrola_HlaseniaObsluhy()
    {
        using var installation = Installation();
        var workspace = Iniss2Workspace.Load(installation.DataDir);
        var edit = ConstReportsEdit.Of(workspace.ConstReports);
        var functions = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase) { ["přestup"] = [] };

        edit.Reports[0].Key = "7";
        edit.Reports.Add(new ConstReportDraft { Key = "7", Name = "Dvojka", Function = "Neznama", Stations = "5400010 x" });
        edit.Reports.Add(new ConstReportDraft { Key = "8", Name = "Bez vzoru" });

        var problems = SetupRules.Check(edit, functions, new HashSet<int> { 4 });

        Assert.AreEqual(3, problems.Count(problem => problem.IsError), string.Join("\n", problems));
        // neznamy vzor a zmazane cislo 4, na ktore sa odkazuju zaznamy vlakov
        Assert.AreEqual(2, problems.Count(problem => !problem.IsError), string.Join("\n", problems));
    }

    [TestMethod]
    public void NovyGrafikon_BezVlakovSNastavenimPredlohy()
    {
        using var installation = Installation();
        installation.Write(@"Server\Data\2030\GV.XML.log", "log servera");
        var workspace = Iniss2Workspace.Load(installation.DataDir);
        Assert.IsNull(StructureEdits.CheckGroupName(workspace, "2031"));

        StructureEdits.NewGroup(workspace, "2031", 0, copyTrains: false);

        var reloaded = Iniss2Workspace.Load(installation.DataDir);
        Assert.HasCount(2, reloaded.Groups);
        var group = reloaded.Groups[1];
        Assert.AreEqual("2031", group.Group.Name);
        Assert.HasCount(2, group.Stations);
        Assert.IsEmpty(group.Timetable!.Trains);
        CollectionAssert.AreEqual(new[] { TestInstallation.Lipovec, TestInstallation.Horany }, group.Timetable.Stations.ToList());
        var lipovec = group.Stations[0];
        Assert.IsEmpty(lipovec.Files!.Positions);
        Assert.IsEmpty(lipovec.Files.TextSets.Single().Trains);
        Assert.HasCount(1, Iniss2Workspace.Load(installation.DataDir).Groups[0].Stations[0].Files!.TextSets.Single().Trains);
        Assert.HasCount(2, lipovec.Tracks);
        Assert.HasCount(3, group.Setup!.Names);
        Assert.IsFalse(File.Exists(Path.Combine(installation.DataDir, "2031", "GV.XML.log")));
        Assert.IsNotNull(StructureEdits.CheckGroupName(reloaded, "2031"));
        Assert.IsNotNull(StructureEdits.CheckGroupName(reloaded, "20/31"));
    }

    [TestMethod]
    public void NovyGrafikon_SVlakmiJeKopia()
    {
        using var installation = Installation();
        var workspace = Iniss2Workspace.Load(installation.DataDir);

        StructureEdits.NewGroup(workspace, "2031", 0, copyTrains: true);

        CollectionAssert.AreEqual(File.ReadAllBytes(Path.Combine(installation.DataDir, "2030", "GV.XML")),
            File.ReadAllBytes(Path.Combine(installation.DataDir, "2031", "GV.XML")));
        Assert.HasCount(2, Iniss2Workspace.Load(installation.DataDir).Groups[1].Stations[0].Files!.Positions);
    }

    [TestMethod]
    public void NovaStanica_ZPredlohyBezZaznamovVlakov()
    {
        using var installation = Installation();
        var workspace = Iniss2Workspace.Load(installation.DataDir);
        var station = new NewStation(0, "03 Brezova", 5400030, "Brezová", DirListItemFlags.Stop, 0, 0);
        Assert.IsEmpty(StructureEdits.Check(workspace, station));
        Assert.HasCount(2, StructureEdits.Check(workspace, station with { Directory = "01 lipovec", Station = TestInstallation.Horany }));

        StructureEdits.AddStation(workspace, station);

        var reloaded = Iniss2Workspace.Load(installation.DataDir);
        var added = reloaded.Groups[0].Stations[2];
        Assert.AreEqual(("03 Brezova", 5400030, DirListItemFlags.Stop), (added.Station.Directory, added.Station.Station, added.Station.Flags));
        Assert.IsEmpty(added.Files!.Positions);
        Assert.HasCount(2, added.Tracks);
        Assert.AreEqual("Brezová", reloaded.StationName(5400030));
        StringAssert.Contains(Text(installation, @"2030\GV.stanice.XML"), "<!-- stanice siete -->");
        Assert.AreEqual("03 Brezova", StructureEdits.SuggestDirectory(3, "Brezova"));
    }

    [TestMethod]
    public void NovaStanica_ChybaZapisuVratiZmeny()
    {
        using var installation = Installation();
        var workspace = Iniss2Workspace.Load(installation.DataDir);
        var names = Path.Combine(installation.DataDir, "2030", GvStationsFile.FileName);
        var dirList = File.ReadAllBytes(Path.Combine(installation.DataDir, DirListXml.FileName));
        File.SetAttributes(names, FileAttributes.ReadOnly);
        try
        {
            Assert.ThrowsExactly<UnauthorizedAccessException>(() =>
                StructureEdits.AddStation(workspace, new NewStation(0, "03 Brezova", 5400030, "Brezová", DirListItemFlags.None, 0, 0)));
        }
        finally
        {
            File.SetAttributes(names, FileAttributes.Normal);
        }

        Assert.IsFalse(Directory.Exists(Path.Combine(installation.DataDir, "2030", "03 Brezova")));
        CollectionAssert.AreEqual(dirList, File.ReadAllBytes(Path.Combine(installation.DataDir, DirListXml.FileName)));
    }
}
