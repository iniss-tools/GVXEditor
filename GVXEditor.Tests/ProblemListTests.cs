using System.Drawing;
using System.Windows.Forms;
using GVXEditor.Domain.Analysis;
using GVXEditor.UI.Controls;

namespace GVXEditor.Tests;

/// <summary>
/// Zoznam zisteni na spodku okien: chyby su prve (poradie rovnako zavaznych ostava, okna podla indexu riadka skacu na miesto
/// zistenia) a riadok ma miesto pre ikonu zavaznosti.
/// </summary>
[TestClass]
public class ProblemListTests
{
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

    [TestMethod]
    public void Zoznam_ChybyPrvePoradieOstava() => Sta(() =>
    {
        using var list = new ProblemList();

        list.ShowProblems([(false, "upozornenie 1"), (true, "chyba 1"), (false, "upozornenie 2"), (true, "chyba 2")]);

        var items = list.Items.Cast<ProblemList.Problem>().ToList();
        CollectionAssert.AreEqual(new[] { "chyba 1", "chyba 2", "upozornenie 1", "upozornenie 2" }, items.Select(item => item.Message).ToArray());
        CollectionAssert.AreEqual(new[] { FindingSeverity.Error, FindingSeverity.Error, FindingSeverity.Warning, FindingSeverity.Warning },
            items.Select(item => item.Severity).ToArray());
        Assert.AreEqual("chyba 1", list.GetItemText(list.Items[0]), "text riadka je sprava");
    });

    [TestMethod]
    public void Riadok_VyskaPreIkonu() => Sta(() =>
    {
        using var list = new ProblemList { Font = new Font("Segoe UI", 6f) };
        list.CreateControl();

        Assert.IsTrue(list.ItemHeight >= list.LogicalToDeviceUnits(16), $"vyska riadka {list.ItemHeight}");
        Assert.AreEqual(DrawMode.OwnerDrawFixed, list.DrawMode);
    });

    [TestMethod]
    public void Ikona_PodlaZavaznosti()
    {
        Assert.AreEqual("status-error", ProblemList.IconName(FindingSeverity.Error));
        Assert.AreEqual("status-warning", ProblemList.IconName(FindingSeverity.Warning));
        Assert.AreEqual("status-info", ProblemList.IconName(FindingSeverity.Info));
    }
}
