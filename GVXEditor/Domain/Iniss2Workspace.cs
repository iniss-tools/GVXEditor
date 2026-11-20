using System.Text;
using System.Xml;
using GVXEditor.Domain.Boards;
using GVXEditor.Domain.Code;
using GVXEditor.Domain.Setup;
using GVXEditor.Domain.Stations;
using Serilog;
using ToolsCore.Iniss.Tools;
using ToolsCore.Iniss2;
using ToolsCore.Iniss2.Grafikon;
using ToolsCore.Iniss2.Tables;

namespace GVXEditor.Domain;

/// <summary>
/// Otvorena instalacia INISS2: grafikony z DirList.xml, ich vlaky zo suboru GV.XML tak, ako su v subore, subory stanic,
/// vyluky a odklony a nazvy stanic. Zmeny su kroky historie (<see cref="History" />), ulozenie zapise zmenene subory naraz.
/// </summary>
internal sealed class Iniss2Workspace
{
    // stav suborov pri nacitani a poslednom ulozeni podla cesty - nie je sucastou historie
    private readonly Dictionary<string, FileStamp> _stamps;
    private WorkspaceState _saved;

    // nazvy stanic pre stav _namesState
    private WorkspaceState? _namesState;
    private IReadOnlyDictionary<int, string> _names = new Dictionary<int, string>();

    private Iniss2Workspace(string dataDir, WorkspaceState state, Dictionary<string, FileStamp> stamps)
    {
        DataDir = dataDir;
        History = new UndoHistory<WorkspaceState>(state);
        LoadedState = state;
        _saved = state;
        _stamps = stamps;
    }

    // funkcie a popisy LogText.vb pre text _functionsText
    private string? _functionsText;
    private IReadOnlyDictionary<string, HashSet<string>>? _functions;
    private IReadOnlyDictionary<string, string>? _functionDescriptions;

    /// <summary>Funkcie LogText.vb a oznacenia nad nimi (aktualny text); <see langword="null" />, ak subor chyba.</summary>
    public IReadOnlyDictionary<string, HashSet<string>>? Functions
    {
        get
        {
            UpdateFunctions();
            return _functions;
        }
    }

    /// <summary>Popisy funkcii LogText.vb z ich oznaceni (aktualny text); <see langword="null" />, ak subor chyba.</summary>
    public IReadOnlyDictionary<string, string>? FunctionDescriptions
    {
        get
        {
            UpdateFunctions();
            return _functionDescriptions;
        }
    }

    private void UpdateFunctions()
    {
        var text = History.Current.LogText;
        if (ReferenceEquals(text, _functionsText) && (_functions is not null || text is null))
            return;

        _functionsText = text;
        var lines = text?.Split('\n').Select(line => line.TrimEnd('\r')).ToList();
        _functions = lines is null ? null : LogTextFile.Functions(lines);
        _functionDescriptions = lines is null ? null : LogTextFile.Descriptions(lines);
    }

    /// <summary>Hlasenia obsluhy z ConstReports.xml (aktualny stav).</summary>
    public IReadOnlyList<ConstReport> ConstReports => History.Current.ConstReports ?? [];

    /// <summary>Priecinok dat servera.</summary>
    public string DataDir { get; }

    /// <summary>Komunikacne linky z CommLines.xml (len na citanie - pre vyber linky tabule).</summary>
    public IReadOnlyList<CommLine> Lines { get; private init; } = [];

    /// <summary>Priecinok servera - server cita vzdy priecinok Data vedla svojho programu.</summary>
    public string ServerDir => Path.GetDirectoryName(DataDir) ?? DataDir;

    private SoundBank? _bank;

    /// <summary>Zvukova banka instalacie (RawBank vedla servera), cita sa az pri prvom pouziti.</summary>
    public SoundBank Bank => _bank ??= new SoundBank(Path.Combine(ServerDir, SoundBank.DirectoryName));

    private IReadOnlyList<TrType>? _trainTypes;

