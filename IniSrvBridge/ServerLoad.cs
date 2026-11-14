using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace IniSrvBridge;

/// <summary>
/// Nacitanie dat kodom servera v poradi ako pri jeho starte (modGlob.MainProc): kazdy subor jeho citacom, preklad
/// stavoveho diagramu, LogText.vb, TabTab a podmienok tabul. Vynechava vsetko, co server pri starte spusta alebo kam
/// zapisuje mimo kopie dat: registraciu, mutex, PPV, zalohu, AGV, siet, vlakna, svetla a inicializaciu zvuku a liniek.
/// </summary>
internal sealed class ServerLoad
{
    /// <summary>Program servera v priecinku instalacie.</summary>
    public const string ExecutableName = "IniSrv.exe";

    private const BindingFlags AnyStatic = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
    private const BindingFlags AnyInstance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private static readonly string[] AssemblyExtensions = [".dll", ".exe"];

    private const char LineBreak = '\n';

    private static readonly Regex LogEntryStart = new(@"^\d{2}\.\d{2}\.\d{4}\s+\d{2}:\d{2}:\d{2}\.\d{3}\s?", RegexOptions.CultureInvariant);
    private static readonly Regex InterpreterError = new(@"Chyba (\S+) na \S+ (\d+) v souboru .*? : (.*)$", RegexOptions.CultureInvariant);

    private readonly string _serverDir;
    private readonly string _root;
    private readonly string _rawBank;
    private readonly bool _codeOnly;
    private readonly BridgeResult _result = new();
    private Assembly _iniSrv = null!;
    private Type _glob = null!;

    /// <param name="serverDir">priecinok servera instalacie s IniSrv.exe (len sa cita)</param>
    /// <param name="root">docasny priecinok servera s kopiou Data</param>
    /// <param name="rawBank">banka zvukov (len sa cita)</param>
    /// <param name="codeOnly">len preklad kodu: chyba prekladu nezastavi dalsi preklad, podmienky tabul bez kontroly konfiguracie,
    /// kroky za Tables.xml sa vynechaju</param>
    public ServerLoad(string serverDir, string root, string rawBank, bool codeOnly)
    {
        _serverDir = serverDir;
        _root = root;
        _rawBank = rawBank;
        _codeOnly = codeOnly;
    }

    private string DataDir => Path.Combine(_root, "Data");

    /// <summary>
    /// Nacita data kodom servera. Prva chyba, pre ktoru by sa server nespustil, nacitanie ukonci.
    /// </summary>
    public BridgeResult Run()
    {
        var watch = Stopwatch.StartNew();
        try
        {
            Prepare();
            Load();
        }
        catch (StageFailedException)
        {
            // chyba je zapisana vo vysledku
        }
        finally
        {
            _result.Log.AddRange(ReadLog(Path.Combine(_root, "LOGTMP", "Error.log")));
            _result.Milliseconds = watch.ElapsedMilliseconds;
        }

        return _result;
    }

    // cesty servera: kod servera berie priecinky z Application a modGlob, preklad pise do aktualneho priecinka
    private void Prepare()
    {
        Stage("IniSrv.exe", () =>
        {
            // prelozeny kod (LogText.dll, vyrazy) odkazuje IniSrv menom - zostava je uz nacitana z priecinka servera
            AppDomain.CurrentDomain.AssemblyResolve += (_, args) =>
            {
                var name = new AssemblyName(args.Name).Name;
                if (_iniSrv is not null && string.Equals(name, _iniSrv.GetName().Name, StringComparison.OrdinalIgnoreCase))
                    return _iniSrv;
                foreach (var extension in AssemblyExtensions)
                {
                    var file = Path.Combine(_serverDir, name + extension);
                    if (File.Exists(file))
                        return Assembly.LoadFrom(file);
                }

                return null;
            };

            Environment.CurrentDirectory = _root;
            var application = typeof(System.Windows.Forms.Application);
            // LogText.dll vznika v StartupPath, preklad odkazuje ExecutablePath (typy servera)
            application.GetField("startupPath", AnyStatic)!.SetValue(null, _root);
            application.GetField("executablePath", AnyStatic)!.SetValue(null, Path.Combine(_serverDir, ExecutableName));

            _iniSrv = Assembly.LoadFrom(Path.Combine(_serverDir, ExecutableName));
            _glob = Type("modGlob");
            var log = Type("Log");
            log.GetField("msLogPath", AnyStatic)!.SetValue(null, Path.Combine(_root, "LOG"));
            log.GetField("msTmpLogPath", AnyStatic)!.SetValue(null, Path.Combine(_root, "LOGTMP"));

            SetGlob("sconstStartupPath", _root);
            SetGlob("constsDataDirFullPathName", DataDir);
            SetGlob("constsRawBankDirFullPathName", _rawBank);
            SetGlob("constsAGVFullPathName", Path.Combine(_root, (string)Glob("constsFnameAGV")!));
        });
    }

