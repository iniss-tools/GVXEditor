using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using GVXEditor.Config;
using GVXEditor.Mcp;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using ToolsCore;
using ToolsCore.XML;

namespace GVXEditor.Tests.Mcp;

/// <summary>Server MCP v GVXEditore: len s klucom, len z tohto pocitaca, bez otvorenej instalacie chyba s kodom.</summary>
[TestClass]
public sealed class GvxMcpServerTests
{
    public TestContext TestContext { get; set; } = null!;

    internal static int FreePort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        return ((IPEndPoint)listener.LocalEndpoint).Port;
    }

    internal static GvxMcpSettings Enabled(bool allowWrites = false) =>
        new() { Enabled = true, AllowWrites = allowWrites, Port = FreePort(), Key = new GvxMcpSettings().CreateKey() };

    internal static EditorContext Context()
    {
        var styles = Styles<GVXEditorStyle>.CreateDefault();
        var session = new AppSession<GVXEditorConfig, GVXEditorStyle>(new GVXEditorConfig(), styles, styles.StyleList[0]);
        session.UsingStyle = styles.StyleList[0];
        return new EditorContext(session);
    }

    internal static async Task<McpClient> Connect(GvxMcpServer server, GvxMcpSettings settings, CancellationToken cancellation)
    {
        var transport = new HttpClientTransport(new HttpClientTransportOptions
        {
            Endpoint = new Uri(server.Address!),
            TransportMode = HttpTransportMode.StreamableHttp,
            AdditionalHeaders = new Dictionary<string, string> { ["Authorization"] = "Bearer " + settings.Key }
        });
        return await McpClient.CreateAsync(transport, cancellationToken: cancellation);
    }

    private async Task<HttpStatusCode> Initialize(string address, string? key, string? origin = null)
    {
        using var http = new HttpClient();
        using var request = new HttpRequestMessage(HttpMethod.Post, address)
        {
            Content = new StringContent(
                """{"jsonrpc":"2.0","id":1,"method":"initialize","params":{"protocolVersion":"2025-11-25","capabilities":{},"clientInfo":{"name":"test","version":"1"}}}""",
                Encoding.UTF8, "application/json")
        };
        request.Headers.Accept.ParseAdd("application/json");
        request.Headers.Accept.ParseAdd("text/event-stream");
        if (key is not null)
            request.Headers.Authorization = new("Bearer", key);
        if (origin is not null)
            request.Headers.Add("Origin", origin);
        using var response = await http.SendAsync(request, TestContext.CancellationToken);
        return response.StatusCode;
    }

    [TestMethod]
    public async Task Server_LenSKlucomAZTohtoPocitaca()
    {
        using var server = new GvxMcpServer();
        var settings = Enabled();
        await server.ApplyAsync(new EditorAccess(Context(), null), settings);
        var address = server.Address!;

        Assert.AreEqual(settings.Address, address);
        Assert.AreEqual(HttpStatusCode.Unauthorized, await Initialize(address, null));
        Assert.AreEqual(HttpStatusCode.Unauthorized, await Initialize(address, "gvx_zly"));
        Assert.AreEqual(HttpStatusCode.Forbidden, await Initialize(address, settings.Key, "http://evil.example"));
        Assert.AreEqual(HttpStatusCode.OK, await Initialize(address, settings.Key));
    }

    [TestMethod]
    public async Task Server_BezInstalacieChybaSKodom()
    {
        using var server = new GvxMcpServer();
        var settings = Enabled();
        await server.ApplyAsync(new EditorAccess(Context(), null), settings);
        await using var client = await Connect(server, settings, TestContext.CancellationToken);

        var result = await client.CallToolAsync("get_installation", cancellationToken: TestContext.CancellationToken);

        Assert.IsTrue(result.IsError);
        StringAssert.Contains(string.Join(" ", result.Content.OfType<TextContentBlock>().Select(c => c.Text)), "installation.notOpen");
    }

    [TestMethod]
    public async Task Server_NastrojeSoZmenamiLenSPovolenim()
    {
        async Task<List<string>> Tools(bool allowWrites)
        {
            using var server = new GvxMcpServer();
            var settings = Enabled(allowWrites);
            await server.ApplyAsync(new EditorAccess(Context(), null), settings);
            await using var client = await Connect(server, settings, TestContext.CancellationToken);
            StringAssert.Contains(client.ServerInstructions, allowWrites ? "undo step" : "Changes are made by the user");
            return (await client.ListToolsAsync(cancellationToken: TestContext.CancellationToken)).Select(t => t.Name).ToList();
        }

        var readOnly = await Tools(false);
        var writes = await Tools(true);

        CollectionAssert.Contains(readOnly, "get_train");
        CollectionAssert.DoesNotContain(readOnly, "set_train");
        CollectionAssert.Contains(writes, "set_train");
        Assert.AreEqual(GvxMcpServer.ToolMethods().Count, writes.Count);
        Assert.AreEqual(GvxMcpServer.ToolMethods().Count(GvxMcpServer.IsReadOnly), readOnly.Count);
    }

    [TestMethod]
    public async Task Server_ObsadenyPortJeChybaStavu()
    {
        var port = FreePort();
        using var listener = new TcpListener(IPAddress.Loopback, port);
        listener.Start();
        using var server = new GvxMcpServer();
        var settings = Enabled();
        settings.Port = port;

        await server.ApplyAsync(new EditorAccess(Context(), null), settings);

        // GVXEditor bezi dalej - chyba je v stave servera
        Assert.IsNull(server.Address);
        Assert.IsNotNull(server.Error);
    }

    [TestMethod]
    public void Nastavenie_KlucSifrovanyVConfigu()
    {
        var settings = new GvxMcpSettings { Enabled = true, AllowWrites = true };
        var key = settings.EnsureKey();
        var serializer = new XmlSerializer(typeof(GvxMcpSettings));
        using var writer = new StringWriter();
        serializer.Serialize(writer, settings);
        using var reader = XmlReader.Create(new StringReader(writer.ToString()), new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null });
        var read = (GvxMcpSettings)serializer.Deserialize(reader)!;

        StringAssert.StartsWith(key, GvxMcpSettings.Prefix);
        Assert.DoesNotContain(key, writer.ToString(), "kluc nie je v configu citatelny");
        Assert.AreEqual(key, read.Key);
        Assert.IsTrue(read.SameAs(settings));
        Assert.AreEqual($"http://127.0.0.1:{GvxMcpSettings.DefaultServerPort}/mcp", read.Address);
        CollectionAssert.AreEquivalent(new[] { "Enabled", "AllowWrites", "Port", "Key" },
            XElement.Parse(writer.ToString()).Attributes().Where(a => !a.IsNamespaceDeclaration).Select(a => a.Name.LocalName).ToArray());
        // config programu nastavenie servera prenasa aj v kopii z okna nastaveni
        var config = new GVXEditorConfig { McpServer = settings };
        Assert.IsTrue((config with { }).McpServer.SameAs(settings));
    }
}