    /// <summary>Druhy vlakov z TrTypes.xml (kluc, zvuk, text na tabuli); prazdne, ak subor chyba alebo sa neda nacitat.</summary>
    public IReadOnlyList<TrType> TrainTypeDefinitions => _trainTypes ??= ReadTrainTypes();

    /// <summary>Kluce druhov vlakov z TrTypes.xml na vyber v poradi suboru.</summary>
    public IReadOnlyList<string> TrainTypes => [.. TrainTypeDefinitions.Select(type => type.Key)];

    private List<TrType> ReadTrainTypes()
    {
        try
        {
            return TrTypesFile.Read(DataDir) ?? [];
        }
        catch (Exception e) when (IsLoadError(e))
        {
            Log.Warning(e, "Druhy vlakov instalacie {DataDir} sa nepodarilo nacitat", DataDir);
            return [];
        }
    }

    /// <summary>Stav pri otvoreni instalacie (napr. na porovnanie s AGV.xml servera).</summary>
    public WorkspaceState LoadedState { get; }

    /// <summary>Kroky spat a znova.</summary>
    public UndoHistory<WorkspaceState> History { get; }

    /// <summary>Grafikony v poradi DirList.xml (aktualny stav).</summary>
    public IReadOnlyList<GroupDocument> Groups => History.Current.Groups;

    /// <summary>
    /// Nazvy stanic podla cisla ako ich pouzije server: z GV.stanice.XML grafikonov v poradi DirList, plati prvy (aktualny
    /// stav vratane neulozenych zmien).
    /// </summary>
    public IReadOnlyDictionary<int, string> StationNames
    {
        get
        {
            var state = History.Current;
            if (!ReferenceEquals(state, _namesState))
            {
                var names = new Dictionary<int, string>();
                foreach (var station in state.Groups.SelectMany(group => group.Setup?.Names ?? []))
                    names.TryAdd(station.Number, station.Name);
                _names = names;
                _namesState = state;
            }

            return _names;
        }
    }

    /// <summary>Instalacia ma neulozene zmeny.</summary>
    public bool IsDirty => DirtyGroups().Count > 0 || InstallationWrites(DataDir).Any();

    /// <summary>
    /// Nazov stanice; stanica bez nazvu ma cislo, ako ju ukazuje server.
    /// </summary>
    public string StationName(int number) =>
        StationNames.TryGetValue(number, out var name) ? name : number.ToString(System.Globalization.CultureInfo.InvariantCulture);

    /// <summary>
    /// Nacita instalaciu. Grafikon, ktory sa neda nacitat, ostane v zozname s chybou.
    /// </summary>
    /// <param name="dataDir">priecinok dat servera</param>
    /// <exception cref="IOException">DirList.xml chyba alebo sa neda citat</exception>
    /// <exception cref="XmlException">DirList.xml nie je XML</exception>
    /// <exception cref="FormatException">DirList.xml ma chybny obsah</exception>
    public static Iniss2Workspace Load(string dataDir)
    {
        var groups = DirListXml.Read(dataDir);
        var stamps = new Dictionary<string, FileStamp>(StringComparer.OrdinalIgnoreCase);
        var documents = new List<GroupDocument>();
        foreach (var group in groups)
        {
            var groupDir = Path.Combine(dataDir, group.Name);
            foreach (var file in new[] { GvFile.FileName, VylukyFile.FileName, OdklonyFile.FileName, TabTabXml.GlobalFileName }.Concat(GroupSetup.FileNames))
                stamps[Path.Combine(groupDir, file)] = FileStamp.Of(Path.Combine(groupDir, file));
            foreach (var item in group.Items)
            {
                foreach (var file in StationFiles.FileNames.Concat(StationSetup.FileNames).Concat(StationCode.FileNames).Concat(StationBoards.FileNames))
                    stamps[Path.Combine(groupDir, item.Directory, file)] = FileStamp.Of(Path.Combine(groupDir, item.Directory, file));
            }

            documents.Add(LoadGroup(dataDir, group));
        }

        foreach (var file in new[] { ConstReportsFile.FileName, LogTextFile.FileName })
            stamps[Path.Combine(dataDir, file)] = FileStamp.Of(Path.Combine(dataDir, file));
        var state = new WorkspaceState(documents)
        {
            ConstReports = ReadOrDefault<IReadOnlyList<ConstReport>?>(() => ConstReportsFile.Read(dataDir), null, ConstReportsFile.FileName, dataDir),
            LogText = ReadOrDefault(() => LogTextFile.ReadText(dataDir), null, LogTextFile.FileName, dataDir)
        };
        return new Iniss2Workspace(dataDir, state, stamps)
        {
            Lines = ReadOrDefault<IReadOnlyList<CommLine>>(() => CommLinesXml.Read(dataDir), [], CommLinesXml.FileName, dataDir)
        };
    }

