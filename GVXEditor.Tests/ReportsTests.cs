using System.Reflection;
using System.Windows.Forms;
using GVXEditor.Config;
using GVXEditor.Domain;
using GVXEditor.Domain.Code;
using GVXEditor.Domain.Code.Drafts;
using GVXEditor.Domain.Rules;
using GVXEditor.Domain.Setup;
using GVXEditor.Properties;
using GVXEditor.UI.Code;
using ToolsCore;
using ToolsCore.Iniss2.Grafikon.Script;
using ToolsCore.XML;

namespace GVXEditor.Tests;

/// <summary>
/// Okno Hlasenia a jeho domena: hlasenia skriptu ako udaje, koncept tam a spat, zapis len upravenej funkcie, kontrola,
/// odkazy z dat a nahlad textu z banky.
/// </summary>
[TestClass]
public class ReportsTests
{
    private const BindingFlags Any = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private static readonly string Script = """
        Public Class LogText
            Inherits LogTextBase

            <Dodatek("Zmena radenia")> _
            Public Sub D1006()
                Zvuk("CZ/Slova/razeni")
            End Sub ' D1006

            <OstatniHlaseni("Výzva")> _
            Public Sub Vyzva()
                Zvuk("CZ/Slova/vyzva")
                'Zvuk("GB/Slova/vyzva")
            End Sub

            <ZakladniHlaseni("Přijede")> _
            Public Sub Přijede()
                Dim kolaj = VlakS.PozicePrijSkut
                If VlakS.JeVychozi Then Return
                Zvuk("CZ/Slova/razeni")
            End Sub

            <VzorDodatku("Koniec v stanici")> _
            Public Sub Koniec(ByVal aiStanice As Integer())
                Zvuk("CZ/Slova/koniec")
            End Sub
        End Class

        """.ReplaceLineEndings("\r\n");

    private const string Bank = """
        <?xml version="1.0" encoding="UTF-8"?>
        <FYZBANK>
          <lang default="true" k="CZ" n="Čeština" d="CZ">
            <g k="Slova" d="Slova\">
              <z k="razeni" t="řazení vlaku" f="razeni.EWA" l="700" />
              <z k="vyzva" t="pozor" f="vyzva.EWA" l="500" />
            </g>
          </lang>
        </FYZBANK>
        """;

    private static TestInstallation Installation()
    {
        var installation = new TestInstallation();
        installation.Trains("");
        installation.Write(@"Server\Data\2030\01 Lipovec\ReportKeys.xml", """<ReportKeys><r n="Přijede" k="P" v="V P K" /></ReportKeys>""");
        installation.Write(@"Server\Data\LogText.vb", Script);
        var bank = Path.Combine(installation.Root, "Server", "RawBank", "CZ", "FyzBank.xml");
        Directory.CreateDirectory(Path.GetDirectoryName(bank)!);
        File.WriteAllText(bank, Bank, new System.Text.UTF8Encoding(true));
        return installation;
    }

    private static ScriptReport Report(ReportsEdit edit, string name) => edit.Find(name)!.Report!;

    [TestMethod]
    public void Hlasenia_UpravitelneARucne()
    {
        var edit = new ReportsEdit(Script);

        CollectionAssert.AreEqual(new[] { "D1006", "Vyzva", "Přijede", "Koniec" }, edit.Reports.Select(result => result.Member.Name).ToArray());
        Assert.IsNull(edit.Find("Přijede")!.Report);
        Assert.AreEqual(ScriptManualReason.Statement, edit.Find("přijede")!.Reason);
        Assert.AreSame(Script, edit.Result());
    }

    [TestMethod]
    public void Koncept_TamASpat()
    {
        var edit = new ReportsEdit(Script);

        foreach (var result in edit.Reports.Where(result => result.Report is not null))
            Assert.AreEqual(result.Report, ReportDraft.Of(result.Report!).Build(), result.Member.Name);
    }

