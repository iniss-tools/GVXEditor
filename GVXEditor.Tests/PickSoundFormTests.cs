using System.Reflection;
using System.Windows.Forms;
using GVXEditor.Config;
using GVXEditor.Domain;
using GVXEditor.UI.Controls;
using ToolsCore;
using ToolsCore.Iniss.Entities;
using ToolsCore.XML;

namespace GVXEditor.Tests;

/// <summary>
/// Okno vyberu zvukov (radenie, zvuk kolaje): skupina dostane zvuky az pri rozbaleni, vybrany zvuk a malo najdenych zvukov
/// sa ukazu hned.
/// </summary>
[TestClass]
public class PickSoundFormTests
{
    private const string Bank = """
        <?xml version="1.0" encoding="UTF-8"?>
        <FYZBANK>
          <lang default="true" k="CZ" n="Čeština" d="CZ">
            <g k="K1" d="K1\">
              <z k="2" t="na druhou kolej" f="2.EWA" l="900" />
              <z k="3" t="na třetí kolej" f="3.EWA" l="900" />
            </g>
            <g k="Slova" d="Slova\">
              <z k="razeni" t="řazení vlaku" f="razeni.EWA" l="700" />
            </g>
          </lang>
        </FYZBANK>
        """;

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

    private static SoundBank CreateBank(TestInstallation installation)
    {
        var styles = Styles<GVXEditorStyle>.CreateDefault();
        // inicializator vlastnosti obide setter - tema okien cita styl z GlobSettings
        _ = new AppSession<GVXEditorConfig, GVXEditorStyle>(new GVXEditorConfig(), styles, styles.StyleList[0]) { UsingStyle = styles.StyleList[0] };
        var file = Path.Combine(installation.Root, "Server", "RawBank", "CZ", "FyzBank.xml");
        Directory.CreateDirectory(Path.GetDirectoryName(file)!);
        File.WriteAllText(file, Bank, new System.Text.UTF8Encoding(true));
        return Iniss2Workspace.Load(installation.DataDir).Bank;
    }

    private static TreeView Tree(FPickSound form) =>
        (TreeView)typeof(FPickSound).GetField("tvSounds", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(form)!;

    private static List<string> Sounds(TreeNode group) => group.Nodes.Cast<TreeNode>().Select(node => ((FyzSound)node.Tag!).Key).ToList();

    [TestMethod]
    public void Strom_ZvukySkupinyAzPriRozbaleni() => Sta(() =>
    {
        using var installation = new TestInstallation();
        using var form = new FPickSound(CreateBank(installation), "Radenie", []);
        form.CreateControl();
        var groups = Tree(form).Nodes.Cast<TreeNode>().ToList();

        CollectionAssert.AreEqual(new[] { "K1", "Slova" }, groups.Select(node => node.Text).ToArray());
        Assert.IsTrue(groups.All(node => node.Nodes.Count == 1 && node.Nodes[0].Tag is null), "len uzol pre tlacidlo rozbalenia");

        groups[0].Expand();

        CollectionAssert.AreEqual(new[] { "2", "3" }, Sounds(groups[0]));
    });

    [TestMethod]
    public void Strom_VybranyZvukKolajeJeHnedVidno() => Sta(() =>
    {
        using var installation = new TestInstallation();
        using var form = new FPickSound(CreateBank(installation), "Kolaj", null, "razeni");
        form.CreateControl();

        Assert.AreEqual("razeni", form.SoundKey);
        Assert.AreEqual(1, Tree(form).Nodes[0].Nodes.Count, "ina skupina ostava bez zvukov");
    });

    [TestMethod]
    public void Hladanie_MaloNajdenychZvukovJeRozbalenych() => Sta(() =>
    {
        using var installation = new TestInstallation();
        using var form = new FPickSound(CreateBank(installation), "Radenie", []);
        form.CreateControl();

        form.Controls.Find("tbSoundFilter", true).Single().Text = "třetí";

        var group = Tree(form).Nodes.Cast<TreeNode>().Single();
        Assert.AreEqual("K1", group.Text);
        CollectionAssert.AreEqual(new[] { "3" }, Sounds(group));
    });
}