    private static T ReadOrDefault<T>(Func<T> read, T fallback, string what, string where)
    {
        try
        {
            return read();
        }
        catch (Exception e) when (IsLoadError(e))
        {
            Log.Warning(e, "Subor {What} v {Where} sa nepodarilo nacitat", what, where);
            return fallback;
        }
    }

    private static bool IsLoadError(Exception e) =>
        e is IOException or UnauthorizedAccessException or XmlException or FormatException or OverflowException or InvalidOperationException;

    private static GroupDocument LoadGroup(string dataDir, DirListGroup group)
    {
        var groupDir = Path.Combine(dataDir, group.Name);
        var stations = group.Items.Select(item => LoadStation(groupDir, item)).ToList();

        IReadOnlyList<TrainLockout>? lockouts = null;
        IReadOnlyList<TrainDiversion>? diversions = null;
        string? noticesError = null;
        try
        {
            lockouts = VylukyFile.Read(groupDir);
            diversions = OdklonyFile.Read(groupDir);
        }
        catch (Exception e) when (IsLoadError(e))
        {
            Log.Warning(e, "Vyluky alebo odklony grafikonu {Group} instalacie {DataDir} sa nepodarilo nacitat", group.Name, dataDir);
            noticesError = e.Message;
        }

        GroupSetup? setup = null;
        string? setupError = null;
        try
        {
            setup = GroupSetup.Load(groupDir);
        }
        catch (Exception e) when (IsLoadError(e))
        {
            Log.Warning(e, "Nastavenie grafikonu {Group} instalacie {DataDir} sa nepodarilo nacitat", group.Name, dataDir);
            setupError = e.Message;
        }

        IReadOnlyList<TabTabTable>? tabTabs = null;
        string? tabTabsError = null;
        try
        {
            tabTabs = TabTabXml.Read(Path.Combine(groupDir, TabTabXml.GlobalFileName));
        }
        catch (Exception e) when (IsLoadError(e))
        {
            Log.Warning(e, "TabTabGlobal.xml grafikonu {Group} instalacie {DataDir} sa nepodarilo nacitat", group.Name, dataDir);
            tabTabsError = e.Message;
        }

        GvFile? timetable = null;
        string? error = null;
        try
        {
            timetable = GvFile.Load(groupDir);
        }
        catch (Exception e) when (IsLoadError(e))
        {
            Log.Warning(e, "Grafikon {Group} instalacie {DataDir} sa nepodarilo nacitat", group.Name, dataDir);
            error = e.Message;
        }

        return new GroupDocument(group, timetable, error)
        {
            Stations = stations,
            Lockouts = lockouts,
            Diversions = diversions,
            NoticesError = noticesError,
            Setup = setup,
            SetupError = setupError,
            GlobalTabTabs = tabTabs,
            GlobalTabTabsError = tabTabsError
        };
    }

