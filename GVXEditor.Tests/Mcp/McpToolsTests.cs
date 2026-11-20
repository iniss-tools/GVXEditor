using System.Text.Json;
using GVXEditor.Domain;
using GVXEditor.Mcp;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Tests.Mcp;

/// <summary>
/// Kazdy nastroj cez server MCP nad testovacou instalaciou: vysledok zodpoveda scheme vystupu, ktoru dostane klient (aj
/// formaty datumu - Claude Code ich kontroluje), a obsahuje udaje instalacie vratane neulozenych zmien. Novy nastroj treba
/// pridat do <see cref="KazdyNastroj_VysledokPodlaSchemy" />.
/// </summary>
[TestClass]
public sealed partial class McpToolsTests
{
    public TestContext TestContext { get; set; } = null!;

    // Javor -> Lipovec (Z, cislo 2002, hlasi sa 2002A) -> Dub (prejazd) -> Horany (Z), v soboty
    private const string Train = """
        <v o="2030-12-15" d="2031-12-13">
          <v jd="jede v 6" d="Fiktivní dráhy" f="D" p="sobota">
            <z c="5400005" o="1420" dcv="1001" dv="Os" nv="Javor" />
            <z c="5400010" p="1435" o="1437" f="Z PR X" dcv="2002" cv="2002A" dv="Sp" l="X5" ids="S1" />
            <z c="5400030" f="P DB" />
            <z c="5400020" p="15" cp="1" f="Z" />
          </v>
        </v>
        """;

    private const string LogText = """
        Public Class LogText
            <Dodatek("Upozornenie na zmenu radenia")>
            Public Function D1006() As Boolean
                Return True
            End Function
        End Class
        """;

    /// <summary>Pripojeny klient a schemy vystupu nastrojov; zaznamenava volane nastroje.</summary>
    private sealed class Session(McpClient client, Dictionary<string, JsonElement?> schemas, TestContext test) : IAsyncDisposable
    {
        public HashSet<string> Called { get; } = [];

        public IReadOnlyCollection<string> Tools => schemas.Keys;

        public async Task<JsonElement> Call(string tool, Dictionary<string, object?>? arguments = null)
        {
            var result = await client.CallToolAsync(tool, arguments, cancellationToken: test.CancellationToken);
            Assert.IsFalse(result.IsError == true, $"{tool}: {Text(result)}");
            Called.Add(tool);
            var content = result.StructuredContent!.Value;
            var errors = OutputSchema.Validate(content, schemas[tool]!.Value);
            Assert.IsEmpty(errors, $"{tool}: {string.Join("; ", errors)}");
            return content;
        }

        /// <summary>Chyba nastroja - text, ktory dostane model.</summary>
        public async Task<string> Error(string tool, Dictionary<string, object?>? arguments = null)
        {
            var result = await client.CallToolAsync(tool, arguments, cancellationToken: test.CancellationToken);
            Assert.IsTrue(result.IsError, $"{tool} mal skoncit chybou: {Text(result)}");
            Called.Add(tool);
            return Text(result);
        }

        private static string Text(CallToolResult result) => string.Join(" ", result.Content.OfType<TextContentBlock>().Select(t => t.Text));

        public ValueTask DisposeAsync() => client.DisposeAsync();
    }

    private static Dictionary<string, object?> Stop(int station, string arrival, string departure, string flags = "", string number = "") =>
        new() { ["station"] = station, ["arrival"] = arrival, ["departure"] = departure, ["flags"] = flags, ["number"] = number };

