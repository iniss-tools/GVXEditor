using GVXEditor.Domain;
using GVXEditor.Domain.Code;
using GVXEditor.Domain.Code.Drafts;
using GVXEditor.Domain.Rules;
using GVXEditor.Domain.Server;
using ToolsCore.Iniss2.Grafikon;
using ToolsCore.Iniss2.Tables;

namespace GVXEditor.Tests;

/// <summary>
/// Kod, ktory server preklada: vlozeny kod v XML, koncepty stavoveho diagramu a tabuliek TabTab, pravidla, miesto chyby
/// prekladu a ulozenie so zachovanim komentarov.
/// </summary>
[TestClass]
public class CodeTests
{
    internal const string StateDiagram = """
        <?xml version="1.0" encoding="windows-1250"?>
        <SD indg=".IndCat3">
          <g ds="#Výchozí vlak">
        <!-- pomocne stavy -->
            <n k="Kolej" f="AUXSTATE">
              <b t="Kolej" p="0" />
            </n>
            <n k="#Start" f="STOJI JENAODJEZDOVE" />
            <n k="Nástup" r="Přijede" f="STOJI">
              <c>.PlanDobaPobytu &lt; 5</c>
              <b t="Nástup" p="6" />
              <a t=".DatumCasOdj(True).AddSeconds(-600)" m="1" h="2" />
            </n>
          </g>
        </SD>
        """;

    internal const string TabTab = """
        <?xml version="1.0" encoding="windows-1250"?>
        <TABTAB>
          <!-- tabulky grafikonu -->
          <t n="PRIZNAK">
            <expr>
              Select Case .Druh
                Case "R"
                    Return "(červené pole)"
              End Select
            </expr>
            <k>
              <k s="(červené pole)" f="-2" />
              <v s="&#127;" f="-2" />
            </k>
          </t>
          <t n="Smer">
            <expr>
              Return sOrig
            </expr>
          </t>
        </TABTAB>
        """;

    internal static TestInstallation Installation()
    {
        var installation = new TestInstallation();
        installation.Trains("");
        installation.Write(@"Server\Data\2030\01 Lipovec\statedgm.xml", StateDiagram);
        installation.Write(@"Server\Data\2030\TabTabGlobal.xml", TabTab);
        installation.Write(@"Server\Data\2030\01 Lipovec\ReportKeys.xml", """<ReportKeys><r n="Přijede" k="P" v="V P K" /></ReportKeys>""");
        installation.Write(@"Server\Data\LogText.vb", "Public Class LogText\r\n    <Dodatek(\"Bufet\")>\r\n    Public Function Bufet() As Boolean\r\n        Return True\r\n    End Function\r\nEnd Class\r\n");
        return installation;
    }

    private static string Text(TestInstallation installation, string relativeFile) =>
        File.ReadAllText(Path.Combine(installation.DataDir, relativeFile), System.Text.Encoding.Latin1);

    [TestMethod]
    public void VlozenyKod_ZobrazenieAZapisSOdsadenim()
    {
        const string raw = "\n      Select Case .Druh\n        Case \"R\"\n            Return \"x\"\n      End Select\n    ";

        Assert.AreEqual("Select Case .Druh\n  Case \"R\"\n      Return \"x\"\nEnd Select", EmbeddedCode.Display(raw));
        Assert.AreEqual(1, EmbeddedCode.LineOffset(raw));
        Assert.AreSame(raw, EmbeddedCode.Store(EmbeddedCode.Display(raw), raw, "  ", ""));
        Assert.AreEqual("\n      Return sOrig\n    ", EmbeddedCode.Store("Return sOrig", raw, "  ", ""));
        Assert.AreEqual(".DatumOdj", EmbeddedCode.Store(".DatumOdj", "", "      ", "    "));
        Assert.AreEqual("\n      If x Then\n        Return 1\n      End If\n    ", EmbeddedCode.Store("If x Then\n  Return 1\nEnd If", "", "      ", "    "));
    }

    [TestMethod]
    public void StavovyDiagram_BezZmenyAZmenaPodmienky()
    {
        using var installation = Installation();
        var workspace = Iniss2Workspace.Load(installation.DataDir);
        var group = workspace.Groups[0];
        var diagram = group.Stations[0].Code!.Diagram!;

        var edit = StateDiagramEdit.Of(0, diagram);
        Assert.AreSame(diagram, edit.Apply());

        var node = edit.Graphs[0].Nodes[2];
        Assert.AreEqual(".PlanDobaPobytu < 5", node.Condition);
        node.Condition = ".PlanDobaPobytu < 10";
        node.Flags.Add("NYNISTOJI");
        var applied = edit.Apply()!;
        Assert.AreSame(diagram.Graphs[0].Nodes[0], applied.Graphs[0].Nodes[0]);

        workspace.Change(0, group.WithStationCode(0, group.Stations[0].Code! with { Diagram = applied }), "diagram");
        Assert.AreEqual(SaveOutcome.Saved, workspace.Save().Outcome);
        var text = Text(installation, @"2030\01 Lipovec\statedgm.xml");
        StringAssert.Contains(text, "<!-- pomocne stavy -->");
        StringAssert.Contains(text, "<c>.PlanDobaPobytu &lt; 10</c>");
        StringAssert.Contains(text, "f=\"STOJI NYNISTOJI\"");
        StringAssert.Contains(text, """<a t=".DatumCasOdj(True).AddSeconds(-600)" m="1" h="2" />""");
    }