    private static StationDocument LoadStation(string groupDir, DirListItem item)
    {
        var stationDir = Path.Combine(groupDir, item.Directory);
        StationSetup? setup = null;
        string? setupError = null;
        try
        {
            setup = StationSetup.Load(stationDir);
        }
        catch (Exception e) when (IsLoadError(e))
        {
            Log.Warning(e, "Nastavenie stanice {Station} sa nepodarilo nacitat", stationDir);
            setupError = e.Message;
        }

        StationCode? code = null;
        string? codeError = null;
        try
        {
            code = StationCode.Load(stationDir);
        }
        catch (Exception e) when (IsLoadError(e))
        {
            Log.Warning(e, "Kod stanice {Station} sa nepodarilo nacitat", stationDir);
            codeError = e.Message;
        }

        StationBoards? boards = null;
        string? boardsError = null;
        try
        {
            boards = StationBoards.Load(stationDir);
        }
        catch (Exception e) when (IsLoadError(e))
        {
            Log.Warning(e, "Tabule stanice {Station} sa nepodarilo nacitat", stationDir);
            boardsError = e.Message;
        }

        try
        {
            return new StationDocument(item, StationFiles.Load(stationDir), setup, null)
            {
                SetupError = setupError, Code = code, CodeError = codeError, Boards = boards, BoardsError = boardsError
            };
        }
        catch (Exception e) when (IsLoadError(e))
        {
            Log.Warning(e, "Subory stanice {Station} sa nepodarilo nacitat", stationDir);
            return new StationDocument(item, null, setup, e.Message)
            {
                SetupError = setupError, Code = code, CodeError = codeError, Boards = boards, BoardsError = boardsError
            };
        }
    }

    /// <summary>
    /// Zaznamena zmenu grafikonu ako krok historie.
    /// </summary>
    /// <param name="index">poradie grafikonu</param>
    /// <param name="timetable">nove vlaky grafikonu</param>
    /// <param name="label">popis kroku pre ponuku Spat</param>
    public void Change(int index, GvFile timetable, string label) => Change(index, Groups[index] with { Timetable = timetable }, label);

    /// <summary>
    /// Zaznamena zmenu grafikonu (vlaky, subory stanic, vyluky, odklony) ako krok historie.
    /// </summary>
    /// <param name="index">poradie grafikonu</param>
    /// <param name="group">novy stav grafikonu</param>
    /// <param name="label">popis kroku pre ponuku Spat</param>
    public void Change(int index, GroupDocument group, string label) => History.Push(History.Current.With(index, group), label);

    /// <summary>
    /// Zaznamena zmenu hlaseni obsluhy ako krok historie.
    /// </summary>
    /// <param name="reports">nove hlasenia obsluhy</param>
    /// <param name="label">popis kroku pre ponuku Spat</param>
    public void ChangeConstReports(IReadOnlyList<ConstReport> reports, string label) => History.Push(History.Current with { ConstReports = reports }, label);

    /// <summary>
    /// Zaznamena zmenu LogText.vb ako krok historie.
    /// </summary>
    /// <param name="text">novy text suboru</param>
    /// <param name="label">popis kroku pre ponuku Spat</param>
    public void ChangeLogText(string text, string label) => History.Push(History.Current with { LogText = text }, label);

    /// <summary>
    /// Zaznamena zmenu viacerych casti instalacie naraz (napr. skript hlaseni s odkazmi v datach) ako jeden krok historie.
    /// </summary>
    /// <param name="state">novy stav instalacie</param>
    /// <param name="label">popis kroku</param>
    public void ChangeState(WorkspaceState state, string label) => History.Push(state, label);

    /// <summary>
    /// Poradia grafikonov, ktorych subory sa od nacitania alebo ulozenia zmenili.
    /// </summary>
    public List<int> DirtyGroups()
    {
        var dirty = new List<int>();
        for (var i = 0; i < Groups.Count; i++)
        {
            if (!ReferenceEquals(Groups[i], _saved.Groups[i]) && Writes(i).Any())
                dirty.Add(i);
        }

        return dirty;
    }

    /// <summary>
    /// Zapise neulozene zmeny do kopie priecinka dat (overenie kodom servera pred ulozenim).
    /// </summary>
    /// <param name="dataDir">kopia priecinka dat</param>
    public void WritePending(string dataDir) => WritePending(dataDir, History.Current);

