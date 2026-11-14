using System.Globalization;
using GVXEditor.Domain.Editing;
using GVXEditor.Domain.Rules;
using GVXEditor.Properties;
using static GVXEditor.Tests.EditingTests;

namespace GVXEditor.Tests;

/// <summary>
/// Kontrola vlaku v okne Upravit vlak: chyby branu ulozeniu, upozornenia hovoria, co by server urobil inak.
/// </summary>
[TestClass]
public class TrainRulesTests
{
    private static readonly IReadOnlySet<int> Interest = new HashSet<int> { TestInstallation.Lipovec, TestInstallation.Horany };

    private static string Name(int number) => number switch
    {
        TestInstallation.Lipovec => "Lipovec",
        TestInstallation.Horany => "Horany",
        _ => number.ToString(CultureInfo.InvariantCulture)
    };

    private static List<TrainProblem> Check(TrainDraft draft) => TrainRules.Check(draft, Interest, Name);

    private static string Text(string format, params object[] args) => string.Format(CultureInfo.CurrentCulture, format, args);

    [TestMethod]
    public void Pravidla_PlatnyVlakBezZisteni()
    {
        var draft = Draft(Stop(TestInstallation.Lipovec, "", "8:00", "1001"), Stop(TestInstallation.Horany, "8:15", ""));

        Assert.IsEmpty(Check(draft));
    }

    [TestMethod]
    public void Pravidla_ChybnyCasAStanica()
    {
        var draft = Draft(Stop(TestInstallation.Lipovec, "", "8:61", "1001"), Stop(0, "24:00", ""));
        draft.Variants[0].Stops[1].Station = "abc";

        var problems = Check(draft);

        Assert.IsTrue(problems.All(problem => problem.IsError));
        CollectionAssert.AreEquivalent(new[]
        {
            Text(Resources.Rule_BadTime, 1, 1, "8:61"),
            Text(Resources.Rule_BadStation, 1, 2, "abc"),
            Text(Resources.Rule_BadTime, 1, 2, "24:00")
        }, problems.Select(problem => problem.Message).ToList());
        Assert.AreEqual((0, 1), (problems[1].Variant, problems[1].Stop));
    }

    [TestMethod]
    public void Pravidla_ObdobieJedenBodACisloVlaku()
    {
        var draft = Draft(Stop(TestInstallation.Lipovec, "", "8:00"));
        draft.To = draft.From.AddDays(-1);
        draft.Variants.Add(new VariantDraft { Stops = [Stop(TestInstallation.Lipovec, "", "8:00"), Stop(TestInstallation.Horany, "8:15", "")] });

        var messages = Check(draft).Select(problem => problem.Message).ToList();

        CollectionAssert.AreEquivalent(new[] { Resources.Rule_PeriodReversed, Text(Resources.Rule_FewStops, 1), Text(Resources.Rule_NoNumber, 2) }, messages);
    }

    [TestMethod]
    public void Pravidla_UpozorneniaNaPoznamkuAVariantu()
    {
        var draft = Draft(Stop(TestInstallation.Lipovec, "", "8:00", "1001"), Stop(TestInstallation.Horany, "8:15", ""));
        draft.Variants.Add(draft.Variants[0].Clone());
        draft.Variants.Add(draft.Variants[0].Clone());
        draft.Variants[2].Validity = "toto nie je poznamka";

        var problems = Check(draft);

        Assert.IsFalse(problems.Any(problem => problem.IsError));
        CollectionAssert.AreEquivalent(new[] { Text(Resources.Rule_VariantNeverActive, 2), Text(Resources.Rule_ValidityInvalid, 3, "toto nie je poznamka") },
            problems.Select(problem => problem.Message).ToList());
    }

    [TestMethod]
    public void Pravidla_StanicaGrafikonuBezCasu()
    {
        var draft = Draft(Stop(TestInstallation.Javor, "", "8:00", "1001"), Stop(TestInstallation.Lipovec, "", ""), Stop(5400099, "8:30", ""));

        var messages = Check(draft).Select(problem => problem.Message).ToList();

        CollectionAssert.AreEquivalent(new[] { Text(Resources.Rule_InterestWithoutTime, 1, "Lipovec"), Resources.Rule_NoInterestStop }, messages);
    }

    [TestMethod]
    public void Pravidla_ObdobieNad400DniPriPoznamke()
    {
        var draft = Draft(Stop(TestInstallation.Lipovec, "", "8:00", "1001"), Stop(TestInstallation.Horany, "8:15", ""));
        draft.To = draft.From.AddDays(400);
        draft.Variants[0].Validity = "jede v 1-5";

        var problem = Check(draft).Single();

        Assert.AreEqual((false, 0, Text(Resources.Rule_ValidityTooLong, 1)), (problem.IsError, problem.Variant, problem.Message));
    }
}