    // zmeny: kazda je krok spat (agent), skuska nanecisto nic nemeni, chyba pravidiel zmenu odmietne
    private static async Task Changes(Session mcp, Iniss2Workspace workspace)
    {
        var trains = () => workspace.Groups[0].Timetable!.Trains;
        var steps = workspace.History.UndoLabels.Count;

        var dry = await mcp.Call("set_train", new() { ["train"] = 0, ["to"] = "2031-06-30", ["dryRun"] = true });
        Assert.IsTrue(dry.GetProperty("change").GetProperty("changed").GetBoolean());
        Assert.AreEqual(JsonValueKind.Null, dry.GetProperty("change").GetProperty("step").ValueKind);
        Assert.AreEqual(new DateOnly(2031, 12, 13), trains()[0].To, "nanecisto bez zmeny");
        var same = await mcp.Call("set_train", new() { ["train"] = 0, ["from"] = "2030-12-15" });
        Assert.IsFalse(same.GetProperty("change").GetProperty("changed").GetBoolean(), "rovnaky vlak nie je zmena");

        var added = await mcp.Call("set_train", new()
        {
            ["variants"] = new[]
            {
                new Dictionary<string, object?>
                {
                    ["validity"] = "jede v 1-5", ["flags"] = "D",
                    ["route"] = new[] { Stop(TestInstallation.Lipovec, "", "10:00", number: "3001"), Stop(TestInstallation.Horany, "10:20 +0", "") }
                }
            }
        });
        Assert.AreEqual(1, added.GetProperty("train").GetInt32());
        StringAssert.EndsWith(added.GetProperty("change").GetProperty("step").GetString(), "(agent)");
        Assert.HasCount(2, trains());
        Assert.AreEqual(GvVariantFlags.D, trains()[1].Variants[0].Flags);
        Assert.IsTrue(trains()[1].Variants[0].Stops[0].IsInterest, "Z dopocitane zo stanic grafikonu");
        StringAssert.Contains(await mcp.Error("set_train", new()
        {
            ["train"] = 1, ["variants"] = new[] { new Dictionary<string, object?> { ["route"] = new[] { Stop(TestInstallation.Lipovec, "", "10:00", number: "3001") } } }
        }), "train.invalid");
        StringAssert.Contains(await mcp.Error("set_train", new()
        {
            ["train"] = 1, ["variants"] = new[] { new Dictionary<string, object?> { ["flags"] = "Q", ["route"] = Array.Empty<object>() } }
        }), "train.flag");

        var copy = await mcp.Call("copy_train", new() { ["trains"] = new[] { 1 } });
        Assert.AreEqual(2, copy.GetProperty("trains")[0].GetInt32());
        var renamed = await mcp.Call("rename_train_number", new() { ["train"] = 0, ["from"] = "2002", ["to"] = "2004" });
        Assert.AreEqual(2, renamed.GetProperty("records").GetInt32(), "dva zaznamy kolaji cisla 2002");
        Assert.IsTrue(workspace.Groups[0].Stations[0].Files!.Positions.All(position => position.Train == "2004"));
        StringAssert.Contains(await mcp.Error("rename_train_number", new() { ["train"] = 1, ["from"] = "3001", ["to"] = "1001" }), "rename.exists");
        await mcp.Call("shift_validity", new() { ["days"] = 364, ["trains"] = new[] { 2 } });
        Assert.AreEqual(new DateOnly(2031, 12, 14), trains()[2].From);
        var deleted = await mcp.Call("delete_train", new() { ["trains"] = new[] { 2 }, ["removeUnusedRecords"] = true });
        Assert.AreEqual(0, deleted.GetProperty("removedRecords").GetInt32());
        Assert.HasCount(2, trains());

        // hlasenia obsluhy: nove za najvyssim cislom, neznamy vzor je upozornenie, zmazanie
        var report = await mcp.Call("set_const_report", new() { ["name"] = "Výluka", ["function"] = "D1006", ["stations"] = new[] { TestInstallation.Lipovec } });
        Assert.AreEqual(4, report.GetProperty("number").GetInt32());
        var unknown = await mcp.Call("set_const_report", new() { ["number"] = 4, ["name"] = "Výluka", ["function"] = "Neznama" });
        Assert.AreEqual(1, unknown.GetProperty("warnings").GetArrayLength());
        StringAssert.Contains(await mcp.Error("set_const_report", new() { ["number"] = 4, ["name"] = "Výluka", ["function"] = " " }), "report.invalid");
        await mcp.Call("delete_const_report", new() { ["number"] = 4 });
        Assert.HasCount(1, workspace.ConstReports);
        StringAssert.Contains(await mcp.Error("delete_const_report", new() { ["number"] = 9 }), "report.notFound");

        StringAssert.Contains(await mcp.Error("generate_board_texts", new() { ["station"] = "Lipovec" }), "texts.noSet");
        Assert.AreEqual(steps + 8, workspace.History.UndoLabels.Count, "kazda zmena jeden krok spat");
    }

    [TestMethod]
    public async Task Zmena_ZaneprazdneneOknoOdmietne()
    {
        using var installation = new TestInstallation();
        installation.Trains(Train);
        var context = GvxMcpServerTests.Context();
        context.Open(Iniss2Workspace.Load(installation.DataDir));
        var host = new BusyHost();
        var editor = new EditorAccess(context, null, host);

        var error = await Assert.ThrowsAsync<ModelContextProtocol.McpException>(() =>
            GVXEditor.Mcp.TrainEditTools.ShiftValidity(editor, 7));

        StringAssert.Contains(error.Message, "editor.busy");
        Assert.AreEqual(new DateOnly(2030, 12, 15), context.Workspace!.Groups[0].Timetable!.Trains[0].From);
    }

    private sealed class BusyHost : IAgentHost
    {
        public void EnsureIdle() => throw new ModelContextProtocol.McpException("editor.busy: test");

        public void Change(WorkspaceState state, string step) => throw new InvalidOperationException("nema sa volat");
    }