    /// <summary>
    /// Zapise stav <paramref name="state" /> (napr. aktualny stav s rozpisanou upravou okna) do kopie priecinka dat.
    /// </summary>
    /// <param name="dataDir">kopia priecinka dat</param>
    /// <param name="state">stav, ktory sa zapise (rozdiel voci ulozenemu)</param>
    public void WritePending(string dataDir, WorkspaceState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        foreach (var write in InstallationWrites(dataDir, state))
            write.Write();
        for (var i = 0; i < state.Groups.Count; i++)
        {
            if (ReferenceEquals(state.Groups[i], _saved.Groups[i]))
                continue;
            foreach (var write in state.Groups[i].Changes(dataDir, _saved.Groups[i]).Where(write => write.IsNeeded))
                write.Write();
        }
    }

    /// <summary>
    /// Subory dat, ktore by ulozenie zapisalo (cesty relativne k priecinku dat).
    /// </summary>
    public List<string> UnsavedFiles() =>
        [.. PendingWrites(DataDir, History.Current).Select(write => Path.GetRelativePath(DataDir, write.Path))];

    /// <summary>
    /// Text suboru dat tak, ako by ho program teraz ulozil - s neulozenymi zmenami (zapis do docasnej kopie suboru, upravy
    /// na mieste ako pri ulozeni). Subor bez zmeny sa precita z disku.
    /// </summary>
    /// <param name="relativePath">cesta relativne k priecinku dat</param>
    /// <returns>text (null = subor nie je a ulozenie ho nevytvori) a ci ho zmeny menia</returns>
    public (string? Text, bool Changed) CurrentText(string relativePath)
    {
        var original = Path.Combine(DataDir, relativePath);
        var temp = Path.Combine(Path.GetTempPath(), "GVXEditor", "file-" + Guid.NewGuid().ToString("N"));
        try
        {
            var target = Path.GetFullPath(Path.Combine(temp, relativePath));
            var write = PendingWrites(temp, History.Current)
                .FirstOrDefault(item => string.Equals(Path.GetFullPath(item.Path), target, StringComparison.OrdinalIgnoreCase));
            if (write is null)
                return (File.Exists(original) ? ReadData(original) : null, false);

            Directory.CreateDirectory(Path.GetDirectoryName(target)!);
            if (File.Exists(original))
                File.Copy(original, target);
            write.Write();
            return (File.Exists(target) ? ReadData(target) : null, true);
        }
        finally
        {
            try
            {
                if (Directory.Exists(temp))
                    Directory.Delete(temp, true);
            }
            catch (IOException e)
            {
                Log.Warning(e, "Docasny priecinok {Directory} sa nepodarilo zmazat", temp);
            }
        }
    }

