using System.Globalization;
using System.Reflection;
using ExControls;
using GVXEditor.Domain.Editing;
using GVXEditor.Domain.Setup;
using GVXEditor.Domain.Stations;
using GVXEditor.Domain.Views;
using GVXEditor.Properties;
using GVXEditor.UI.Analysis;
using GVXEditor.UI.Boards;
using GVXEditor.UI.Code;
using GVXEditor.UI.Controls;
using GVXEditor.UI.EditTrain;
using GVXEditor.UI.Main;
using GVXEditor.UI.Settings;
using GVXEditor.UI.Setup;

namespace GVXEditor.DocScreenshots;

/// <summary>
/// Zoznam snímok. Každá snímka je okno uložené ako <c>&lt;priečinok&gt;/&lt;názov&gt;-light.png</c> a <c>-dark.png</c>;
/// priečinok je názov článku v <c>docs/gvxeditor</c>.
/// </summary>
/// <remarks>
/// Ak premenná prostredia <c>DOCSHOTS_BOUNDS</c> určuje súbor, pripíšu sa doň polohy prvkov každej snímky
/// v percentách – podľa nich sa v článkoch kreslia číslované značky.
/// </remarks>
internal sealed class Shots(Program.Options options, string theme, List<string> log, EditorContext context)
{
    private const BindingFlags Any = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private int _count;