    [TestMethod]
    public async Task KazdyNastroj_VysledokPodlaSchemy()
    {
        using var installation = new TestInstallation();
        installation.Trains(Train);
        installation.Write(@"Server\Data\LogText.vb", LogText);
        var context = GvxMcpServerTests.Context();
        context.Open(Iniss2Workspace.Load(installation.DataDir));
        var workspace = context.Workspace!;

        using var server = new GvxMcpServer();
        var settings = GvxMcpServerTests.Enabled(allowWrites: true);
        await server.ApplyAsync(new EditorAccess(context, null), settings);
        Assert.IsNull(server.Error, server.Error);
        var client = await GvxMcpServerTests.Connect(server, settings, TestContext.CancellationToken);
        var tools = await client.ListToolsAsync(cancellationToken: TestContext.CancellationToken);
        await using var mcp = new Session(client, tools.ToDictionary(t => t.Name, t => t.ProtocolTool.OutputSchema), TestContext);

        var info = await mcp.Call("get_installation");
        Assert.AreEqual("stopped", info.GetProperty("server").GetString());
        var group = info.GetProperty("groups")[0];
        Assert.AreEqual("2030", group.GetProperty("name").GetString());
        Assert.AreEqual(2, group.GetProperty("stations").GetArrayLength());

        // vlaky: filtre a cely vlak so zaznamami stanice podla cisla pri odchode
        Assert.AreEqual(1, (await mcp.Call("list_trains")).GetProperty("total").GetInt32());
        Assert.AreEqual(1, (await mcp.Call("list_trains", new() { ["type"] = "sp", ["station"] = "Horany", ["day"] = "2030-12-21" })).GetProperty("total").GetInt32());
        Assert.AreEqual(0, (await mcp.Call("list_trains", new() { ["day"] = "2030-12-22" })).GetProperty("total").GetInt32(), "v nedelu nejde");
        var train = await mcp.Call("get_train", new() { ["train"] = 0 });
        Assert.AreEqual("jede v 6", train.GetProperty("variants")[0].GetProperty("validity").GetString());
        var lipovec = train.GetProperty("stations").EnumerateArray().Single(s => s.GetProperty("station").GetInt32() == TestInstallation.Lipovec);
        Assert.AreEqual("2002", lipovec.GetProperty("number").GetString(), "cislo vlaku, nie hlasene cislo 2002A");
        Assert.AreEqual(2, lipovec.GetProperty("tracks").GetArrayLength());

        var days = await mcp.Call("explain_validity", new() { ["text"] = "jede v 6", ["from"] = "2030-12-15", ["to"] = "2030-12-31" });
        Assert.AreEqual(2, days.GetProperty("count").GetInt32());
        Assert.AreEqual("2030-12-21", days.GetProperty("days")[0].GetProperty("from").GetString());
        Assert.IsGreaterThan(40, (await mcp.Call("explain_validity", new() { ["text"] = "jede v 6", ["train"] = 0 })).GetProperty("count").GetInt32());

        var day = await mcp.Call("station_day", new() { ["station"] = "Lipovec", ["day"] = "2030-12-21" });
        Assert.AreEqual(1, day.GetProperty("trains").GetArrayLength());

        // subory: s neulozenou zmenou a mimo priecinka dat
        var gv = await mcp.Call("get_file", new() { ["path"] = "2030/GV.XML" });
        StringAssert.Contains(gv.GetProperty("text").GetString(), "2002A");
        Assert.IsFalse(gv.GetProperty("unsaved").GetBoolean());
        workspace.ChangeConstReports([new ConstReport(3, "Hlásenie agenta", "D1006", [TestInstallation.Lipovec])], "test");
        var reports = await mcp.Call("get_file", new() { ["path"] = "ConstReports.xml" });
        Assert.IsTrue(reports.GetProperty("unsaved").GetBoolean());
        StringAssert.Contains(reports.GetProperty("text").GetString(), "Hlásenie agenta");
        CollectionAssert.Contains((await mcp.Call("get_installation")).GetProperty("unsavedFiles").EnumerateArray().Select(f => f.GetString()).ToList(),
            "ConstReports.xml");
        StringAssert.Contains(await mcp.Error("get_file", new() { ["path"] = "../Config.xml" }), "file.outside");
        Assert.AreEqual(2, (await mcp.Call("get_file", new() { ["path"] = "2030/GV.XML", ["offset"] = 1, ["limit"] = 2 }))
            .GetProperty("text").GetString()!.Split('\n').Length);

        var functions = await mcp.Call("list_script_functions");
        var function = functions.GetProperty("functions")[0];
        Assert.AreEqual("D1006", function.GetProperty("name").GetString());
        Assert.AreEqual("Upozornenie na zmenu radenia", function.GetProperty("description").GetString());
        Assert.AreEqual((2, 5), (function.GetProperty("startLine").GetInt32(), function.GetProperty("endLine").GetInt32()));

        await mcp.Call("check_installation");
        StringAssert.Contains(await mcp.Error("verify_with_server"), "verify.unavailable");
        await mcp.Call("preview_board_texts", new() { ["station"] = "5400010" });
        StringAssert.Contains(await mcp.Error("station_day", new() { ["station"] = "Vrch", ["day"] = "2030-12-21" }), "station.notFound");
        StringAssert.Contains(await mcp.Error("get_train", new() { ["train"] = 5 }), "train.notFound");
        StringAssert.Contains(await mcp.Error("list_trains", new() { ["group"] = "2031" }), "group.notFound");

        await Changes(mcp, workspace);

        CollectionAssert.AreEquivalent(mcp.Tools.ToList(), mcp.Called.ToList(), "kazdy nastroj volany v teste");
    }
}
