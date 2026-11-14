using System.Text.Json;
using GVXEditor.Domain.Analysis;
using GVXEditor.Domain.Server;

namespace GVXEditor.Tests;

/// <summary>
/// Vysledok mosta IniSrvBridge (JSON) a zistenia z neho.
/// </summary>
[TestClass]
public class ServerFindingsTests
{
    // tvar vystupu mosta (BridgeResult.ToJson)
    private const string Json = """
        {"ok":false,"fatal":"Chyba prekladu statedgm.xml.","stage":"statedgm.xml","milliseconds":900,
         "stages":["IniSrv.exe","Config.xml","statedgm.xml"],
         "log":["Chyba při rozvinutí datové poznámky xyz do období platnosti 14.12.2025 - 12.12.2026.\nIniSrv.DateRem+ParseException: Chybné datum xyz.\n   at IniSrv.DateRem.GetDate(String sToken)"],
         "compileErrors":[{"source":"statedgm.xml","line":1,"column":0,"number":"BC30451","message":"'Neznama' is not declared.",
                           "description":"Soubor \"StateDgm.xml\" v \"2030:01 Lipovec\", graf \"#Vychozi\", uzel \"Odjede\"\nKod","code":"Neznama And x"}]}
        """;

    [TestMethod]
    public void Most_JsonNaZistenia()
    {
        var verification = JsonSerializer.Deserialize<ServerVerification>(Json)!;

        Assert.IsFalse(verification.Ok);
        Assert.AreEqual(1 + 1, verification.ErrorCount);
        var findings = ServerFindings.Of(verification);

        // chyba prekladu nahradi celkove hlasenie, zaznam logu je upozornenie bez zasobnika
        Assert.HasCount(2, findings);
        var compile = findings.Single(finding => finding.Severity == FindingSeverity.Error);
        StringAssert.Contains(compile.Message, "BC30451");
        Assert.AreEqual("Soubor \"StateDgm.xml\" v \"2030:01 Lipovec\", graf \"#Vychozi\", uzel \"Odjede\"", compile.Place);
        var log = findings.Single(finding => finding.Severity == FindingSeverity.Warning);
        StringAssert.Contains(log.Message, "Chybné datum xyz.");
        Assert.DoesNotContain("   at ", log.Message);
    }

    [TestMethod]
    public void Most_ChybaNacitaniaBezPrekladu()
    {
        var verification = new ServerVerification { Fatal = "Chyba načítání souboru Positions.xml", Stage = "Positions.xml" };

        var finding = ServerFindings.Of(verification).Single();

        Assert.AreEqual(FindingSeverity.Error, finding.Severity);
        StringAssert.Contains(finding.Message, "Positions.xml");
    }
}
