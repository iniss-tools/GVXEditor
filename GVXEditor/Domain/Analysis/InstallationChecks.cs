using System.Globalization;
using System.Xml;
using GVXEditor.Domain.Editing;
using GVXEditor.Domain.Rules;
using GVXEditor.Domain.Stations;
using GVXEditor.Properties;
using Serilog;
using ToolsCore.Iniss2;
using ToolsCore.Iniss2.Grafikon;
using ToolsCore.Iniss2.Grafikon.Script;

namespace GVXEditor.Domain.Analysis;

/// <summary>
/// Kontrola otvorenej instalacie: chyby, pre ktore by sa server nespustil, a upozornenia na udaje, ktore server pouzije inak,
/// nez obsluha caka. Kontroluje aktualny stav vratane neulozenych zmien.
/// </summary>
internal static class InstallationChecks
{
    // najdlhsie obdobie, v ktorom sa hladaju dva vlaky s rovnakym cislom v rovnaky den (vlak bez konca ma obdobie do roku 9999)
    private const int MaxScannedDays = 800;

    private static readonly string[] RequiredDataFiles = ["zpozdeni.xml", "PigCols.xml", TrTypesFile.FileName];

    /// <summary>
    /// Zistenia instalacie v poradi: cela instalacia, potom grafikony.
    /// </summary>
    public static List<Finding> Run(Iniss2Workspace workspace)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        var findings = new List<Finding>();
        var dataDir = workspace.DataDir;

        foreach (var file in RequiredDataFiles.Where(file => !File.Exists(Path.Combine(dataDir, file))))
            findings.Add(new Finding(FindingSeverity.Error, -1, -1, Resources.Place_Data, Text(Resources.Check_MissingFile, file)));
        if (!workspace.Groups.Any(group => File.Exists(Path.Combine(dataDir, group.Group.Name, GvStationsFile.FileName))))
            findings.Add(new Finding(FindingSeverity.Error, -1, -1, Resources.Place_Data, Resources.Check_NoStationNames));

        var config = Read(() => Iniss2Config.Read(dataDir), new Iniss2Config(0), dataDir);
        if (config.PpvDataDirectory.Length > 0)
            findings.Add(new Finding(FindingSeverity.Warning, -1, -1, Resources.Place_Data, Text(Resources.Check_Ppv, config.PpvDataDirectory)));
        if (config.DataBackupPath.Length > 0)
            findings.Add(new Finding(FindingSeverity.Info, -1, -1, Resources.Place_Data, Text(Resources.Check_Backup, config.DataBackupPath)));

        var trainTypes = Read(() => TrTypesFile.ReadKeys(dataDir), null, dataDir)?.ToHashSet(StringComparer.Ordinal);
        var keys = new KeyCatalog(workspace.Functions, workspace.FunctionDescriptions, workspace.ConstReports);
        var agvFresh = IsAgvFresh(workspace, config);

        CheckScript(findings, workspace);
        for (var g = 0; g < workspace.Groups.Count; g++)
        {
            var loaded = workspace.LoadedState.Groups[g].Timetable;
            CheckGroup(findings, workspace, g, keys, trainTypes, agvFresh ? loaded : null);
        }