    private void Load()
    {
        var data = DataDir;
        string Name(string field) => (string)Glob(field)!;
        string DataFile(string field) => Path.Combine(data, Name(field));

        Stage("Config.xml", () => SetGlob("goConfig", Static("Config", "ReadData", DataFile("constsConfigFName"))));
        var config = Glob("goConfig") ?? throw Fail("Config.xml", "Config.xml chýba alebo sa nedá načítať.");

        Stage("CommLines.xml", () => SetGlob("goCommLines", Static("CommLines", "ReadData", DataFile("constsCommLinesFName"))));
        Stage("zpozdeni.xml", () =>
        {
            var delays = Static("ValidDelays", "ReadData", DataFile("constsValidDelaysFName"));
            SetGlob("goValidDelays", delays);
            Call(delays, "AddZeroDelay");
        });
        Stage("DirList.xml", () => SetGlob("goDirList", Static("DirList", "ReadData", data, DataFile("constsDirListFName"))));
        var dirList = Glob("goDirList");
        Stage("GV.stanice.XML", () => SetGlob("goStations", Static("Stations", "ReadData", dirList, data, Name("constsStationsFName"))));
        Stage("Default.Stations.xml", () => SetGlob("gaoDefaultStations", Static("DefaultStations", "ReadData", data, Name("constsDefaultStationsFName"))));
        Stage("statedgm.xml", () => SetGlob("goStateDgmList", Static("StateDgmList", "ReadData", dirList, data, Name("constsStateDgmFname"))));
        Stage("PigCols.xml", () => SetGlob("goPigCols", Static("PigCols", "ReadData", DataFile("constPigColsFName"))));
        Compile("statedgm.xml", () => Call(Glob("goStateDgmList"), "Compile", false, Name("constsStateDgmFname")));

        Stage("GV.XML", () =>
        {
            var gv = Static("GV", "ReadData", data, Name("constsGVFname"));
            SetGlob("goGV", gv);
            if (gv is null)
                return;
            Call(gv, "KontrolaZajmStanic");
            Call(gv, "ExpandDateRem");
            Call(gv, "ReadMOS", data);
        });

        // zvuk sa len nacita, zariadenia sa neinicializuju; LogText.vb a ReportKeys.xml server cita len so zvukom
        Stage("FyzBank.xml", () => SetGlob("goFyzBank", Static("FyzBank", "ReadDataFromSubdirectories", _rawBank)));
        Stage("Audio.xml", () =>
        {
            var audio = Static("Audio", "ReadData", DataFile("constsAudioFname"));
            if (audio is null && Glob("goFyzBank") is not null)
                audio = Static("Audio", "CreateDefaultAudio");
            SetGlob("goAudio", audio);
        });
        var hasAudio = (bool)_glob.GetProperty("HasAudio", AnyStatic)!.GetValue(null)!;
        if (hasAudio)
            Stage("ReportKeys.xml", () => SetGlob("goReportKeys", Static("ReportKeys", "ReadData", dirList, data, Name("constsReportKeysFname"))));

        Stage("TrTypes.xml", () => SetGlob("goTrTypes", Static("TrTypes", "ReadData", DataFile("constsTrTypesFName"))));
        Stage("PositionsDef.xml", () =>
            SetGlob("goPositionsDef", Static("PositionsDef", "ReadData", dirList, data, Name("constsPositionsDefFName"), "Pozice_A.txt")));
        Stage("Positions.xml", () => Expand(SetGlob("goPositions", Static("Positions", "ReadData", dirList, data, Name("constsPositionsFName"), "Pozice.txt")), true));
        Stage("Foreign.xml", () => Expand(SetGlob("goForeign", Static("Foreign", "ReadData", dirList, data, Name("constsForeignFName"))), true));

        if (hasAudio)
        {
            var interpreters = Activator.CreateInstance(Type("Interpreters"), true);
            SetGlob("goInterpreters", interpreters);
            Stage("LogText.vb", () =>
            {
                if (!(bool)Call(interpreters, "Init", data, Name("constsLogIntrprtVBFileName"), false)!)
                {
                    AddInterpreterErrors();
                    if (_codeOnly)
                        return;
                    throw new InvalidOperationException("Chyba prekladu LogText.vb.");
                }

                Static("modGlob", "LoadVzoryDodatkuOdklonuVyluk");
            });
        }

        Stage("Razeni.xml", () => Call(SetGlob("goRazeni", Static("Razeni", "ReadData", data, Name("constsRazeniFname"), "Razeni1.txt")), "Kontrola"));
        Stage("UzivDefRazeni", () => Call(SetGlob("goUzivDefRazeni", Static("Razeni", "ReadData", data, Name("constsUzivDefRazeniFname"))), "Kontrola"));
        Stage("ConstReports.xml", () => SetGlob("goConstReports", Static("ConstReports", "ReadData", DataFile("constsConstReports"))));
        Stage("Dodatky.xml", () => Expand(SetGlob("goDodatky", Static("Dodatky", "ReadData", data, Name("constsDodatkyFname"))), true));
        Stage("Vyluky.xml", () => Expand(SetGlob("goVyluky", Static("Vyluky", "ReadData", data, Name("constsVylukyFname"))), null));
        Stage("Odklony.xml", () => Expand(SetGlob("goOdklony", Static("Odklony", "ReadData", data, Name("constsOdklonyFname"))), null));
        Stage("StaHlasVl.xml", () => Expand(SetGlob("goStaHlasVl", Static("StaHlasVl", "ReadData", data, Name("constsStaHlasVlFname"))), true));
        Stage("StaHlasB.xml", () => SetGlob("goStaHlasB", Static("StaHlasList", "ReadData", data, Name("constsStaHlasBFname"))));
        Stage("StaHlasC.xml", () => SetGlob("goStaHlasC", Static("StaHlasList", "ReadData", data, Name("constsStaHlasCFname"))));
        Stage("TTTrasy.xml", () => SetGlob("goTabTextsTrc", Static("TabTextsTrc", "ReadData", data, Name("constsTabTextsTrc"))));
        Stage("TTStanice.xml", () => SetGlob("goTabTextsSt", Static("TabTextsSt", "ReadData", data, Name("constsTabTextsSt"))));
        Stage("TCatalog.xml", () => SetGlob("goCatalog", Static("Catalog", "ReadData", data, Name("constsCatalogFName"))));

        if (Glob("goTabTab") is { } tabTab)
            Compile("TabTab", () => Call(tabTab, "Compile", false, Field(config, "bDbgLogDuplicateTabTab")));

        Stage("Tables.xml", () => SetGlob("goTables", Static("Tables", "ReadData", data, Name("constsTablesFName"))));

        // pri samotnom preklade sa podmienky tabul prekladaju bez kontroly konfiguracie (Init) a dalsie subory sa nectu
        if (_codeOnly)
        {
            if (Glob("goTables") is { } codeTables)
                Compile("Tables.xml", () => Call(codeTables, "Compile", false));
            return;
        }

        if (Glob("goTables") is { } tables)
        {
            Stage("Tables.xml", () =>
            {
                if (!(bool)Call(tables, "Init")!)
                    throw new InvalidOperationException("Byly detekovány chyby v konfiguraci tabulí.");
            });
            Compile("Tables.xml", () => Call(tables, "Compile", false));
        }

        Stage("TTVlaky.xml", () => Expand(SetGlob("goTabTexts", Static("TabTexts", "ReadDataAll", data, Name("constsTabTextsVlak"))), true));
        Stage("TableFonts", () => SetGlob("goTableFonts", Static("TableFonts", "ReadData", dirList, data, Name("constTableFontsFName"))));
        Stage("RouteSections.xml", () => Call(SetGlob("goRouteSections", Static("RouteSections", "ReadData", DataFile("constsRouteSectionsFName"))), "InitData"));
    }