    [TestMethod]
    public void Koncept_SusedneKrokyJednehoJazykaSuJedenBlok()
    {
        var draft = new ReportDraft { Name = "Test", Description = "Test" };
        draft.Steps.Add(new StepDraft { Value = "CZ/Slova/razeni" });
        draft.Steps.Add(new StepDraft { Block = "GB", Value = "GB/Slova/x", After = "200" });
        draft.Steps.Add(new StepDraft { Block = "GB", Kind = StepDraft.PauseKind, Value = "300" });
        draft.Steps.Add(new StepDraft { Kind = StepDraft.OffKind, Value = "Zvuk(\"D/Slova/x\")" });

        var report = draft.Build()!;

        Assert.HasCount(3, report.Blocks);
        Assert.IsNull(report.Blocks[0].Language);
        Assert.AreEqual("GB", report.Blocks[1].Language);
        CollectionAssert.AreEqual(new ScriptStep[] { new SoundStep("GB/Slova/x", 0, 200), new PauseStep(300) }, report.Blocks[1].Steps.ToArray());
        Assert.AreEqual(new CommentStep("Zvuk(\"D/Slova/x\")"), report.Blocks[2].Steps.Single());

        draft.Steps.Add(new StepDraft { Kind = StepDraft.PauseKind, Value = "x" });
        Assert.IsNull(draft.Build());
    }

    [TestMethod]
    public void Koncept_DruhUrciParameterATypVyluky()
    {
        var draft = ReportDraft.Of(Report(new ReportsEdit(Script), "Vyzva"));

        draft.Kind = ScriptReportKind.AdditionPattern;
        Assert.AreEqual(ReportDraft.DefaultStationsParameter, draft.Build()!.StationsParameter);

        draft.Kind = ScriptReportKind.Exclusion;
        draft.ExclusionType = "TYPVYLUKY.TRATOVA";
        draft.ExclusionStations = "5400010, 5400020";
        CollectionAssert.AreEqual(new[] { "TYPVYLUKY.TRATOVA", "5400010", "5400020" }, draft.Build()!.Arguments.ToArray());
        Assert.IsNull(draft.Build()!.ForStations);

        draft.Kind = ScriptReportKind.Other;
        draft.ForStations = "5400010";
        var other = draft.Build()!;
        Assert.IsEmpty(other.Arguments);
        Assert.IsNull(other.StationsParameter);
        Assert.AreEqual("5400010", other.ForStations);
    }

    [TestMethod]
    public void Krok_VypnutieAZapnutie()
    {
        var step = new StepDraft { Value = "CZ/Slova/razeni", After = "200" };

        Assert.IsTrue(step.Toggle());
        Assert.AreEqual(StepDraft.OffKind, step.Kind);
        Assert.AreEqual("Zvuk(\"CZ/Slova/razeni\", 0, 200)", step.Value);
        Assert.IsTrue(step.Toggle());
        Assert.AreEqual(new SoundStep("CZ/Slova/razeni", 0, 200), step.Build());

        var note = new StepDraft { Kind = StepDraft.OffKind, Value = " poznamka" };
        Assert.IsFalse(note.Toggle());
        Assert.AreEqual(StepDraft.OffKind, note.Kind);
    }

    [TestMethod]
    public void Uprava_ZapiseLenFunkciuAPremenovanie()
    {
        var edit = new ReportsEdit(Script);
        var report = Report(edit, "D1006") with { Description = "Zmena radenia vlaku" };

        edit.Write(report, "D1006");

        Assert.AreEqual(Script.Replace("<Dodatek(\"Zmena radenia\")>", "<Dodatek(\"Zmena radenia vlaku\")>", StringComparison.Ordinal), edit.Result());
        Assert.AreEqual(report, Report(edit, "D1006"));

        edit.Write(report with { Name = "D1007" }, "D1006");
        StringAssert.Contains(edit.Text, "    Public Sub D1007()\r\n        Zvuk(\"CZ/Slova/razeni\")\r\n    End Sub ' D1007\r\n");
        Assert.IsNull(edit.Find("D1006"));
        CollectionAssert.AreEqual(new[] { "D1007", "Vyzva", "Přijede", "Koniec" }, edit.Reports.Select(result => result.Member.Name).ToArray());
        Assert.AreEqual("D1007_2", edit.UniqueName("D1007"));
        Assert.AreEqual("Nove", edit.UniqueName("Nove"));

        // povodny text = povodny objekt, aj ked sa k nemu dospeje upravami
        edit.Write(Report(new ReportsEdit(Script), "D1006"), "D1007");
        Assert.AreSame(edit.Original, edit.Result());
    }