    [TestMethod]
    public void TabTab_ZmenaVyrazuZachovaOdsadenieAOstatneTabulky()
    {
        using var installation = Installation();
        var workspace = Iniss2Workspace.Load(installation.DataDir);
        var group = workspace.Groups[0];

        var edit = TabTabEdit.Of(-1, group.GlobalTabTabs!);
        Assert.AreSame(group.GlobalTabTabs, edit.Apply());

        edit.Tables[1].Expression = "If .JeOdklonen Then Return \"ODKLON\"\nReturn sOrig";
        var tables = edit.Apply();
        Assert.AreSame(group.GlobalTabTabs![0], tables[0]);

        workspace.Change(0, group with { GlobalTabTabs = tables }, "tabtab");
        workspace.Save();
        var text = Text(installation, @"2030\TabTabGlobal.xml");
        StringAssert.Contains(text, "<!-- tabulky grafikonu -->");
        StringAssert.Contains(text, "    <expr>\n      If .JeOdklonen Then Return \"ODKLON\"\n      Return sOrig\n    </expr>");
        Assert.AreEqual(group.GlobalTabTabs[0].Expression, TabTabXml.Read(Path.Combine(installation.DataDir, "2030", TabTabXml.GlobalFileName))[0].Expression);
    }

    [TestMethod]
    public void SkriptHlaseni_ZmenaJeKrokHistorieAFunkciePodlaStavu()
    {
        using var installation = Installation();
        var workspace = Iniss2Workspace.Load(installation.DataDir);
        Assert.IsTrue(workspace.Functions!.ContainsKey("Bufet"));

        var text = workspace.History.Current.LogText!.Replace("Bufet()", "Jedalen()", StringComparison.Ordinal);
        workspace.ChangeLogText(text, "skript");

        Assert.IsTrue(workspace.IsDirty);
        Assert.IsTrue(workspace.Functions!.ContainsKey("Jedalen"));
        workspace.History.Undo();
        Assert.IsFalse(workspace.IsDirty);
        Assert.IsTrue(workspace.Functions!.ContainsKey("Bufet"));
        workspace.History.Redo();
        Assert.AreEqual(SaveOutcome.Saved, workspace.Save().Outcome);
        StringAssert.Contains(Text(installation, "LogText.vb"), "Public Function Jedalen() As Boolean\r\n");
    }

    [TestMethod]
    public void Pravidla_StavovyDiagramATabTab()
    {
        var diagram = StateDiagramEdit.Of(0, null);
        diagram.Graphs.Add(new GraphDraft { Description = "g" });
        diagram.Graphs[0].Nodes.Add(new NodeDraft { Key = "A", HasButton = true, ButtonPosition = "5", Flags = { "NEZNAMY" }, Report = "Odjede" });
        diagram.Graphs[0].Nodes.Add(new NodeDraft { Key = "A", HasButton = true, ButtonPosition = "3", HasAutomation = true });

        var problems = CodeRules.Check(diagram, "Lipovec", new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase) { ["Přijede"] = [] });
        // chyby: bez cisla grafu, neznamy priznak, automatika bez casu; upozornenia: kluc dvakrat, hlasenie mimo skriptu, poradie tlacidla
        Assert.AreEqual(3, problems.Count(problem => problem.IsError), string.Join("\n", problems));
        Assert.AreEqual(3, problems.Count(problem => !problem.IsError), string.Join("\n", problems));

        var tabTab = TabTabEdit.Of(0, [new TabTabTable("PRIZNAK"), new TabTabTable("Smer") { IgnoreCase = true }]);
        tabTab.Tables[1].Entries.Add(new EntryDraft { KeyText = "A", KeyFont = "*" });
        tabTab.Tables[1].Entries.Add(new EntryDraft { KeyText = "a", KeyFont = "*", ValueFont = "x" });
        var tables = CodeRules.Check(tabTab, "stanica", new HashSet<string> { "PRIZNAK" });
        // chyby: nazov spolocnej tabulky, rovnaky kluc bez ohladu na velkost, zle pismo
        Assert.AreEqual(3, tables.Count(problem => problem.IsError), string.Join("\n", tables));
    }

    [TestMethod]
    public void MiestoChyby_ZPopisuServera()
    {
        var place = CodeErrorPlace.Of(new ServerCompileError
        {
            Source = "statedgm.xml", Line = 1, Number = "BC30451",
            Description = "Soubor \"StateDgm.xml\" v \"2030:01 Lipovec\", graf \"#Výchozí vlak\", uzel \"Nástup\", automatika funkce \"t\""
        });
        Assert.AreEqual(("2030", "01 Lipovec", "#Výchozí vlak", "Nástup", "t", true), (place.Group, place.Station, place.Graph, place.Node, place.Field, place.Automation));

        var index = CodeErrorPlace.Of(new ServerCompileError { Source = "statedgm.xml", Description = "Soubor \"StateDgm.xml\" v \"2030:01 Lipovec\", funkce \"indg\"" });
        Assert.AreEqual(("01 Lipovec", "", "indg", false), (index.Station, index.Node, index.Field, index.Automation));

        var table = CodeErrorPlace.Of(new ServerCompileError { Source = "TabTab", Description = "PRIZNAK", Number = "BC42024" });
        Assert.AreEqual(("PRIZNAK", true), (table.Table, table.IsWarning));
    }
}