    // krok nacitania: vynimka znamena, ze by sa server nespustil
    private void Stage(string name, Action action)
    {
        if (!_result.Stages.Contains(name))
            _result.Stages.Add(name);
        try
        {
            action();
        }
        catch (StageFailedException)
        {
            throw;
        }
        catch (Exception e)
        {
            var inner = e is TargetInvocationException { InnerException: { } cause } ? cause : e;
            // server casto zapise pricinu do Error.log a hodi ApplicationException bez textu
            // predvoleny text vynimky (v kazdom jazyku) obsahuje nazov typu
            var message = inner is ApplicationException && inner.Message.Contains(typeof(ApplicationException).FullName!)
                ? ReadLog(Path.Combine(_root, "LOGTMP", "Error.log")).LastOrDefault()?.Split(LineBreak)[0] ?? inner.Message
                : inner.Message;
            throw Fail(name, message);
        }
    }

    private StageFailedException Fail(string stage, string message)
    {
        _result.Fatal = message;
        _result.Stage = stage;
        return new StageFailedException();
    }

    // preklad kodu: server zastavi aj varovanie kompilatora
    private void Compile(string source, Func<object?> compile)
    {
        object? errors = null;
        Stage(source, () => errors = compile());
        if (errors is not IEnumerable list)
            return;

        foreach (var error in list)
        {
            _result.CompileErrors.Add(new CompileError(source, (int)Field(error, "iLine")!, (int)Field(error, "iColumn")!, Field(error, "sErrNum") as string ?? "",
                Field(error, "sErrMsg") as string ?? "", Field(error, "sDescr") as string ?? "", Field(error, "sCode") as string ?? ""));
        }

        if (_codeOnly)
            return;
        throw Fail(source, "Chyba prekladu " + source + ".");
    }