    public int Run(string installation)
    {
        log.Add($"krok: otvorenie hlavného okna ({theme})");
        var main = Program.OpenMain(context, installation);

        try
        {
            // hlavné okno: grafikon, rýchlik 611 s dvoma variantmi, víkendová varianta
            Shot("prve-kroky/hlavne-okno", main, _ =>
            {
                var tree = Field<ExTreeView>(main, "tvData");
                tree.SelectedNode = tree.Nodes[0];
                Pump.Events();
                SelectRow(Field<DataGridView>(main, "dgvTrains"), row => row.Cells["cNumbers"].Value as string == "611");
                SelectRow(Field<DataGridView>(main, "dgvVariants"), row => row.Index == 0);
            }, dispose: false);

            // vlaky stanice Lipovec v ukážkový piatok
            Shot("prve-kroky/stanica-na-den", main, _ =>
            {
                var tree = Field<ExTreeView>(main, "tvData");
                tree.SelectedNode = tree.Nodes[0].Nodes[0];
                Pump.Events();
                Field<ExDateTimePicker>(main, "dtpDay").Value = DemoInstallation.Day.ToDateTime(TimeOnly.MinValue);
                Pump.Events();
                Field<DataGridView>(main, "dgvStation").ClearSelection();
            }, dispose: false);

            // okno Upraviť vlak: rýchlik 611, víkendová varianta s kalendárom
            Shot("vlaky/upravit-vlak", () => EditTrain("611"), form =>
            {
                SelectRow(Field<DataGridView>(form, "dgvVariants"), row => row.Index == 0);
                Field<DataGridView>(form, "dgvStops").ClearSelection();
            });

            // karta V staniciach: dodatky rychlika 611 v Lipovci
            Shot("vlaky/upravit-vlak-stanice", () => EditTrain("611"), form =>
            {
                var tabs = Field<ExTabControl>(form, "tcMain");
                tabs.SelectedTab = Field<TabPage>(form, "tpStations");
                Pump.Events();
                var station = Field<ExTabControl>(form, "tcStation");
                station.SelectedTab = Field<TabPage>(form, "tpAdditions");
                Pump.Events();
                // tabulka upravuje bunku hned po vstupe - bez aktualnej bunky nie je text oznaceny
                var additions = Field<DataGridView>(form, "dgvAdditions");
                additions.CurrentCell = null;
                additions.ClearSelection();
            });

            // karta Vyluky a odklony
            Shot("vlaky/upravit-vlak-vyluky", () => EditTrain("611"), form =>
            {
                Field<ExTabControl>(form, "tcMain").SelectedTab = Field<TabPage>(form, "tpNotices");
                Pump.Events();
                Field<DataGridView>(form, "dgvLockouts").ClearSelection();
                Field<DataGridView>(form, "dgvDiversions").ClearSelection();
            });

            // okna vyberu: stanice v hlaseni v poradi a typy hlasenia zaskrtnutim
            Shot("vlaky/vyber-stanic", () =>
            {
                var workspace = context.Workspace!;
                var stations = workspace.StationNames.OrderBy(pair => pair.Value, StringComparer.CurrentCulture)
                    .Select(pair => new ValueChoice(pair.Key.ToString(CultureInfo.InvariantCulture), pair.Value)).ToList();
                return new FPickOrdered(Resources.Pick_StationsTitle, stations, [stations[^1].Key, stations[1].Key]);
            });

            Shot("vlaky/vyber-typov-hlasenia", () => new FPickChecked(Resources.Pick_ReportsTitle, Resources.Pick_ReportsHint,
                [
                    new ValueChoice("P", "Přijede (P)"), new ValueChoice("V", "Vjíždí (V)"), new ValueChoice("N", "Nástup (N)"),
                    new ValueChoice("O", "Odjede (O)"), new ValueChoice("-", Resources.Pick_NoReport)
                ], ["V", "N"]));

            // zvuky radenia v poradi a zvuk kolaje zo zvukovej banky
            Shot("vlaky/vyber-zvukov", () => new FPickSound(context.Workspace!.Bank, Resources.Pick_SoundsTitle,
                ["CZ/Slova/razeni", "CZ/Slova/1trida", "CZ/Slova/vpredu"]), form =>
            {
                var tree = Field<ExTreeView>(form, "tvSounds");
                tree.Nodes[2].Expand();
                tree.SelectedNode = tree.Nodes[2].Nodes[2];
                Pump.Events();
            });

            Shot("nastavenie/vyber-zvuku-kolaje", () => new FPickSound(context.Workspace!.Bank, "Zvuk koľaje", null, "2"));

            Shot("vlaky/posunut-platnost", () => new FShiftValidity(context.Workspace!.Groups[0].Group.Name));

            Shot("vlaky/premenovat-cislo", () => RenameNumber("611"));

            // okno Nastavenie grafikonu: nazvy stanic, kolaje, vzory stanic v hlaseni a texty podla trasy stanice Lipovec
            Shot("nastavenie/nastavenie-grafikonu", () => Setup(-1), form =>
            {
                Field<DataGridView>(form, "dgvNames").ClearSelection();
                // bez zmeny karty harness zachyti okno skor, nez sa karty prekreslia
                var tabs = Field<ExTabControl>(form, "tcMain");
                tabs.Invalidate();
                tabs.Update();
            });

            Shot("nastavenie/nastavenie-stanice", () => Setup(0), form =>
                Field<DataGridView>(form, "dgvTracks").ClearSelection());

            Shot("nastavenie/stanice-v-hlaseni", () => Setup(0), form =>
            {
                Field<ExTabControl>(form, "tcStation").SelectedTab = Field<TabPage>(form, "tpPatterns");
                Pump.Events();
                SelectRow(Field<DataGridView>(form, "dgvPatterns"), row => row.Index == 0);
                Field<DataGridView>(form, "dgvPatternStops").ClearSelection();
            });

            Shot("nastavenie/texty-podla-trasy", () => Setup(0), form =>
            {
                Field<ExTabControl>(form, "tcStation").SelectedTab = Field<TabPage>(form, "tpRouteTexts");
                Pump.Events();
                SelectRow(Field<DataGridView>(form, "dgvRouteSets"), row => row.Index == 0);
                SelectRow(Field<DataGridView>(form, "dgvRoutes"), row => row.Index == 0);
                Field<DataGridView>(form, "dgvRouteTargets").ClearSelection();
                Field<DataGridView>(form, "dgvRouteStops").ClearSelection();
            });

            Shot("nastavenie/hlasenia-obsluhy", () =>
            {
                var workspace = context.Workspace!;
                var keys = new KeyCatalog(workspace.Functions, workspace.FunctionDescriptions, workspace.ConstReports);
                return new FConstReports(workspace.ConstReports, keys, ConstReportsEdit.UsedKeys(workspace.Groups), workspace.StationNames);
            }, form =>
            {
                var reports = Field<DataGridView>(form, "dgvReports");
                reports.CurrentCell = null;
                reports.ClearSelection();
            });

            Shot("nastavenie/novy-grafikon", () => new FNewGroup(context.Workspace!, 0));

            Shot("nastavenie/pridat-stanicu", () => new FAddStation(context.Workspace!, 0, 0), form =>
                Field<TextBox>(form, "tbNumber").Text = DemoInstallation.BukovaNumber);

            // okna kodu: stav Nastup s automatikou, tabulka PRIZNAK so slovnikom, skript hlaseni
            Shot("kod/stavovy-diagram", () => new FStateDiagram(context, 0, 0), form =>
            {
                var tree = Field<ExTreeView>(form, "tvGraphs");
                tree.SelectedNode = tree.Nodes[0].Nodes[2];
                Pump.Events();
            });

            Shot("kod/tabulky-tabtab", () => new FTabTabs(context, 0, -1), form =>
            {
                Field<ListBox>(form, "lbTables").SelectedIndex = 1;
                Pump.Events();
                Field<DataGridView>(form, "dgvEntries").ClearSelection();
            });

            Shot("kod/skript-hlaseni", () => new FLogText(context));

            // okno Hlasenia: zakladne hlasenie so strazou, vstavanymi polozkami a podmienkou, rucne hlasenie s vyberom podla stanice
            Shot("kod/hlasenia", () => new FReports(context), form =>
            {
                Method(form, "SelectReport", "Přijede");
                Pump.Events();
                Field<DataGridView>(form, "dgvSteps").ClearSelection();
            });

            Shot("kod/hlasenia-rucne", () => new FReports(context), form =>
            {
                Method(form, "SelectReport", "Odjede");
                Pump.Events();
            });

            // okno Tabule stanice Lipovec: tabula v hale, cast nastupistia s podmienkou a kolajami, predloha odchodov
            Shot("tabule/tabula", () => new FBoards(context, 0, 0), form =>
            {
                var tree = Field<ExTreeView>(form, "tvBoards");
                tree.SelectedNode = tree.Nodes[0];
                Pump.Events();
            });

            Shot("tabule/cast", () => new FBoards(context, 0, 0), form =>
            {
                var tree = Field<ExTreeView>(form, "tvBoards");
                tree.SelectedNode = tree.Nodes[1].Nodes[0];
                Pump.Events();
                Field<DataGridView>(form, "dgvTracks").ClearSelection();
            });

            Shot("tabule/predlohy", () => new FBoards(context, 0, 0), form =>
            {
                Field<TabControl>(form, "tcMain").SelectedIndex = 1;
                Pump.Events();
                Field<ListBox>(form, "lbTemplates").SelectedIndex = 0;
                Pump.Events();
                Field<DataGridView>(form, "dgvColumns").ClearSelection();
            });

            // generovanie textov vlakov stanice Lipovec do novej sady pre udaj Smer
            Shot("tabule/generovat-texty", () => new FGenerateTexts(context.Workspace!.Groups[0], 0, context.Workspace.StationName, true), form =>
            {
                var sets = Field<ComboBox>(form, "cboSet");
                sets.SelectedIndex = Math.Max(0, sets.Items.Cast<object>().ToList().FindIndex(item => item.ToString()!.Contains("/ Smer", StringComparison.Ordinal)));
                Pump.Events();
                Field<DataGridView>(form, "dgvPreview").ClearSelection();
            });

            // panel Kontrola instalacie v hlavnom okne (prikaz Skontrolovat instalaciu)
            Shot("kontrola/kontrola-instalacie", main, _ =>
            {
                var tree = Field<ExTreeView>(main, "tvData");
                tree.SelectedNode = tree.Nodes[0];
                Pump.Events();
                typeof(FMain).GetMethod("ShowAnalysis", Any)!.Invoke(main, null);
                Pump.Events();
                var view = Field<AnalysisView>(main, "_analysisView");
                Field<DataGridView>(view, "dgvFindings").ClearSelection();
                Field<DataGridView>(main, "dgvTrains").ClearSelection();
            }, dispose: false);

            Shot("nastavenia-programu/nastavenia-programu", () => new FAppSettings(context));

            // server MCP pre agentov (bez beziaceho servera - stav Vypnuty)
            Shot("nastavenia-programu/agenti", () =>
            {
                var form = new FAppSettings(context);
                form.PreselectMenuItem("pMcp");
                return form;
            });

            // farebna schema: editor kodu VB.NET s vybranym klucovym slovom
            Shot("nastavenia-programu/farebna-schema", () =>
            {
                var form = new FAppSettings(context);
                form.PreselectMenuItem("pStyles");
                return form;
            }, form =>
            {
                var tree = (TreeView)form.Controls.Find("tvKeys", true).Single();
                tree.SelectedNode = tree.Nodes.Cast<TreeNode>().SelectMany(node => node.Nodes.Cast<TreeNode>())
                    .First(node => (node.Tag as ToolsCore.XML.ColorKey)?.Id == "VisualBasic.Keyword");
                Pump.Events();
            });
        }
        finally
        {
            main.Dispose();
        }

        return _count;
    }

