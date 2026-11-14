using GVXEditor.Domain;
using GVXEditor.Domain.Boards;
using GVXEditor.Domain.Boards.Drafts;
using GVXEditor.Domain.Code;
using GVXEditor.Domain.Rules;
using GVXEditor.Domain.Server;
using GVXEditor.Domain.Views;
using ToolsCore.Iniss2.Tables;

namespace GVXEditor.Tests;

/// <summary>
/// Tabule a katalogove predlohy stanice: koncept bez zmeny, ulozenie len zmeneneho suboru s komentarmi, pravidla podla servera
/// a miesto chyby prekladu podmienky.
/// </summary>
[TestClass]
public class BoardsTests
{
    internal const string Tables = """
        <?xml version="1.0" encoding="windows-1250"?>
        <TABLES>
          <!-- nastupistia -->
          <t k="N1" n="Nástupiště 1" lin="Lipovec_Tabule" addr="2" light0="80">
            <part n="N1" ck="Nástupištní LCD" t="PLATFORM" rec="1" cond=".Druh &lt;&gt; &quot;Os&quot;">
              <pos k="1" />
              <pos k="2" pri="1" />
            </part>
          </t>
          <t k="Odjezdy" n="Odjezdy" addr="-1" save="5400010O.xml">
            <part ck="Odjezdová Internet" t="DEPARTURE" rec="10" />
          </t>
        </TABLES>
        """;

    internal const string Catalog = """
        <?xml version="1.0" encoding="windows-1250"?>
        <CATALOG>
          <t k="Nástupištní LCD" n="Nástupištní" cls="LCD1">
            <div k="Číslo" n="Číslo" inf="CisloVlaku" fnt="16" align="RIGHT">
              <pos c1="3" c2="10" />
            </div>
          </t>
          <t k="Odjezdová Internet" cls="INTERNET">
            <div k="Cíl" inf="CilNazev" trn="TRANSLATE" t1="Smer">
              <pos c1="0" c2="20" />
            </div>
          </t>
        </CATALOG>
        """;

    internal static TestInstallation Installation()
    {
        var installation = CodeTests.Installation();
        installation.Write(@"Server\Data\CommLines.xml", """<COMMLINES><line k="Lipovec_Tabule" typ="TCP_CLIENT" protocol="LCD" line="127.0.0.1:2000" /></COMMLINES>""");
        installation.Write(@"Server\Data\2030\01 Lipovec\Tables.xml", Tables);
        installation.Write(@"Server\Data\2030\01 Lipovec\TCatalog.xml", Catalog);
        return installation;
    }

    private static string Text(TestInstallation installation, string relativeFile) =>
        File.ReadAllText(Path.Combine(installation.DataDir, relativeFile), System.Text.Encoding.Latin1);

    [TestMethod]
    public void Tabule_BezZmenyAZmenaJasuAPodmienky()
    {
        using var installation = Installation();
        var workspace = Iniss2Workspace.Load(installation.DataDir);
        var group = workspace.Groups[0];
        var boards = group.Stations[0].Boards!;
        Assert.AreEqual(".Druh <> \"Os\"", boards.Tables[0].Parts[0].Condition);

        var edit = BoardsEdit.Of(0, boards);
        Assert.AreSame(boards, edit.Apply());

        edit.Tables[0].Light0 = "90";
        edit.Tables[0].Parts[0].Condition = ".Druh = \"R\"";
        edit.Tables[0].Parts[0].Tracks[1].Priority = "3";
        var applied = edit.Apply();
        Assert.AreSame(boards.Catalog, applied.Catalog);
        Assert.AreSame(boards.Tables[1], applied.Tables[1]);

        workspace.Change(0, group.WithStationBoards(0, applied), "tabule");
        Assert.AreEqual(SaveOutcome.Saved, workspace.Save().Outcome);
        var text = Text(installation, @"2030\01 Lipovec\Tables.xml");
        StringAssert.Contains(text, "<!-- nastupistia -->");
        StringAssert.Contains(text, "light0=\"90\"");
        StringAssert.Contains(text, "cond=\".Druh = &quot;R&quot;\"");
        StringAssert.Contains(text, "<pos k=\"2\" pri=\"3\" />");
        StringAssert.Contains(text, """<t k="Odjezdy" n="Odjezdy" addr="-1" save="5400010O.xml">""");
    }

    [TestMethod]
    public void Predlohy_NovyUdajANovaPredloha()
    {
        using var installation = Installation();
        var workspace = Iniss2Workspace.Load(installation.DataDir);
        var group = workspace.Groups[0];
        var edit = BoardsEdit.Of(0, group.Stations[0].Boards!);

        edit.Templates[0].Columns.Add(new ColumnDraft { Key = "Cas", Content = "CasOdjezdu", Start = "10", End = "15" });
        edit.Templates.Add(new TemplateDraft { Key = "Info", Class = "LCD1" });
        var applied = edit.Apply();
        Assert.AreSame(group.Stations[0].Boards!.Tables, applied.Tables);

        workspace.Change(0, group.WithStationBoards(0, applied), "predlohy");
        workspace.Save();
        var catalog = TableCatalogFile.Read(Path.Combine(installation.DataDir, "2030", "01 Lipovec"));
        Assert.AreEqual(("Cas", "CasOdjezdu", 10, 15), (catalog[0].Columns[1].Key, catalog[0].Columns[1].Content, catalog[0].Columns[1].Start, catalog[0].Columns[1].End));
        Assert.AreEqual("Info", catalog[2].Key);
        // prazdny druh obsahu a zarovnanie sa nezapisu (server by prazdnu hodnotu nevedel precitat)
        StringAssert.Contains(Text(installation, @"2030\01 Lipovec\TCatalog.xml"), """<div k="Cas" inf="CasOdjezdu">""");
    }