    [TestMethod]
    public void Uprava_ZmazanieANoveHlasenie()
    {
        var edit = new ReportsEdit(Script);

        edit.Remove("Vyzva");
        edit.Write(new ScriptReport("Nove", ScriptReportKind.Addition) { Description = "Nové" }, null);

        CollectionAssert.AreEqual(new[] { "D1006", "Nove", "Přijede", "Koniec" }, edit.Reports.Select(result => result.Member.Name).ToArray());
        Assert.IsFalse(edit.Text.Contains("Vyzva", StringComparison.Ordinal));
        Assert.ThrowsExactly<ArgumentException>(() => edit.Write(new ScriptReport("koniec", ScriptReportKind.Other), null));
    }

    [TestMethod]
    public void Kontrola_NazovPopisZvukyAPauzy()
    {
        using var installation = Installation();
        var bank = Iniss2Workspace.Load(installation.DataDir).Bank;

        static bool Taken(string name) => name == "Vyzva";
        static bool IsFunction(string name) => name is "Vyzva" or "Pomocna";
        int Errors(ReportDraft draft) => ReportRules.Check(draft, Taken, IsFunction, bank).Count(problem => problem.IsError);
        int Warnings(ReportDraft draft) => ReportRules.Check(draft, Taken, IsFunction, bank).Count(problem => !problem.IsError);

        var draft = new ReportDraft { Name = "Test", Description = "Test" };
        draft.Steps.Add(new StepDraft { Value = "CZ/Slova/razeni" });
        Assert.AreEqual(0, Errors(draft) + Warnings(draft));

        foreach (var name in new[] { "", "1abc", "_", "a b", "End", "Vyzva" })
        {
            draft.Name = name;
            Assert.AreEqual(1, Errors(draft), name);
        }

        draft.Name = "Test";
        draft.Description = "中";
        Assert.AreEqual(1, Errors(draft));
        draft.Description = "";
        Assert.AreEqual(1, Warnings(draft));
        draft.Description = "Test";

        // zvuk, ktory banka nema; jazyk, ktory banka nema, server neoveruje; zly tvar kluca; zvuk ineho jazyka v bloku
        draft.Steps.Add(new StepDraft { Value = "CZ/Slova/neznamy" });
        draft.Steps.Add(new StepDraft { Value = "GB/Slova/x" });
        Assert.AreEqual(1, Warnings(draft));
        draft.Steps.Add(new StepDraft { Value = "Slova" });
        draft.Steps.Add(new StepDraft { Block = "GB", Value = "CZ/Slova/razeni" });
        draft.Steps.Add(new StepDraft { Kind = StepDraft.PauseKind, Value = "dlho" });
        draft.Steps.Add(new StepDraft { Value = "CZ/Slova/razeni", Before = "-1" });
        Assert.AreEqual(3, Errors(draft));
        Assert.AreEqual(2, Warnings(draft));

        var exclusion = new ReportDraft { Name = "V1", Description = "Výluka", Kind = ScriptReportKind.Exclusion };
        Assert.AreEqual(1, Errors(exclusion));
        exclusion.ExclusionType = "TYPVYLUKY.AUTO";
        exclusion.ExclusionStations = "5400010 x";
        Assert.AreEqual(1, Errors(exclusion));
    }