    private FEditTrain EditTrain(string number)
    {
        var workspace = context.Workspace!;
        var group = workspace.Groups[0];
        var trains = group.Timetable!.Trains;
        var index = trains.ToList().FindIndex(train => TrainRow.Create(train, workspace.StationName).Numbers.Split(" / ").Contains(number));
        var title = string.Format(CultureInfo.CurrentCulture, Resources.Edit_Title, number);
        return new FEditTrain(context, TrainDraft.Of(trains[index]), group, index, workspace.StationName, title);
    }

    private FSetup Setup(int station)
    {
        var workspace = context.Workspace!;
        return new FSetup(workspace.Groups[0], workspace.StationName, station, workspace.Bank);
    }

    private FRenameNumber RenameNumber(string number)
    {
        var group = context.Workspace!.Groups[0];
        var index = group.Timetable!.Trains.ToList()
            .FindIndex(train => TrainRow.Create(train, context.Workspace.StationName).Numbers.Split(" / ").Contains(number));
        return new FRenameNumber(group, index);
    }

    private static void SelectRow(DataGridView grid, Func<DataGridViewRow, bool> match)
    {
        var row = grid.Rows.Cast<DataGridViewRow>().First(match);
        grid.ClearSelection();
        grid.CurrentCell = row.Cells.Cast<DataGridViewCell>().First(cell => cell.Visible);
        row.Selected = true;
        Pump.Events();
    }

