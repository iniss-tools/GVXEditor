using GVXEditor.Domain;
using GVXEditor.Domain.Analysis;
using GVXEditor.Properties;

namespace GVXEditor.Tests;

/// <summary>
/// Kontrola instalacie: chyby, pre ktore by sa server nespustil, a upozornenia na udaje, ktore server pouzije inak.
/// </summary>
[TestClass]
public class ChecksTests
{
    private const string Trains = """
        <v o="2030-12-15" d="2031-12-13">
          <v>
            <z c="5400010" o="480" f="Z" dcv="2002" dv="Sp" />
            <z c="5400020" p="495" f="Z" />
          </v>
        </v>
        """;

    // instalacia so vsetkymi subormi, ktore server vyzaduje
    private static TestInstallation Complete(string trains)
    {
        var installation = new TestInstallation();
        installation.Trains(trains);
        foreach (var file in new[] { "zpozdeni.xml", "PigCols.xml" })
            installation.Write(@"Server\Data\" + file, "<X />");
        installation.Write(@"Server\Data\TrTypes.xml", """<TRTYPES><t key="Os" /><t key="Sp" /></TRTYPES>""");
        foreach (var station in new[] { "01 Lipovec", "02 Horany" })
        {
            installation.Write($@"Server\Data\2030\{station}\statedgm.xml", "<STATEDGM />");
            installation.Write($@"Server\Data\2030\{station}\ReportKeys.xml", "<REPORTKEYS />");
        }

        return installation;
    }

    private static List<Finding> Check(TestInstallation installation) => InstallationChecks.Run(Iniss2Workspace.Load(installation.DataDir));

    [TestMethod]
    public void Kontrola_UplnaInstalaciaBezChyb()
    {
        using var installation = Complete(Trains);

        Assert.IsFalse(Check(installation).Any(finding => finding.Severity == FindingSeverity.Error));
    }

    [TestMethod]
    public void Kontrola_ChybajuciPovinnySuborJeChyba()
    {
        using var installation = Complete(Trains);
        File.Delete(Path.Combine(installation.DataDir, "2030", "02 Horany", "statedgm.xml"));
        File.Delete(Path.Combine(installation.DataDir, "TrTypes.xml"));

        var errors = Check(installation).Where(finding => finding.Severity == FindingSeverity.Error).Select(finding => finding.Message).ToList();

        CollectionAssert.AreEquivalent(new[]
        {
            string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Check_MissingFile, "statedgm.xml"),
            string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Check_MissingFile, "TrTypes.xml")
        }, errors);
    }

    [TestMethod]
    public void Kontrola_ZaznamyStaniceANeznamyDruh()
    {
        using var installation = Complete(Trains.Replace("dv=\"Sp\"", "dv=\"REX\"", StringComparison.Ordinal));
        installation.Write(@"Server\Data\2030\01 Lipovec\Dodatky.xml", """<DODATKY><v cv="2002" k="D1" jd="toto nie je poznamka" /><v cv="3003" k="D2" /></DODATKY>""");

        var findings = Check(installation);

        Assert.IsTrue(findings.Any(finding => finding.Severity == FindingSeverity.Info && finding.Message.Contains("3003", StringComparison.Ordinal)));
        Assert.IsTrue(findings.Any(finding => finding.Severity == FindingSeverity.Warning && finding.Message.Contains("toto nie je poznamka", StringComparison.Ordinal)));
        var type = findings.Single(finding => finding.Message.Contains("REX", StringComparison.Ordinal));
        Assert.AreEqual((0, 0), (type.Group, type.Train));
    }

    [TestMethod]
    public void Kontrola_DvaVlakySRovnakymCislomVRovnakyDen()
    {
        using var installation = Complete(Trains + Trains.Replace("o=\"480\"", "o=\"600\"", StringComparison.Ordinal));

        var findings = Check(installation).Where(finding => finding.Message.Contains("2002", StringComparison.Ordinal)).ToList();

        Assert.IsTrue(findings.Any(finding => finding.Place.Contains("01 Lipovec", StringComparison.Ordinal)));
    }
}
