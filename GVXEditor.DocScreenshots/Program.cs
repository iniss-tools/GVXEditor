using System.Globalization;
using System.Reflection;
using GVXEditor.Config;
using GVXEditor.UI.Main;
using Microsoft.Win32;
using ToolsCore;
using ToolsCore.Iniss.Calendar;
using ToolsCore.Tools;
using ToolsCore.XML;

namespace GVXEditor.DocScreenshots;

/// <summary>
/// Generátor snímok okien GVXEditora do dokumentácie.
/// </summary>
/// <remarks>
/// Použitie: <c>GVXEditor.DocScreenshots [--out=priečinok] [--work=priečinok] [--only=text] [--theme=light|dark|both]</c>.
/// <list type="bullet">
/// <item><c>--out</c> – kam uložiť PNG; predvolene <c>iniss-tools-docs\static\img\gvxeditor</c> vedľa repozitára.</item>
/// <item><c>--work</c> – kde zostaviť ukážkovú inštaláciu; predvolene <c>C:\INISS2</c> (cesta je vidno v stavovom riadku).
/// Existujúci priečinok bez značky <c>.docshots</c> sa nezmaže.</item>
/// <item><c>--timeout</c> – po koľkých minútach sa harness ukončí, ak ho zablokuje modálne okno (predvolene 5).</item>
/// <item><c>--only</c> – len snímky, ktorých cesta obsahuje daný text (napr. <c>hlavne-okno</c>).</item>
/// </list>
/// Program beží pod vlastným menom, takže konfiguráciu (<c>%LocalAppData%\GVXEditor.DocScreenshots</c>) aj register má
/// oddelené od GVXEditora – pri každom spustení začína s predvolenými nastaveniami.
/// </remarks>
internal static class Program
{
    private const BindingFlags Any = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    [STAThread]
    private static int Main(string[] args)
    {
        Options options;
        try
        {
            options = Options.Parse(args);
        }
        catch (ArgumentException e)
        {
            Console.Error.WriteLine(e.Message);
            return 1;
        }

        var log = new List<string>();

        // poistka: modálne okno (chyba, otázka) by harness zablokovalo navždy
        using var watchdog = new System.Threading.Timer(_ =>
        {
            Console.Error.WriteLine("Časový limit vypršal – pravdepodobne visí modálne okno. Posledné kroky:");
            foreach (var line in log.TakeLast(5)) Console.Error.WriteLine("  " + line);
            Console.Error.WriteLine("Otvorené okná: " + string.Join(" | ", WindowCapture.ProcessWindowTitles()));
            Environment.Exit(2);
        }, null, TimeSpan.FromMinutes(options.TimeoutMinutes), Timeout.InfiniteTimeSpan);

        try
        {
            var context = InitApp();
            var installation = DemoInstallation.Build(options.WorkDir);
            AppRegistry.SetUsageOfProject(Path.Combine(installation, "Server", "Data"));
            using (var screen = Graphics.FromHwnd(IntPtr.Zero))
                log.Add($"DPI: {screen.DpiX} (snímky majú rozmery podľa škálovania obrazovky, pre docs 96 = 100 %)");

            var shots = 0;
            foreach (var theme in options.Themes)
            {
                SetTheme(context, theme);
                shots += new Shots(options, theme, log, context).Run(installation);
            }

            log.Add($"hotovo: {shots} snímok do {options.OutDir}");
            Flush(log);
            return 0;
        }
        catch (Exception e)
        {
            log.Add("CHYBA: " + (e is TargetInvocationException { InnerException: { } inner } ? inner : e));
            Flush(log);
            return 1;
        }
    }

    private static void Flush(List<string> log)
    {
        foreach (var line in log) Console.WriteLine(line);
    }