    [TestMethod]
    public void Odkazy_ZDatNahladZBanky()
    {
        using var installation = Installation();
        var workspace = Iniss2Workspace.Load(installation.DataDir);

        var references = ReportReferences.Of(workspace.History.Current);
        Assert.AreEqual(1, references["přijede"].ReportKeys);
        Assert.AreEqual(1, references["přijede"].Total);
        Assert.IsFalse(references.ContainsKey("Vyzva"));

        var report = new ScriptReport("Test", ScriptReportKind.Other)
        {
            Blocks = [new ScriptBlock(null, [new SoundStep("CZ/Slova/vyzva"), new SoundStep("Slova/razeni"), new SoundStep("GB/Slova/x"), new PauseStep(200)])]
        };
        var texts = ReportPreview.Texts(report, workspace.Bank);
        // jazyk GB banka nema - server hlasenie v nom nevyhlasi
        CollectionAssert.AreEqual(new (string, string?)[] { ("CZ", "pozor řazení vlaku"), ("GB", null) }, texts.ToArray());
        CollectionAssert.AreEqual(new (string, string?)[] { ("CZ", "pozor řazení vlaku [Slova/neznamy]") },
            ReportPreview.Texts(report with { Blocks = [new ScriptBlock(null, [new SoundStep("CZ/Slova/vyzva"), new SoundStep("Slova/razeni")]),
                new ScriptBlock("CZ", [new SoundStep("Slova/neznamy")])] }, workspace.Bank).ToArray());
        Assert.IsEmpty(ReportPreview.Texts(report with { OnlyLanguage = "D" }, workspace.Bank));
    }

    // --- okno ---

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

    private static void Invoke(Form form, string method) =>
        form.GetType().GetMethod(method, Any)!.Invoke(form, [null, EventArgs.Empty]);

    private static FReports Open(TestInstallation installation)
    {
        var styles = Styles<GVXEditorStyle>.CreateDefault();
        var session = new AppSession<GVXEditorConfig, GVXEditorStyle>(new GVXEditorConfig(), styles, styles.StyleList[0]);
        session.UsingStyle = styles.StyleList[0];
        var context = new EditorContext(session);
        context.Open(Iniss2Workspace.Load(installation.DataDir));
        var form = new FReports(context);
        form.CreateControl();
        typeof(Form).GetMethod("OnShown", Any)!.Invoke(form, [EventArgs.Empty]);
        return form;
    }

    [TestMethod]
    public void Okno_BezZmenyTenIstyTextAZmenaPopisu() => Sta(() =>
    {
        using var installation = Installation();
        using var form = Open(installation);
        var text = form.Result();
        StringAssert.Contains(text, "<Dodatek(\"Zmena radenia\")>");

        // Escape okno nezatvori (rozpisane upravy sa zahodia len tlacidlom Zrusit)
        Assert.IsNull(form.CancelButton);

        // prve hlasenie je vybrane, zmena popisu prepise len jeho oznacenie
        Assert.AreEqual("D1006", Field<TextBox>(form, "tbName").Text);
        Field<TextBox>(form, "tbDescription").Text = "Zmena radenia vlaku";
        Assert.AreEqual(text.Replace("<Dodatek(\"Zmena radenia\")>", "<Dodatek(\"Zmena radenia vlaku\")>", StringComparison.Ordinal), form.Result());

        // nazov s chybou sa do skriptu nezapise
        Field<TextBox>(form, "tbName").Text = "1x";
        StringAssert.Contains(form.Result(), "Public Sub D1006()");
        Field<TextBox>(form, "tbName").Text = "D1007";
        StringAssert.Contains(form.Result(), "Public Sub D1007()");
        StringAssert.Contains(form.Result(), "End Sub ' D1007");
    });

    [TestMethod]
    public void Okno_RucnyKodANoveHlasenie() => Sta(() =>
    {
        using var installation = Installation();
        using var form = Open(installation);

        // vyber v zozname (okno nie je zobrazene, udalost vyberu tabulky by neprisla)
        form.GetType().GetMethod("SelectReport", Any)!.Invoke(form, ["Přijede"]);
        StringAssert.Contains(Field<Label>(form, "lManual").Text, "Přijede");
        Assert.AreEqual(Script, form.Result());

        Invoke(form, "tsbNew_Click");
        StringAssert.Contains(form.Result(), "    Public Sub " + Resources.Reports_NewName + "()\r\n");
        Assert.AreEqual(Resources.Reports_NewName, Field<TextBox>(form, "tbName").Text);
    });

