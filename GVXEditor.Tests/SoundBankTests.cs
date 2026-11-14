using GVXEditor.Domain;

namespace GVXEditor.Tests;

/// <summary>
/// Zvukova banka instalacie pre vyber zvukov: jazyky, zvuk podla cesty radenia a kluca kolaje, banka, ktora chyba.
/// </summary>
[TestClass]
public class SoundBankTests
{
    private const string Bank = """
        <?xml version="1.0" encoding="UTF-8"?>
        <FYZBANK>
          <lang default="true" k="CZ" n="Čeština" d="CZ">
            <g k="K1" d="K1\">
              <z k="2" t="na druhou kolej" f="2.EWA" l="900" />
            </g>
            <g k="Slova" d="Slova\">
              <z k="razeni" t="řazení vlaku" f="razeni.EWA" l="700" />
            </g>
          </lang>
        </FYZBANK>
        """;

    [TestMethod]
    public void Banka_ZvukPodlaCestyAKlucaKolaje()
    {
        using var installation = new TestInstallation();
        // banka INISS2 je v UTF-8 (server sa riadi hlavickou XML)
        var file = Path.Combine(installation.Root, "Server", "RawBank", "CZ", "FyzBank.xml");
        Directory.CreateDirectory(Path.GetDirectoryName(file)!);
        File.WriteAllText(file, Bank, new System.Text.UTF8Encoding(true));
        var bank = Iniss2Workspace.Load(installation.DataDir).Bank;

        Assert.AreEqual("CZ", bank.Languages.Single().Key);
        Assert.AreEqual("řazení vlaku", bank.Find("CZ/Slova/razeni")?.Text);
        Assert.AreEqual("řazení vlaku", bank.Find("Slova/razeni")?.Text);
        Assert.IsNull(bank.Find("Slova/neznamy"));
        StringAssert.Contains(bank.Describe("Slova/razeni"), "řazení vlaku");
        Assert.AreEqual("Vlaknum/1", bank.Describe("Vlaknum/1"));
        StringAssert.Contains(bank.DescribeKey("2"), "na druhou kolej");
    }

    [TestMethod]
    public void Banka_ChybajucaJePrazdna()
    {
        using var installation = new TestInstallation();
        var bank = Iniss2Workspace.Load(installation.DataDir).Bank;

        Assert.IsEmpty(bank.Languages);
        Assert.AreEqual("Slova/razeni", bank.Describe("Slova/razeni"));
    }
}