    /// <summary>
    /// Rovnaká inicializácia ako GVXEditor.Program.Main, s čistou konfiguráciou, registrom a slovenčinou.
    /// </summary>
    private static EditorContext InitApp()
    {
        if (Directory.Exists(AppPaths.DataDir))
            Directory.Delete(AppPaths.DataDir, true);

        // zoznam nedávnych inštalácií – kľúč patrí harnessu (názov zostavy), nie GVXEditoru
        var name = Assembly.GetEntryAssembly()!.GetName().Name;
        Registry.CurrentUser.DeleteSubKeyTree($@"SOFTWARE\{name}", throwOnMissingSubKey: false);

        var context = new EditorContext(AppInit.Initialization<GVXEditorConfig, GVXEditorStyle>());
        DateLimit.Loc = DateLimit.Locale.Cz;

        // harness nebeží v Application.Run: modálne okno by pri skončení svojej slučky odinštalovalo synchronizačný kontext
        WindowsFormsSynchronizationContext.AutoInstall = false;
        SynchronizationContext.SetSynchronizationContext(new WindowsFormsSynchronizationContext());
        Control.CheckForIllegalCrossThreadCalls = true;

        var culture = CultureInfo.CreateSpecificCulture("sk");
        Thread.CurrentThread.CurrentUICulture = culture;
        Thread.CurrentThread.CurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        return context;
    }

    private static void SetTheme(EditorContext context, string theme)
    {
        // zabudovaná schéma harnessu (svetlá alebo tmavá), zvolená aj na stránke Vzhľad
        var styles = context.Session.Styles;
        var style = styles[theme == "dark" ? StyleNames.Dark : StyleNames.Light];
        styles.Active = style.Name;
        styles.FollowSystem = false;
        context.Session.UsingStyle = style;
        AppInit.MsgBoxStyleInit(style, context.Config);
    }

    /// <summary>
    /// Otvorí hlavné okno s inštaláciou rovnako ako Súbor → Nedávne inštalácie.
    /// </summary>
    public static FMain OpenMain(EditorContext context, string installation)
    {
        var main = new FMain(context)
        {
            StartPosition = FormStartPosition.Manual,
            Location = new Point(40, 40)
        };
        main.Show();
        Pump.Events();

        // okno sa otvára maximalizované - na snímke by malo šírku celej obrazovky
        main.WindowState = FormWindowState.Normal;
        main.Location = new Point(40, 40);
        main.Size = new Size(1240, 720);
        Pump.Events();

        typeof(FMain).GetMethod("Open", Any, [typeof(string)])!.Invoke(main, [installation]);
        Pump.Events();
        return main;
    }

    internal sealed record Options(string OutDir, string WorkDir, string? Only, string[] Themes, int TimeoutMinutes)
    {
        public static Options Parse(string[] args)
        {
            var values = new Dictionary<string, string>();
            foreach (var arg in args)
            {
                var eq = arg.IndexOf('=');
                if (!arg.StartsWith("--", StringComparison.Ordinal) || eq < 0)
                    throw new ArgumentException($"Neznámy argument '{arg}' – hodnoty sa zadávajú ako --názov=hodnota (napr. --only=hlavne-okno).");
                values[arg[..eq]] = arg[(eq + 1)..];
            }

            string? Arg(string name) => values.GetValueOrDefault(name);

            var theme = Arg("--theme") ?? "both";
            return new Options(
                Arg("--out") ?? DefaultOutDir(),
                Arg("--work") ?? @"C:\INISS2",
                Arg("--only"),
                theme == "both" ? ["light", "dark"] : [theme],
                int.Parse(Arg("--timeout") ?? "5", CultureInfo.InvariantCulture));
        }

        private static string DefaultOutDir()
        {
            // hľadá iniss-tools-docs v niektorom nadradenom priečinku (INISSTools\iniss-tools-docs)
            for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
            {
                var docs = Path.Combine(dir.FullName, "iniss-tools-docs");
                if (Directory.Exists(docs))
                    return Path.Combine(docs, "static", "img", "gvxeditor");
            }

            throw new DirectoryNotFoundException("Nenašiel sa priečinok iniss-tools-docs – zadaj --out=….");
        }
    }
}