    // --- prepojenie s datami ---

    // instalacia s odkazmi na funkcie: typ hlasenia a stav (Prijede), dodatok (D1006), hlasenie obsluhy (vzor Koniec)
    private static TestInstallation Linked()
    {
        var installation = Installation();
        installation.Write(@"Server\Data\2030\01 Lipovec\statedgm.xml", CodeTests.StateDiagram);
        installation.Write(@"Server\Data\2030\01 Lipovec\Dodatky.xml", """<DODATKY><v cv="2002" k="D1006" /><v cv="3003" k="d1006" /></DODATKY>""");
        installation.Write(@"Server\Data\ConstReports.xml",
            """<CONSTREPORTS><r k="4" n="Koniec v Horanoch" ik="Koniec"><s>5400020</s></r><r k="5" n="Stary" ik="Neexistuje"><s>5400020</s></r></CONSTREPORTS>""");
        return installation;
    }

    [TestMethod]
    public void Odkazy_PremenovanieVDatach()
    {
        using var installation = Linked();
        var state = Iniss2Workspace.Load(installation.DataDir).History.Current;

        Assert.AreSame(state, ReportRenames.Apply(state, new Dictionary<string, string>()));
        Assert.AreSame(state, ReportRenames.Apply(state, new Dictionary<string, string> { ["Nepouzite"] = "X" }));

        var renamed = ReportRenames.Apply(state, new Dictionary<string, string> { ["přijede"] = "Prichod", ["D1006"] = "D2000", ["Koniec"] = "Koniec_jazdy" });
        var station = renamed.Groups[0].Stations[0];
        Assert.AreEqual("Prichod", station.Setup!.ReportKeys.Single().Name);
        Assert.IsTrue(station.Code!.Diagram!.Graphs.SelectMany(graph => graph.Nodes).Any(node => node.Report == "Prichod"));
        Assert.IsFalse(station.Code!.Diagram!.Graphs.SelectMany(graph => graph.Nodes).Any(node => node.Report == "Přijede"));
        CollectionAssert.AreEqual(new[] { "D2000", "D2000" }, station.Files!.Additions.Select(addition => addition.Key).ToArray());
        Assert.AreEqual("Koniec_jazdy", renamed.ConstReports![0].Function);
        Assert.AreSame(state.ConstReports![1], renamed.ConstReports[1]);
        // druha stanica odkazy nema - ostava ten isty objekt
        Assert.AreSame(state.Groups[0].Stations[1], renamed.Groups[0].Stations[1]);
    }

