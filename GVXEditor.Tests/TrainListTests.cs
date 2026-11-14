using System.Reflection;
using System.Windows.Forms;
using GVXEditor.Config;
using GVXEditor.Domain;
using GVXEditor.Domain.Editing;
using GVXEditor.Domain.Views;
using GVXEditor.UI.Main;
using ToolsCore;
using ToolsCore.Iniss2.Grafikon;
using ToolsCore.XML;

namespace GVXEditor.Tests;

/// <summary>
/// Zoznam vlakov hlavneho okna: skupiny druhov pre farby, triedenie podla stlpca (treti klik vrati poradie GV.XML), vyber
/// viacerych vlakov a hromadne zmeny ako jeden krok historie.
/// </summary>
[TestClass]
public class TrainListTests
{
    private const BindingFlags Any = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    // cislo, druh, odchod z vychodzej stanice (minuty)
    private const string Trains = """
        <v o="2030-12-15" d="2031-12-13"><v jd="denne"><z c="5400010" o="360" dcv="3101" dv="Os" /><z c="5400020" p="380" /></v></v>
        <v o="2030-12-15" d="2031-12-13"><v jd="denne"><z c="5400010" o="510" dcv="611" dv="R" /><z c="5400020" p="530" /></v></v>
        <v o="2030-12-15" d="2031-12-13"><v jd="denne"><z c="5400010" o="300" dcv="171" dv="EC" /><z c="5400020" p="320" /></v></v>
        <v o="2030-12-15" d="2031-12-13"><v jd="denne"><z c="5400010" o="720" dcv="99" dv="Sl" /><z c="5400020" p="740" /></v></v>
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

    [TestMethod]
    [DataRow("R", "R")]
    [DataRow("rex", "R")]
    [DataRow("RLJ", "X")]
    [DataRow("Nákl", "Sl")]
    [DataRow("Os", "Os")]
    [DataRow("ALX", "R")]
    [DataRow("Tram", "Os")]
    public void Druh_SkupinaAkoKlientAleboPodlaZvuku(string type, string expected)
    {
        TrType[] types = [new("ALX", "Alex", "Ex", "ALX"), new("Tram", "Tramvaj", "Bus", "MHD")];

        Assert.AreEqual(expected, TrainTypeGroups.Of(type, types).ToString());
    }

    [TestMethod]
    public void Triedenie_CislaPodlaHodnoty()
    {
        string[] numbers = ["3101", "611", "Os 10", "Os 2", "", "99 / 199", "0171"];

        var sorted = numbers.Order(Comparer<string>.Create(NaturalText.Compare)).ToArray();

        CollectionAssert.AreEqual(new[] { "", "99 / 199", "0171", "611", "3101", "Os 2", "Os 10" }, sorted);
    }

    [TestMethod]
    public void Triedenie_RovnakeHodnotyVPoradiSuboru()
    {
        var rows = new[] { Row(0, "R", 600), Row(1, "Os", 300), Row(2, "R", 300), Row(3, "Os", null) };

        Assert.AreEqual("1,3,0,2", Indexes(TrainRowOrder.Sort(rows, nameof(TrainRow.Type), false)));
        Assert.AreEqual("0,2,1,3", Indexes(TrainRowOrder.Sort(rows, nameof(TrainRow.Type), true)));
        Assert.AreEqual("3,1,2,0", Indexes(TrainRowOrder.Sort(rows, nameof(TrainRow.Departure), false)));
        Assert.AreEqual("0,1,2,3", Indexes(TrainRowOrder.Sort(rows.Reverse(), null, false)));
    }

    private static TrainRow Row(int index, string type, int? departure) =>
        new(new GvTrain(DateOnly.MinValue, DateOnly.MaxValue, []), "", type, "", "", "", "", "", "", 1, "") { Index = index, DepartureMinutes = departure };

    private static string Indexes(IEnumerable<TrainRow> rows) => string.Join(",", rows.Select(row => row.Index));

    [TestMethod]
    public void Uprava_KopiaZmazanieAPosunVybranych()
    {
        var trains = Enumerable.Range(0, 4).Select(day => new GvTrain(new DateOnly(2031, 1, 1).AddDays(day), new DateOnly(2031, 12, 1), [])).ToList();
        var timetable = new GvFile([], trains);

        var copied = TimetableEditing.Copy(timetable, [3, 1]);
        CollectionAssert.AreEqual(new[] { trains[0], trains[1], trains[1], trains[2], trains[3], trains[3] }, copied.Trains.ToArray());

        var removed = TimetableEditing.Remove(timetable, [0, 2]);
        CollectionAssert.AreEqual(new[] { trains[1], trains[3] }, removed.Trains.ToArray());

        var shifted = TimetableEditing.ShiftValidity(timetable, 7, new HashSet<int> { 1 });
        Assert.AreEqual((trains[0].From, trains[1].From.AddDays(7)), (shifted.Trains[0].From, shifted.Trains[1].From));
    }

    private static FMain OpenMain(TestInstallation installation)
    {
        var styles = Styles<GVXEditorStyle>.CreateDefault();
        var session = new AppSession<GVXEditorConfig, GVXEditorStyle>(new GVXEditorConfig(), styles, styles.StyleList[0]);
        // inicializator vlastnosti obide setter - tema okien cita styl z GlobSettings
        session.UsingStyle = styles.StyleList[0];
        var context = new EditorContext(session);
        var form = new FMain(context);
        form.CreateControl();

        // ako Otvorit instalaciu, bez zapisu do zoznamu nedavnych instalacii
        var workspace = Iniss2Workspace.Load(installation.DataDir);
        context.Open(workspace);
        typeof(FMain).GetMethod("FillTree", Any)!.Invoke(form, [workspace]);
        var tree = Field<TreeView>(form, "tvData");
        tree.SelectedNode = tree.Nodes[0];
        return form;
    }

    private static T Field<T>(object target, string name) => (T)target.GetType().GetField(name, Any)!.GetValue(target)!;

    private static DataGridView Grid(FMain form) => Field<DataGridView>(form, "dgvTrains");

    private static string Numbers(DataGridView grid) => string.Join(",", grid.Rows.Cast<DataGridViewRow>().Select(row => ((TrainRow)row.DataBoundItem!).Numbers));

    private static void ClickHeader(FMain form, string column)
    {
        var grid = Grid(form);
        var args = new DataGridViewCellMouseEventArgs(grid.Columns[column]!.Index, -1, 0, 0, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        typeof(FMain).GetMethod("dgvTrains_ColumnHeaderMouseClick", Any)!.Invoke(form, [grid, args]);
    }

    [TestMethod]
    public void HlavneOkno_TriedenieTretimKlikomVratiPoradieSuboru() => Sta(() =>
    {
        using var installation = new TestInstallation();
        installation.Trains(Trains);
        using var form = OpenMain(installation);
        var grid = Grid(form);
        grid.Rows[2].Selected = true;
        grid.CurrentCell = grid.Rows[2].Cells[0];

        ClickHeader(form, "cNumbers");
        Assert.AreEqual("99,171,611,3101", Numbers(grid));
        Assert.AreEqual("171", ((TrainRow)grid.CurrentRow!.DataBoundItem!).Numbers, "kurzor ostava na vlaku");

        ClickHeader(form, "cNumbers");
        Assert.AreEqual("3101,611,171,99", Numbers(grid));

        ClickHeader(form, "cNumbers");
        Assert.AreEqual("3101,611,171,99", Numbers(grid));
        Assert.AreEqual(SortOrder.None, grid.Columns["cNumbers"]!.HeaderCell.SortGlyphDirection);

        ClickHeader(form, "cDeparture");
        Assert.AreEqual("171,3101,611,99", Numbers(grid));
    });

    [TestMethod]
    public void HlavneOkno_KopiaVybranychVlakovJedenKrokSpat() => Sta(() =>
    {
        using var installation = new TestInstallation();
        installation.Trains(Trains);
        using var form = OpenMain(installation);
        var grid = Grid(form);
        ClickHeader(form, "cNumbers");

        // v zoradenom zozname vybrane 99 a 611 (v GV.XML 4. a 2. vlak)
        grid.CurrentCell = grid.Rows[0].Cells[0];
        grid.Rows[2].Selected = true;
        typeof(FMain).GetMethod("CopyTrain", Any)!.Invoke(form, null);

        var context = Field<EditorContext>(form, "_ctx");
        var trains = context.Workspace!.Groups[0].Timetable!.Trains;
        Assert.AreEqual(6, trains.Count);
        Assert.AreSame(trains[1], trains[2]);
        Assert.AreSame(trains[4], trains[5]);
        Assert.AreEqual("kópia vlakov (2)", context.Workspace.History.UndoLabels[0]);
        Assert.AreEqual(2, grid.SelectedRows.Count);
        Assert.AreEqual("99", ((TrainRow)grid.CurrentRow!.DataBoundItem!).Numbers);
        Assert.AreEqual(5, ((TrainRow)grid.CurrentRow.DataBoundItem!).Index, "kurzor na kopii");
    });
}