        return findings;
    }

    // hlasenia, ktorych pritomnost klient zistuje podla nazvu (nemusia mat odkaz z dat)
    private static readonly HashSet<string> SpecialReports = new(
        ["Znělka", "Znělka_mezi", "Znělka_koncová", "Přípoje", "NPripoj", "Přestup", "Urychlený_přestup"], StringComparer.OrdinalIgnoreCase);

    // odkazy z dat na funkcie skriptu hlaseni (typy hlaseni, stavy, hlasenia obsluhy) a zakladne hlasenia bez odkazu;
    // kluce dodatkov, vyluk a odklonov kontroluju zaznamy grafikonu
    private static void CheckScript(List<Finding> findings, Iniss2Workspace workspace)
    {
        if (workspace.Functions is not { } functions)
            return;

        foreach (var report in workspace.ConstReports.Where(report => report.Function.Trim().Length > 0 && !functions.ContainsKey(report.Function.Trim())))
        {
            findings.Add(new Finding(FindingSeverity.Warning, -1, -1, Resources.Place_Data,
                Text(Resources.Check_ConstReportMissing, report.Key, report.Name, report.Function)));
        }

        var started = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        for (var g = 0; g < workspace.Groups.Count; g++)
        {
            var group = workspace.Groups[g];
            foreach (var station in group.Stations)
            {
                var where = Text(Resources.Place_Station, Text(Resources.Place_Group, group.Group.Name), station.Station.Directory);
                foreach (var key in station.Setup?.ReportKeys ?? [])
                {
                    started.Add(key.Name.Trim());
                    if (key.Name.Trim().Length > 0 && !functions.ContainsKey(key.Name.Trim()))
                        findings.Add(new Finding(FindingSeverity.Warning, g, -1, where, Text(Resources.Check_ReportKeyMissing, key.Name)));
                }

                foreach (var node in station.Code?.Diagram?.Graphs.SelectMany(graph => graph.Nodes) ?? [])
                {
                    started.Add(node.Report.Trim());
                    if (node.Report.Trim().Length > 0 && !functions.ContainsKey(node.Report.Trim()))
                        findings.Add(new Finding(FindingSeverity.Warning, g, -1, where, Text(Resources.Check_StateReportMissing, node.Key, node.Report)));
                }
            }
        }

        foreach (var (name, attributes) in functions)
        {
            if (!started.Contains(name) && !SpecialReports.Contains(name)
                && attributes.Any(attribute => ScriptReportKinds.TryParse(attribute, out var kind)
                                               && kind == ScriptReportKind.Basic))
                findings.Add(new Finding(FindingSeverity.Info, -1, -1, Resources.Place_Data, Text(Resources.Check_UnusedBasicReport, name)));
        }
    }

    private static T Read<T>(Func<T> read, T fallback, string dataDir)
    {
        try
        {
            return read();
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException or XmlException or FormatException)
        {
            Log.Warning(e, "Kontrola instalacie {DataDir}: subor sa nepodarilo nacitat", dataDir);
            return fallback;
        }
    }

    // server cerstvy AGV.xml pri starte nacita a vlaky s inou trasou nez v grafikone zahodi vsetky
    private static bool IsAgvFresh(Iniss2Workspace workspace, Iniss2Config config)
    {
        var agv = new FileInfo(Path.Combine(workspace.ServerDir, "AGV.xml"));
        return !config.NoLoadAgv && agv.Exists && DateTime.UtcNow - agv.LastWriteTimeUtc < TimeSpan.FromMinutes(config.AgvPreTime);
    }

    private static void CheckGroup(List<Finding> findings, Iniss2Workspace workspace, int g, KeyCatalog keys, HashSet<string>? trainTypes,
        GvFile? agvTimetable)
    {
        var group = workspace.Groups[g];
        var place = Text(Resources.Place_Group, group.Group.Name);
        void Add(FindingSeverity severity, int train, string where, string message) => findings.Add(new Finding(severity, g, train, where, message));

        if (group.Error is { } error)
            Add(FindingSeverity.Error, -1, place, Text(Resources.Check_GroupFailed, error));
        if (group.NoticesError is { } noticesError)
            Add(FindingSeverity.Error, -1, place, Text(Resources.Check_NoticesFailed, noticesError));

        var groupDir = Path.Combine(workspace.DataDir, group.Group.Name);
        foreach (var station in group.Stations)
        {
            var where = Text(Resources.Place_Station, place, station.Station.Directory);
            var stationDir = Path.Combine(groupDir, station.Station.Directory);
            if (station.Error is { } stationError)
                Add(FindingSeverity.Error, -1, where, Text(Resources.Check_StationFailed, stationError));
            foreach (var file in new[] { StateDiagramFile.FileName, DirListXml.StationNumberFile }.Where(file => !File.Exists(Path.Combine(stationDir, file))))
                Add(FindingSeverity.Error, -1, where, Text(Resources.Check_MissingFile, file));
            if (!File.Exists(Path.Combine(stationDir, ReportKeysFile.FileName)))
                Add(FindingSeverity.Warning, -1, where, Resources.Check_MissingReportKeys);
        }

        if (group.Timetable is not { } timetable)
            return;

        CheckTrains(group, timetable, workspace, trainTypes, agvTimetable, Add, place);

        var mos = group.Stations.Where(station => station.Files is not null)
            .GroupBy(station => station.Station.Station)
            .ToDictionary(stations => stations.Key, stations => stations.First().Files!.Mos.ToList());
        var server = timetable.ForServer(group.InterestStations, mos);

        foreach (var station in group.Stations)
        {
            var where = Text(Resources.Place_Station, place, station.Station.Directory);
            if (station.Files is { } files)
                CheckStation(station, files, server.TrainRanges(station.Station.Station), keys, (severity, message) => Add(severity, -1, where, message));
            CheckSameNumber(server, station.Station.Station, (severity, message) => Add(severity, -1, where, message));
        }

        CheckNotices(group, timetable, keys, (severity, message) => Add(severity, -1, place, message));
    }

    private static void CheckTrains(GroupDocument group, GvFile timetable, Iniss2Workspace workspace, HashSet<string>? trainTypes, GvFile? agvTimetable,
        Action<FindingSeverity, int, string, string> add, string place)
    {
        var interest = group.InterestStations;
        var unnamed = new SortedSet<int>();
        var unknownTypes = new Dictionary<string, List<int>>(StringComparer.Ordinal);
        var loadedRoutes = agvTimetable?.Trains.Select(RouteOf).ToHashSet(StringComparer.Ordinal);

        for (var i = 0; i < timetable.Trains.Count; i++)
        {
            var train = timetable.Trains[i];
            var where = Text(Resources.Place_Train, place, TrainLabel(train));

            // pravidla okna Upravit vlak: chyba okna (napr. vlak s jednym bodom) server neodmietne, je to upozornenie
            foreach (var problem in TrainRules.Check(TrainDraft.Of(train), interest, workspace.StationName))
                add(FindingSeverity.Warning, i, where, problem.Message);

            var stops = train.Variants.SelectMany(variant => variant.Stops).ToList();
            if (trainTypes is not null)
            {
                foreach (var type in stops.Select(stop => stop.TrainType).Where(type => type.Length > 0 && type != MosFile.MotorType)
                             .Distinct(StringComparer.Ordinal).Where(type => !trainTypes.Contains(type)))
                {
                    if (!unknownTypes.TryGetValue(type, out var trains))
                        unknownTypes[type] = trains = [];
                    trains.Add(i);
                }
            }

            foreach (var stop in stops.Where(stop => !workspace.StationNames.ContainsKey(stop.Station)))
                unnamed.Add(stop.Station);

            if (loadedRoutes is not null && !loadedRoutes.Contains(RouteOf(train)))
                add(FindingSeverity.Warning, i, where, Text(Resources.Check_AgvChanged, TrainLabel(train)));
        }

        // jedno zistenie za druh a za vsetky stanice bez nazvu - v datach ich byva vela
        foreach (var (type, trains) in unknownTypes)
        {
            add(FindingSeverity.Warning, trains[0], Text(Resources.Place_Train, place, TrainLabel(timetable.Trains[trains[0]])),
                Text(Resources.Check_TrainType, type, trains.Count, TrainLabel(timetable.Trains[trains[0]])));
        }

        if (unnamed.Count > 0)
        {
            add(FindingSeverity.Warning, -1, place,
                Text(Resources.Check_StationName, unnamed.Count, string.Join(", ", unnamed.Select(station => station.ToString(CultureInfo.InvariantCulture)))));
        }
    }

    // podla tychto udajov server paruje vlaky z AGV.xml s grafikonom
    private static string RouteOf(GvTrain train) => string.Join("|", train.Variants.Select(variant =>
        string.Join(",", variant.Stops.Select(stop => string.Create(CultureInfo.InvariantCulture, $"{stop.Station}{(stop.IsInterest ? "Z" : "")}")))));

    private static string TrainLabel(GvTrain train) =>
        train.Variants.SelectMany(variant => variant.Stops).Select(stop => stop.Number.Length > 0 ? stop.Number : stop.DisplayNumber)
            .FirstOrDefault(number => number.Length > 0) ?? "?";

    private static void CheckStation(StationDocument station, StationFiles files, Dictionary<string, (DateOnly From, DateOnly To)> ranges, KeyCatalog keys,
        Action<FindingSeverity, string> add)
    {
        void Records(string file, IEnumerable<(string Number, string Validity)> records)
        {
            foreach (var number in records.GroupBy(record => record.Number, StringComparer.Ordinal))
            {
                if (!ranges.TryGetValue(number.Key, out var range))
                {
                    add(FindingSeverity.Info, Text(Resources.Check_UnusedRecord, file, number.Key, number.Count()));
                    continue;
                }

                foreach (var record in number.Where(record => GvValidity.Check(range.From, range.To, record.Validity) != GvValidityProblem.None))
                    add(FindingSeverity.Warning, Text(Resources.Check_RecordValidity, file, record.Validity, number.Key));
            }
        }

        Records(Resources.File_Positions, files.Positions.Select(record => (record.Train, record.Validity)));
        Records(Resources.File_Additions, files.Additions.Select(record => (record.Number, record.Validity)));
        Records(Resources.File_Compositions, files.Compositions.Select(record => (record.Number, record.Validity)));
        Records(Resources.File_Languages, files.Languages.Select(record => (record.Number, record.Validity)));
        Records(Resources.File_ReportStations, files.ReportStations.Select(record => (record.Number, record.Validity)));
        Records(Resources.File_Texts, files.TextSets.SelectMany(set => set.Trains).Select(record => (record.Number, record.Validity)));
        Records(Resources.File_Mos, files.Mos.Select(record => (record.Number, record.Validity)));

        var tracks = station.Tracks.Select(track => track.Key).ToHashSet(StringComparer.Ordinal);
        if (tracks.Count > 0)
        {
            foreach (var record in files.Positions)
            {
                foreach (var track in new[] { record.ArrivalTrack, record.DepartureTrack }.Where(track => track.Length > 0 && !tracks.Contains(track)).Distinct())
                    add(FindingSeverity.Warning, Text(Resources.Check_RecordTrack, track, record.Train));
            }
        }

        foreach (var record in files.Additions.Where(record => !keys.IsKnown(record.Key)))
            add(FindingSeverity.Warning, Text(Resources.Check_RecordKey, Resources.File_Additions, record.Key, record.Number));
    }

    private static void CheckNotices(GroupDocument group, GvFile timetable, KeyCatalog keys, Action<FindingSeverity, string> add)
    {
        var interest = group.InterestStations;
        var lockoutNumbers = timetable.Trains.Select(train => TrainStations.LockoutNumber(train, interest)).ToHashSet(StringComparer.Ordinal);
        var stationKeys = timetable.Trains.SelectMany(train => TrainStations.Keys(train, group.Group))
            .Select(key => (key.Station.Station, key.Number)).ToHashSet();

        void Notice(string file, string number, string key, DateOnly from, DateOnly to, string validity)
        {
            if (GvValidity.Check(from, to == DateOnly.MinValue ? DateOnly.MaxValue : to, validity) != GvValidityProblem.None)
                add(FindingSeverity.Warning, Text(Resources.Check_RecordValidity, file, validity, number));
            if (!keys.IsKnown(key))
                add(FindingSeverity.Warning, Text(Resources.Check_RecordKey, file, key, number));
        }

        foreach (var lockouts in (group.Lockouts ?? []).GroupBy(record => record.Number, StringComparer.Ordinal))
        {
            if (!lockoutNumbers.Contains(lockouts.Key))
                add(FindingSeverity.Info, Text(Resources.Check_UnusedRecord, Resources.File_Lockouts, lockouts.Key, lockouts.Count()));
            else
            {
                foreach (var record in lockouts)
                    Notice(Resources.File_Lockouts, record.Number, record.Key, record.From, record.To, record.Validity);
            }
        }

        foreach (var diversions in (group.Diversions ?? []).GroupBy(record => (record.Station, record.Number)))
        {
            if (!stationKeys.Contains(diversions.Key))
                add(FindingSeverity.Info, Text(Resources.Check_UnusedRecord, Resources.File_Diversions, diversions.Key.Number, diversions.Count()));
            else
            {
                foreach (var record in diversions)
                    Notice(Resources.File_Diversions, record.Number, record.Key, record.From, record.To, record.Validity);
            }
        }
    }

    // dva rozne vlaky s rovnakym cislom pri odchode v rovnaky den v stanici - zaznamy stanice dostanu oba
    private static void CheckSameNumber(GvFile server, int station, Action<FindingSeverity, string> add)
    {
        var days = new Dictionary<string, Dictionary<DateOnly, GvTrain>>(StringComparer.Ordinal);
        var reported = new HashSet<string>(StringComparer.Ordinal);

        foreach (var train in server.Trains)
        {
            foreach (var variant in train.Variants)
            {
                var active = train.ActiveDaysOf(variant);
                for (var i = 0; i < variant.Stops.Count; i++)
                {
                    var stop = variant.Stops[i];
                    if (stop.Station != station || !stop.IsInterest || !stop.HasTime ||
                        variant.NumberSource(i, false)?.Number is not { Length: > 0 } number || reported.Contains(number))
                        continue;

                    var call = new StationCall(train, variant, i);
                    if (!days.TryGetValue(number, out var dates))
                        days[number] = dates = [];
                    for (var d = 0; d < active.Length && d < MaxScannedDays; d++)
                    {
                        if (!active[d])
                            continue;
                        var date = call.DateAtStation(train.From.AddDays(d));
                        if (dates.TryGetValue(date, out var other) && !ReferenceEquals(other, train))
                        {
                            add(FindingSeverity.Warning, Text(Resources.Check_SameNumber, number, date.ToString("d", CultureInfo.CurrentCulture)));
                            reported.Add(number);
                            break;
                        }

                        dates[date] = train;
                    }
                }
            }
        }
    }

    private static string Text(string format, params object[] args) => string.Format(CultureInfo.CurrentCulture, format, args);
}