    [TestMethod]
    public void Odkazy_KontrolaInstalacie()
    {
        using var installation = Linked();
        installation.Write(@"Server\Data\LogText.vb", Script.Replace("<ZakladniHlaseni(\"Přijede\")> _\r\n    Public Sub Přijede()", "<ZakladniHlaseni(\"Odjede\")> _\r\n    Public Sub Odjede()",
            StringComparison.Ordinal));
        var workspace = Iniss2Workspace.Load(installation.DataDir);

        var messages = Domain.Analysis.InstallationChecks.Run(workspace).Select(finding => (finding.Severity, finding.Message)).ToList();

        CollectionAssert.Contains(messages, (Domain.Analysis.FindingSeverity.Warning,
            string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Check_ConstReportMissing, 5, "Stary", "Neexistuje")));
        CollectionAssert.Contains(messages, (Domain.Analysis.FindingSeverity.Warning,
            string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Check_ReportKeyMissing, "Přijede")));
        Assert.IsTrue(messages.Any(message => message.Message == string.Format(System.Globalization.CultureInfo.CurrentCulture,
            Resources.Check_StateReportMissing, "Nástup", "Přijede")));
        CollectionAssert.Contains(messages, (Domain.Analysis.FindingSeverity.Info,
            string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Check_UnusedBasicReport, "Odjede")));
    }

    [TestMethod]
    public void Odkazy_NovyDodatokAPremenovanieVOkne() => Sta(() =>
    {
        Assert.AreEqual("D1007", new ReportsEdit(Script).NextAdditionName());
        Assert.AreEqual("D1", new ReportsEdit("Public Class LogText\r\nEnd Class").NextAdditionName());

        using var installation = Linked();
        using var form = Open(installation);

        // vzor Koniec ma odkaz z hlasenia obsluhy, D1006 z dodatkov stanice
        form.GetType().GetMethod("SelectReport", Any)!.Invoke(form, ["Koniec"]);
        Field<TextBox>(form, "tbName").Text = "Koniec_jazdy";
        form.GetType().GetMethod("SelectReport", Any)!.Invoke(form, ["Vyzva"]);
        Field<TextBox>(form, "tbName").Text = "Vyzva2";
        CollectionAssert.AreEquivalent(new[] { new KeyValuePair<string, string>("Koniec", "Koniec_jazdy") }, form.Renames.ToArray());

        // okno otvorene na novy dodatok ho zalozi s dalsim cislom D
        var styles = Styles<GVXEditorStyle>.CreateDefault();
        var context = new EditorContext(new AppSession<GVXEditorConfig, GVXEditorStyle>(new GVXEditorConfig(), styles, styles.StyleList[0])
            { UsingStyle = styles.StyleList[0] });
        context.Open(Iniss2Workspace.Load(installation.DataDir));
        using var create = new FReports(context, null, ScriptReportKind.Addition);
        create.CreateControl();
        typeof(Form).GetMethod("OnShown", Any)!.Invoke(create, [EventArgs.Empty]);
        Assert.AreEqual("D1007", create.CreatedName);
        StringAssert.Contains(create.Result(), "    Public Sub D1007()\r\n    End Sub ' D1007\r\n");
        Field<TextBox>(create, "tbName").Text = "D1008";
        Assert.AreEqual("D1008", create.CreatedName);
    });

    // --- uroven 3: podmienky, straze a sekcia ---

    [TestMethod]
    public void Uroven3_KonceptPodmienokStrazASekcie()
    {
        var single = new ScriptCondition("(aiStanice IsNot Nothing) AndAlso (aiStanice.Length = 1)");
        var report = new ScriptReport("Zastavenie", ScriptReportKind.AdditionPattern)
        {
            Description = "Zastavenie", StationsParameter = "aiStanice", OnlyLanguage = "CZ", Guards = ["Not Kontrola()", "VlakS.JeKoncici"], Section = true,
            Blocks =
            [
                new ScriptBlock("CZ", [new SoundStep("CZ/Slova/razeni")]) { Conditions = [single] },
                new ScriptBlock("CZ", [new SoundStep("CZ/Slova/vyzva")]) { Conditions = [single with { Else = true }] },
                new ScriptBlock(null, [new PauseStep(100)]) { Conditions = [new ScriptCondition("DlouheHlaseni"), new ScriptCondition("KratkeHlaseni", true)] }
            ]
        };

        var draft = ReportDraft.Of(report);
        Assert.AreEqual("Not Kontrola()" + Environment.NewLine + "VlakS.JeKoncici", draft.Guards);
        Assert.AreEqual(single.Expression, draft.Steps[0].Condition);
        Assert.AreEqual("Else " + single.Expression, draft.Steps[1].Condition);
        Assert.AreEqual("DlouheHlaseni » Else KratkeHlaseni", draft.Steps[2].Condition);
        Assert.AreEqual(report, draft.Build());

        // prazdna cast podmienky je chyba, koncept sa neda zostavit
        draft.Steps[2].Condition = "DlouheHlaseni »  » KratkeHlaseni";
        Assert.IsNull(draft.Build());
        Assert.AreEqual(1, ReportRules.Check(draft, _ => false, _ => true, null).Count(problem => problem.IsError));
        CollectionAssert.AreEqual(new[] { "a", "b" }, ReportDraft.GuardList(" a \r\n\r\n b ").ToArray());
    }

    [TestMethod]
    public void Uroven3_OknoPodmienkaKrokuAStraz() => Sta(() =>
    {
        using var installation = Installation();
        using var form = Open(installation);
        var text = form.Result();

        // podmienka prveho kroku dodatku z ponuky, potom jej vetva Else a bez podmienky
        var steps = Field<DataGridView>(form, "dgvSteps");
        Assert.AreEqual(1, steps.Rows.Count);
        steps.CurrentCell = steps.Rows[0].Cells[3];
        var menu = Field<ToolStripDropDownButton>(form, "tsbCondition");
        menu.DropDownItems.Cast<ToolStripItem>().Single(item => item.Text!.Contains("(KratkeHlaseni)", StringComparison.Ordinal)).PerformClick();
        StringAssert.Contains(form.Result(), "        If KratkeHlaseni Then\r\n            Zvuk(\"CZ/Slova/razeni\")\r\n        End If\r\n");
        menu.DropDownItems[1].PerformClick();
        StringAssert.Contains(form.Result(), "        If KratkeHlaseni Then\r\n        Else\r\n            Zvuk(\"CZ/Slova/razeni\")\r\n        End If\r\n");
        menu.DropDownItems[0].PerformClick();
        Assert.AreEqual(text, form.Result());

        // straz a sekcia
        Field<TextBox>(form, "tbGuards").Text = "VlakS.JeKoncici";
        Field<CheckBox>(form, "cboxSection").Checked = true;
        StringAssert.Contains(form.Result(), "    Public Sub D1006()\r\n        If VlakS.JeKoncici Then Return\r\n        ZacatekSekce()\r\n        Zvuk(\"CZ/Slova/razeni\")\r\n        KonecSekce(bRollBack:=True)\r\n");
    });

    // --- uroven 2: vstavane polozky a volania ---

    [TestMethod]
    public void Uroven2_KonceptVstavanejPolozkyAVolania()
    {
        var builtin = new BuiltinStep("DruhVlaku", ["\"V4\"", "0", "bMezinarodni:=True"]) { Comment = " druh" };
        var call = new CallStep("Pomocna", ["aiStanice"]);

        var draft = StepDraft.Of("CZ", builtin);
        Assert.AreEqual("DruhVlaku", draft.Kind);
        Assert.AreEqual("\"V4\", 0, bMezinarodni:=True", draft.Value);
        Assert.AreEqual(builtin, draft.Build());
        Assert.AreEqual(StepDraft.CallKind, StepDraft.Of(null, call).Kind);
        Assert.AreEqual("Pomocna(aiStanice)", StepDraft.Of(null, call).Value);
        Assert.AreEqual(call, StepDraft.Of(null, call).Build());

        // argumenty, ktore model neopise, a volanie vstavanej metody ako funkcie nie su krok
        Assert.IsNull(new StepDraft { Kind = "DruhVlaku", Value = "\"V4\", If(JeIDS, 4, 9)" }.Build());
        Assert.IsNull(new StepDraft { Kind = "DruhVlaku", Value = "\"V4\"" }.Build());
        Assert.IsNull(new StepDraft { Kind = StepDraft.CallKind, Value = "Dodatky()" }.Build());

        // vypnutie a zapnutie zachova vstavanu polozku
        Assert.IsTrue(draft.Toggle());
        Assert.AreEqual("DruhVlaku(\"V4\", 0, bMezinarodni:=True) ' druh", draft.Value);
        Assert.IsTrue(draft.Toggle());
        Assert.AreEqual(builtin, draft.Build());

        Assert.AreEqual("\"\", 0", StepDraft.Template(ScriptBuiltins.Find("DruhVlaku")!, null));
        Assert.AreEqual("aiStanice, \"\", \"\", \"\"", StepDraft.Template(ScriptBuiltins.Find("SeznamStanic")!, "aiStanice"));
        Assert.AreEqual("", StepDraft.Template(ScriptBuiltins.Find("Dodatky")!, null));
    }

    [TestMethod]
    public void Uroven2_KontrolaANahlad()
    {
        using var installation = Installation();
        var workspace = Iniss2Workspace.Load(installation.DataDir);
        static bool IsFunction(string name) => name == "Pomocna";
        List<SetupProblem> Check(StepDraft step)
        {
            var draft = new ReportDraft { Name = "Test", Description = "Test" };
            draft.Steps.Add(step);
            return ReportRules.Check(draft, _ => false, IsFunction, workspace.Bank);
        }

        Assert.IsEmpty(Check(new StepDraft { Kind = "Hodiny", Value = "\"Slova\", VlakS.CasPrij" }));
        Assert.IsTrue(Check(new StepDraft { Kind = "Hodiny", Value = "\"C1\", VlakS.CasPrij" }).Single() is { IsError: false });
        Assert.IsTrue(Check(new StepDraft { Kind = "Hodiny", Value = "\"Slova\"" }).Single() is { IsError: true });
        Assert.IsEmpty(Check(new StepDraft { Kind = StepDraft.CallKind, Value = "Pomocna()" }));
        Assert.IsTrue(Check(new StepDraft { Kind = StepDraft.CallKind, Value = "Neznama()" }).Single() is { IsError: true });
        Assert.IsTrue(Check(new StepDraft { Kind = "SeznamStanic", Value = "aiStanice, Nothing, Nothing, \"Slova\", PrefixPosl:=AddressOf Neznama" })
            .Single() is { IsError: true });

        var report = new ScriptReport("Test", ScriptReportKind.Other)
        {
            Blocks = [new ScriptBlock(null, [new SoundStep("CZ/Slova/vyzva"), new BuiltinStep("CisloVlaku", ["VlakS.CVlaku"]), new CallStep("Pomocna", [])])]
        };
        Assert.AreEqual("pozor ‹" + Resources.Reports_BuiltinCisloVlaku + "› ‹Pomocna›", ReportPreview.Texts(report, workspace.Bank).Single().Text);
    }

    [TestMethod]
    public void Uroven2_OknoVstavanaPolozkaANovyVzorVHlaseniachObsluhy() => Sta(() =>
    {
        using var installation = Installation();
        using var form = Open(installation);

        // vzor dodatku dostane zoznam stanic vzoru
        form.GetType().GetMethod("SelectReport", Any)!.Invoke(form, ["Koniec"]);
        var menu = Field<ToolStripDropDownButton>(form, "tsbAddBuiltin");
        menu.DropDownItems.Cast<ToolStripItem>().Single(item => item.Text!.Contains("(SeznamStanic)", StringComparison.Ordinal)).PerformClick();
        StringAssert.Contains(form.Result(), "        SeznamStanic(aiStanice, \"\", \"\", \"\")\r\n");

        // novy vzor odklonu: okno Hlasenia obsluhy ho ponukne ako vzor
        Field<ComboBox>(form, "cboKind").SelectedValue = ScriptReportKinds.AttributeName(ScriptReportKind.DiversionPattern);
        StringAssert.Contains(form.Result(), "<VzorOdklonu(\"Koniec v stanici\")> _\r\n    Public Sub Koniec(ByVal aiStanice As Integer())");

        var workspace = Iniss2Workspace.Load(installation.DataDir);
        workspace.ChangeLogText(form.Result(), "test");
        var reports = workspace.ConstReports;
        var keys = new Domain.Stations.KeyCatalog(workspace.Functions, workspace.FunctionDescriptions, reports);
        using var constReports = new UI.Setup.FConstReports(reports, keys, ConstReportsEdit.UsedKeys(workspace.Groups), workspace.StationNames);
        var choices = (IEnumerable<Domain.Views.ValueChoice>)Field<DataGridViewComboBoxColumn>(constReports, "cRepFunction").DataSource!;
        Assert.IsTrue(choices.Any(choice => choice.Key == "Koniec"));
    });
}