    // subory dat su CP1250, subor s BOM UTF-8 (napr. zapisany inym nastrojom) podla neho
    private static string ReadData(string file)
    {
        var bytes = File.ReadAllBytes(file);
        return bytes is [0xEF, 0xBB, 0xBF, ..] ? Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3) : Encodings.Win1250.GetString(bytes);
    }

    // zapisy, ktore by ulozenie stavu vykonalo, s cestami v priecinku dat dataDir
    private IEnumerable<FileWrite> PendingWrites(string dataDir, WorkspaceState state)
    {
        foreach (var write in InstallationWrites(dataDir, state))
            yield return write;
        for (var i = 0; i < state.Groups.Count; i++)
        {
            if (ReferenceEquals(state.Groups[i], _saved.Groups[i]))
                continue;
            foreach (var write in state.Groups[i].Changes(dataDir, _saved.Groups[i]).Where(write => write.IsNeeded))
                yield return write;
        }
    }

    private IEnumerable<FileWrite> InstallationWrites(string dataDir) => InstallationWrites(dataDir, History.Current);

    // zapisy suborov spolocnych pre instalaciu (hlasenia obsluhy, LogText.vb)
    private IEnumerable<FileWrite> InstallationWrites(string dataDir, WorkspaceState state)
    {
        if (state.ConstReports is { } reports && !ReferenceEquals(reports, _saved.ConstReports))
        {
            var write = new FileWrite(Path.Combine(dataDir, ConstReportsFile.FileName), reports.Count > 0, () => ConstReportsFile.Write(dataDir, reports));
            if (write.IsNeeded)
                yield return write;
        }

        if (state.LogText is { } text && !ReferenceEquals(text, _saved.LogText))
            yield return new FileWrite(Path.Combine(dataDir, LogTextFile.FileName), true, () => LogTextFile.WriteText(dataDir, text));
    }

    // zapisy zmenenych suborov grafikonu; krok spat k ulozenemu stavu vrati tie iste odkazy, teda ziadny zapis
    private IEnumerable<FileWrite> Writes(int index) => Groups[index].Changes(DataDir, _saved.Groups[index]).Where(write => write.IsNeeded);

    /// <summary>
    /// Subor GV.XML grafikonu.
    /// </summary>
    public string TimetableFile(GroupDocument group) => Path.Combine(DataDir, group.Group.Name, GvFile.FileName);

    /// <summary>
    /// Zapise zmenene grafikony naraz. Pri beziacom serveri instalacie nezapise nic; subor, ktory po nacitani zmenil iny
    /// program, prepise len pri <paramref name="overwriteChanged" />.
    /// </summary>
    /// <param name="overwriteChanged">prepisat aj subory zmenene inym programom</param>
    /// <exception cref="SaveFailedException">zapis zlyhal</exception>
    public SaveResult Save(bool overwriteChanged = false)
    {
        var writes = InstallationWrites(DataDir).Concat(DirtyGroups().SelectMany(Writes)).ToList();
        if (writes.Count == 0)
            return SaveResult.Of(SaveOutcome.NothingToSave);
        if (IsServerRunning())
            return SaveResult.Of(SaveOutcome.ServerRunning);

        if (!overwriteChanged)
        {
            var changed = writes.Select(write => write.Path).Where(file => _stamps.TryGetValue(file, out var stamp) && FileStamp.Of(file) != stamp).ToList();
            if (changed.Count > 0)
                return new SaveResult(SaveOutcome.ChangedOnDisk, changed);
        }

        Log.Information("Ukladaju sa subory {Files} instalacie {DataDir}", writes.Select(write => Path.GetRelativePath(DataDir, write.Path)), DataDir);
        var transaction = new FileTransaction(writes.Select(write => write.Path));
        try
        {
            foreach (var write in writes)
                write.Write();
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException or XmlException)
        {
            Log.Error(e, "Subory instalacie {DataDir} sa nepodarilo ulozit - vracaju sa do stavu pred ulozenim", DataDir);
            throw new SaveFailedException(transaction.TryRollback(), transaction.BackupPath, e);
        }

        transaction.Commit();
        foreach (var write in writes)
            _stamps[write.Path] = FileStamp.Of(write.Path);
        _saved = History.Current;
        return SaveResult.Of(SaveOutcome.Saved);
    }

    /// <summary>
    /// Grafikon stanice tak, ako ho vidi server (kolaje, platna varianta v den), s vlakmi aktualneho stavu vratane neulozenych
    /// zmien. Grafikon bez nacitanych vlakov cita subor.
    /// </summary>
    /// <param name="group">grafikon</param>
    /// <param name="station">stanica grafikonu</param>
    public StationTimetable StationTimetable(GroupDocument group, DirListItem station)
    {
        ArgumentNullException.ThrowIfNull(group);
        return group.Timetable is { } timetable
            ? ToolsCore.Iniss2.Grafikon.StationTimetable.Read(DataDir, group.Group, station, timetable)
            : ToolsCore.Iniss2.Grafikon.StationTimetable.Read(DataDir, group.Group, station);
    }

    /// <summary>
    /// Server tejto instalacie bezi.
    /// </summary>
    public bool IsServerRunning() => Iniss2Server.IsRunning(ServerDir);
}