    [TestMethod]
    public void Pravidla_PodlaServera()
    {
        using var installation = Installation();
        var workspace = Iniss2Workspace.Load(installation.DataDir);
        var group = workspace.Groups[0];
        var context = BoardContext.Of(workspace, group, 0);
        var edit = BoardsEdit.Of(0, group.Stations[0].Boards!);
        Assert.IsEmpty(BoardRules.Check(edit, context));

        var table = edit.Tables[0];
        table.Line = "Neznama";
        table.LightTime0 = "6.00";
        table.Parts[0].Tracks.Add(new TrackRefDraft { Key = "9" });
        table.Parts[0].WebSource = "pocasie";
        table.Parts[0].Previous = "Odjezdy";
        edit.Templates[1].Columns[0].Table1 = "";
        edit.Tables.Add(new BoardDraft { Key = "N1" });

        var problems = BoardRules.Check(edit, context);
        var errors = problems.Where(problem => problem.IsError).Select(problem => problem.Message).ToList();
        var warnings = problems.Where(problem => !problem.IsError).Select(problem => problem.Message).ToList();
        // chyby: linka, cas jasu, chybajuca tabulka TabTab, tabula bez casti
        Assert.HasCount(4, errors, string.Join("\n", problems));
        // upozornenia: neznama kolaj, webovy zdroj mimo INFO, predchadzajuca tabula je neskor, kluc dvakrat
        Assert.HasCount(4, warnings, string.Join("\n", problems));
    }

    [TestMethod]
    public void Pravidla_MultioknaAPismaElen()
    {
        Assert.IsNull(BoardRules.WindowDefinitionError("0;1:2,4-6", 8));
        Assert.IsNotNull(BoardRules.WindowDefinitionError("8", 8));
        Assert.IsNotNull(BoardRules.WindowDefinitionError("1:6-4", 8));
        Assert.IsNotNull(BoardRules.WindowDefinitionError("1:8", 8));

        var edit = BoardsEdit.Of(0, new StationBoards(
            [new TableDefinition("E", "E", "", -1, "", [new TablePart("", "Elen", "DEPARTURE", 2)]),
             new TableDefinition("K", "K", "", -1, "", [new TablePart("", "Elekon", "DEPARTURE", 2)])],
            [new CatalogTable("Elen", "Elen", "ELEN16"), new CatalogTable("Elekon", "Elekon", "ELEKON") { FontBank = "Lipovec" }]));
        var context = new BoardContext("Lipovec", new HashSet<string>(), "", null, null, name => name == "Lipovec");
        // ELEN bez banky pisem, ELEKON s bankou
        Assert.HasCount(2, BoardRules.Check(edit, context).Where(problem => problem.IsError));
        edit.Templates[0].FontBank = "Lipovec";
        edit.Templates[1].FontBank = "";
        Assert.IsEmpty(BoardRules.Check(edit, context));
    }

    [TestMethod]
    public void Hodnoty_SlovamiSKlucomAHodnotyZDat()
    {
        Assert.AreEqual("Nástupištná (PLATFORM)", ValueTexts.PartKind("PLATFORM"));
        Assert.AreEqual("NEZNAMY", ValueTexts.NodeFlag("NEZNAMY"));
        var choices = ValueTexts.Choices(BoardValues.Aligns, ValueTexts.Align, BoardValues.Aligns[0], ["RIGHT", "MIDDLE"]);
        // predvolena hodnota (atribut chyba), zname hodnoty a hodnota zo suboru, ktoru server nepozna
        CollectionAssert.AreEqual(new[] { "", "LEFT", "RIGHT", "CENTER", "MIDDLE" }, choices.Select(choice => choice.Key).ToList());
        Assert.AreEqual("Predvolene: Vľavo", choices[0].Text);
    }

    [TestMethod]
    public void ChybaPrekladu_PodmienkaPodlaKodu()
    {
        var place = CodeErrorPlace.Of(new ServerCompileError { Source = "Tables.xml", Line = 1, Number = "BC30451", Code = "return .Neznama And True" });
        Assert.IsTrue(place.IsTables);
        Assert.IsTrue(place.MatchesCondition(".Neznama And True"));
        Assert.IsFalse(place.MatchesCondition(".Druh = \"R\""));
        var withReturn = CodeErrorPlace.Of(new ServerCompileError { Source = "Tables.xml", Code = "Return .Neznama" });
        Assert.IsTrue(withReturn.MatchesCondition("Return .Neznama"));
    }
}
