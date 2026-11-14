using System.Text;

namespace GVXEditor.Tests;

/// <summary>
/// Fiktivna instalacia INISS2 v docasnom priecinku: grafikon 2030 so stanicami Lipovec a Horany.
/// </summary>
internal sealed class TestInstallation : IDisposable
{
    public const int Lipovec = 5400010;
    public const int Horany = 5400020;
    public const int Javor = 5400005;

    private static readonly Encoding Win1250 = CodePages();

    public TestInstallation()
    {
        Root = Path.Combine(Path.GetTempPath(), "GVXEditorTests_" + Guid.NewGuid().ToString("N"));
        Write(@"Server\Data\Config.xml", "<INISSCFG />");
        Write(@"Server\Data\DirList.xml", """
            <DL>
              <g n="2030">
                <i s="5400010" d="01 Lipovec" />
                <i s="5400020" d="02 Horany" />
              </g>
            </DL>
            """);
        Write(@"Server\Data\2030\01 Lipovec\SR70.TXT", "5400010\r\n");
        Write(@"Server\Data\2030\02 Horany\SR70.TXT", "5400020\r\n");
        Write(@"Server\Data\2030\GV.stanice.XML", """
            <!-- stanice siete -->
            <ST>
              <s k="5400005" n="Javor" />
              <s k="5400010" n="Lipovec" />
              <s k="5400020" n="Horany" />
            </ST>
            """);
        Write(@"Server\Data\2030\01 Lipovec\PositionsDef.xml", """<POZICE><pos k="1" n="1" /><pos k="2" n="2" /></POZICE>""");
        Write(@"Server\Data\2030\01 Lipovec\Positions.xml", """
            <POZICESPOJU>
              <pos jd="jede v 6" cv="2002" p="2" o="2" />
              <pos cv="2002" p="1" o="1" />
            </POZICESPOJU>
            """);
    }

    /// <summary>Koren instalacie.</summary>
    public string Root { get; }

    /// <summary>Priecinok dat.</summary>
    public string DataDir => Path.Combine(Root, "Server", "Data");

    /// <summary>
    /// Zapise GV.XML grafikonu 2030 s danymi vlakmi.
    /// </summary>
    public void Trains(string trains) => Write(@"Server\Data\2030\GV.XML", $"""
        <?xml version="1.0" encoding="windows-1250"?>
        <GV f="1" l="C">
          <s>5400010</s>
          <s>5400020</s>
          {trains}
        </GV>
        """);

    public void Write(string relativeFile, string text)
    {
        var file = Path.Combine(Root, relativeFile);
        Directory.CreateDirectory(Path.GetDirectoryName(file)!);
        File.WriteAllText(file, text, Win1250);
    }

    public void Dispose()
    {
        if (Directory.Exists(Root))
            Directory.Delete(Root, true);
    }

    private static Encoding CodePages()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        return Encoding.GetEncoding(1250);
    }
}