    private void Shot(string name, Func<Form> create, Action<Form>? setup = null) => Shot(name, create(), setup);

    private void Shot(string name, Form form, Action<Form>? setup = null, bool dispose = true)
    {
        log.Add($"krok: {name} ({theme})");
        try
        {
            if (!form.Visible)
            {
                form.StartPosition = FormStartPosition.Manual;
                form.Location = new Point(60, 60);
                form.ShowInTaskbar = false;
                form.Show();
            }

            Pump.Events();
            setup?.Invoke(form);
            Pump.Events();
            form.Refresh();
            Pump.Events();
            Save(name, form);
        }
        catch (Exception e)
        {
            log.Add($"{name} ({theme}): {e.GetType().Name}: {(e is TargetInvocationException { InnerException: { } inner } ? inner.Message : e.Message)}");
        }
        finally
        {
            if (dispose)
                form.Dispose();
        }
    }

    private void Save(string name, Form form)
    {
        if (options.Only is not null && !name.Contains(options.Only, StringComparison.OrdinalIgnoreCase))
            return;

        var file = Path.Combine(options.OutDir, $"{name}-{theme}.png".Replace('/', Path.DirectorySeparatorChar));
        WindowCapture.Save(form, file);
        WriteBounds(name, form);
        _count++;
    }

    /// <summary>
    /// Pre značky v článkoch: polohy prvkov okna v percentách snímky (ľavý okraj, stred, horný okraj). Zapíše sa,
    /// len ak premenná prostredia <c>DOCSHOTS_BOUNDS</c> určuje súbor.
    /// </summary>
    private void WriteBounds(string shot, Form form)
    {
        if (Environment.GetEnvironmentVariable("DOCSHOTS_BOUNDS") is not { Length: > 0 } file || theme != "light")
            return;

        var frame = WindowCapture.FrameBounds(form);
        var lines = new List<string>();

        string P(int v, int from, int size) => (100.0 * (v - from) / size).ToString("0.0", System.Globalization.CultureInfo.InvariantCulture);

        void Add(string name, string type, Rectangle r, string text) =>
            lines.Add($"{shot} {name} {type} left={P(r.Left, frame.Left, frame.Width)} cx={P(r.Left + r.Width / 2, frame.Left, frame.Width)} " +
                      $"top={P(r.Top, frame.Top, frame.Height)} cy={P(r.Top + r.Height / 2, frame.Top, frame.Height)} \"{text}\"");

        foreach (var control in Descendants(form).Where(c => c.Visible && c.Name.Length > 0 && c.Width > 0 && c.IsHandleCreated))
        {
            if (control is not (ButtonBase or DataGridView or TextBoxBase or ComboBox or DateTimePicker or TreeView or Label or ToolStrip))
                continue;

            Add(control.Name, control.GetType().Name, control.RectangleToScreen(control.ClientRectangle),
                control is Label or ButtonBase ? control.Text.Split('\n')[0].TrimEnd() : "");

            if (control is ToolStrip strip)
                foreach (ToolStripItem item in strip.Items)
                    if (item.Visible && item.Name is { Length: > 0 })
                    {
                        var r = item.Bounds;
                        r.Offset(strip.PointToScreen(Point.Empty));
                        Add(item.Name, item.GetType().Name, r, item.Text ?? "");
                    }
        }

        File.AppendAllLines(file, lines);
    }

    private static T Field<T>(Control form, string name) =>
        (T)form.GetType().GetField(name, Any)!.GetValue(form)!;

    private static void Method(Control form, string name, params object[] args) =>
        form.GetType().GetMethod(name, Any)!.Invoke(form, args);

    private static IEnumerable<Control> Descendants(Control control)
    {
        foreach (Control child in control.Controls)
        {
            yield return child;
            foreach (var nested in Descendants(child))
                yield return nested;
        }
    }
}