    // Interpreters.Init zapise chyby prekladu LogText.vb do Error.log riadkami "Chyba BCxxxxx na radku N v souboru ...: text"
    private void AddInterpreterErrors()
    {
        foreach (var line in ReadLog(Path.Combine(_root, "LOGTMP", "Error.log")).SelectMany(entry => entry.Split('\n')))
        {
            var match = InterpreterError.Match(line.TrimEnd('\r'));
            if (match.Success)
            {
                _result.CompileErrors.Add(new CompileError("LogText.vb", int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture), 0, match.Groups[1].Value,
                    match.Groups[3].Value, "", ""));
            }
        }
    }

    // ExpandDateRem s parametrom bLog (bez neho pri vylukach a odklonoch)
    private static void Expand(object? target, bool? log)
    {
        if (target is null)
            return;
        if (log is { } value)
            Call(target, "ExpandDateRem", value);
        else
            Call(target, "ExpandDateRem");
    }

    private Type Type(string name) =>
        _iniSrv.GetType("IniSrv." + name) ?? throw new InvalidOperationException("IniSrv.exe nemá typ " + name + ".");

    private object? Glob(string field) =>
        (_glob.GetField(field, AnyStatic) ?? throw new InvalidOperationException("IniSrv.exe nemá pole modGlob." + field + ".")).GetValue(null);

    private object? SetGlob(string field, object? value)
    {
        (_glob.GetField(field, AnyStatic) ?? throw new InvalidOperationException("IniSrv.exe nemá pole modGlob." + field + ".")).SetValue(null, value);
        return value;
    }

    private static object? Field(object? target, string name) =>
        target?.GetType().GetField(name, AnyInstance)?.GetValue(target);

    private object? Static(string type, string method, params object?[] args) => Invoke(Type(type), null, method, args);

    private static object? Call(object? target, string method, params object?[] args) =>
        target is null ? null : Invoke(target.GetType(), target, method, args);

    // metoda podla nazvu a poctu parametrov; volitelne parametre na konci dostanu predvolenu hodnotu
    private static object? Invoke(Type type, object? target, string method, object?[] args)
    {
        var flags = target is null ? AnyStatic : AnyInstance;
        var candidate = type.GetMethods(flags)
            .Where(m => m.Name == method)
            .Select(m => (Method: m, Parameters: m.GetParameters()))
            .Where(m => m.Parameters.Length >= args.Length && m.Parameters.Skip(args.Length).All(p => p.IsOptional) &&
                        m.Parameters.Take(args.Length).Zip(args, Accepts).All(ok => ok))
            .OrderBy(m => m.Parameters.Length)
            .FirstOrDefault();
        if (candidate.Method is null)
            throw new InvalidOperationException("IniSrv.exe nemá metódu " + type.Name + "." + method + ".");

        var values = args.Concat(candidate.Parameters.Skip(args.Length).Select(p => p.DefaultValue)).ToArray();
        return candidate.Method.Invoke(target, values);
    }

    // parameter prijme hodnotu: null len typ odkazom, inak typ hodnoty
    private static bool Accepts(ParameterInfo parameter, object? value)
    {
        var type = parameter.ParameterType.IsByRef ? parameter.ParameterType.GetElementType()! : parameter.ParameterType;
        return value is null ? !type.IsValueType || Nullable.GetUnderlyingType(type) is not null : type.IsInstanceOfType(value);
    }

    // zaznamy logu servera: kazdy zacina datumom a casom, sprava moze mat viac riadkov
    private static List<string> ReadLog(string file)
    {
        var entries = new List<string>();
        if (!File.Exists(file))
            return entries;

        StringBuilder? current = null;
        foreach (var line in File.ReadAllLines(file, Encoding.GetEncoding(1250)))
        {
            var start = LogEntryStart.Match(line);
            if (start.Success)
            {
                if (current is not null)
                    entries.Add(current.ToString().TrimEnd());
                current = new StringBuilder(line.Substring(start.Length));
            }
            else if (current is not null)
            {
                current.Append('\n').Append(line);
            }
        }

        if (current is not null)
            entries.Add(current.ToString().TrimEnd());
        return entries;
    }
}
