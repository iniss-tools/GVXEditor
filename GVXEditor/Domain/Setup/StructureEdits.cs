using System.Globalization;
using System.Xml;
using GVXEditor.Domain.Stations;
using GVXEditor.Properties;
using ToolsCore.Iniss.Tools;
using ToolsCore.Iniss2.Grafikon;
using ToolsCore.Iniss2.Tables;

namespace GVXEditor.Domain.Setup;

/// <summary>
/// Zmeny struktury instalacie, ktore sa zapisu hned (nie su krokom historie): novy grafikon a nova stanica. Zapisuju
/// DirList.xml, priecinky so subormi a SR70.TXT; pri chybe sa vsetko vrati. Volaju sa pri zastavenom serveri a bez
/// neulozenych zmien, potom sa instalacia nacita znova.
/// </summary>
internal static class StructureEdits
{
    private static string Format(string format, params object[] args) => string.Format(CultureInfo.CurrentCulture, format, args);

    /// <summary>
    /// Nazov priecinka je platny (nie prazdny, bez zakazanych znakov, nie <c>.</c> ani <c>..</c>, bez medzery na konci).
    /// </summary>
    public static bool IsValidFolderName(string name) =>
        name.Trim().Length > 0 && name.IndexOfAny(Path.GetInvalidFileNameChars()) < 0 && name.Trim('.').Length > 0 && name == name.TrimEnd(' ', '.');

    /// <summary>
    /// Navrh priecinka novej stanice ako v datach: poradove cislo a nazov, zakazane znaky sa vynechaju.
    /// </summary>
    /// <param name="position">poradie stanice v grafikone od 1</param>
    /// <param name="name">nazov stanice</param>
    public static string SuggestDirectory(int position, string name)
    {
        var clean = new string([.. name.Where(ch => Array.IndexOf(Path.GetInvalidFileNameChars(), ch) < 0)]).Trim().TrimEnd('.');
        return string.Create(CultureInfo.InvariantCulture, $"{position:00} {clean}").TrimEnd();
    }

    /// <summary>
    /// Chyba nazvu noveho grafikonu; <see langword="null" />, ak sa grafikon da zalozit.
    /// </summary>
    public static string? CheckGroupName(Iniss2Workspace workspace, string name)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        if (name.Trim().Length == 0)
            return Resources.Structure_NoGroupName;
        if (!IsValidFolderName(name))
            return Format(Resources.Structure_BadFolder, name);
        if (workspace.Groups.Any(group => string.Equals(group.Group.Name, name, StringComparison.OrdinalIgnoreCase)))
            return Format(Resources.Structure_GroupExists, name);
        if (Directory.Exists(Path.Combine(workspace.DataDir, name)) || File.Exists(Path.Combine(workspace.DataDir, name)))
            return Format(Resources.Structure_FolderExists, name);
        return null;
    }

    /// <summary>
    /// Chyby novej stanice; prazdny zoznam = stanica sa da pridat.
    /// </summary>
    public static List<string> Check(Iniss2Workspace workspace, NewStation station)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        ArgumentNullException.ThrowIfNull(station);
        var problems = new List<string>();
        var group = workspace.Groups[station.Group];
        var groupDir = Path.Combine(workspace.DataDir, group.Group.Name);

        if (station.Station <= 0)
            problems.Add(Resources.Structure_BadStation);
        else if (group.Group.Items.Any(item => item.Station == station.Station))
            problems.Add(Format(Resources.Structure_StationExists, station.Station));

        if (station.Directory.Trim().Length == 0)
            problems.Add(Resources.Structure_NoDirectory);
        else if (!IsValidFolderName(station.Directory))
            problems.Add(Format(Resources.Structure_BadFolder, station.Directory));
        else if (group.Group.Items.Any(item => string.Equals(item.Directory, station.Directory, StringComparison.OrdinalIgnoreCase)) ||
                 Directory.Exists(Path.Combine(groupDir, station.Directory)) || File.Exists(Path.Combine(groupDir, station.Directory)))
            problems.Add(Format(Resources.Structure_FolderExists, station.Directory));

        var named = group.Setup?.Names.Any(name => name.Number == station.Station) == true;
        if (!named && station.Name.Trim().Length == 0)
            problems.Add(Resources.Structure_NoStationName);

        if (station.TemplateGroup < 0 || station.TemplateGroup >= workspace.Groups.Count ||
            station.TemplateStation < 0 || station.TemplateStation >= workspace.Groups[station.TemplateGroup].Stations.Count)
            problems.Add(Resources.Structure_NoTemplate);

        return problems;
    }

    /// <summary>
    /// Zalozi novy grafikon ako kopiu predlohy: subory grafikonu a priecinky jeho stanic. Bez vlakov dostane GV.XML bez vlakov,
    /// prazdne vyluky a odklony a stanice bez zaznamov vlakov (sady textov tabul ostanu). Grafikon pribudne na koniec DirList.xml.
    /// </summary>
    /// <param name="workspace">otvorena instalacia (bez neulozenych zmien)</param>
    /// <param name="name">nazov priecinka grafikonu</param>
    /// <param name="template">poradie grafikonu predlohy</param>
    /// <param name="copyTrains">skopirovat aj vlaky, vyluky, odklony a zaznamy vlakov v staniciach</param>
    /// <exception cref="IOException">zapis zlyhal, zmeny su vratene</exception>
    /// <exception cref="UnauthorizedAccessException">zapis zlyhal, zmeny su vratene</exception>
    /// <exception cref="XmlException">subor predlohy nie je XML, zmeny su vratene</exception>
    /// <exception cref="FormatException">subor predlohy ma chybny obsah, zmeny su vratene</exception>
    public static void NewGroup(Iniss2Workspace workspace, string name, int template, bool copyTrains)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        var dataDir = workspace.DataDir;
        var source = workspace.Groups[template];
        var sourceDir = Path.Combine(dataDir, source.Group.Name);
        var targetDir = Path.Combine(dataDir, name);
        var groups = DirListXml.Load(dataDir);
        var items = groups.FirstOrDefault(group => string.Equals(group.Name, source.Group.Name, StringComparison.OrdinalIgnoreCase))?.Items ?? source.Group.Items;

        var transaction = new FileTransaction([Path.Combine(dataDir, DirListXml.FileName)]);
        try
        {
            Directory.CreateDirectory(targetDir);
            foreach (var file in Directory.GetFiles(sourceDir).Where(file => !IsLog(file)))
                File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)));
            foreach (var item in source.Group.Items)
                CopyDirectory(Path.Combine(sourceDir, item.Directory), Path.Combine(targetDir, item.Directory));

            if (!copyTrains)
            {
                if (source.Timetable is { } timetable)
                    (timetable with { Trains = [] }).Write(targetDir);
                ClearIfExists(targetDir, VylukyFile.FileName, () => VylukyFile.Write(targetDir, []));
                ClearIfExists(targetDir, OdklonyFile.FileName, () => OdklonyFile.Write(targetDir, []));
                foreach (var item in source.Group.Items)
                    ClearTrainRecords(Path.Combine(targetDir, item.Directory));
            }

            groups.Add(new DirListGroup(name, items));
            DirListXml.Write(dataDir, groups);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException or XmlException or FormatException)
        {
            transaction.TryRollback();
            TryDeleteDirectory(targetDir);
            throw;
        }

        transaction.Commit();
        Serilog.Log.Information("Novy grafikon {Group} z predlohy {Template} (vlaky {CopyTrains}) v {DataDir}", name, source.Group.Name, copyTrains, dataDir);
    }

    /// <summary>
    /// Prida stanicu do grafikonu: priecinok s nastavenim predlohy bez zaznamov vlakov, SR70.TXT, nazov do GV.stanice.XML
    /// (ak tam cislo nie je) a zaznam na koniec grafikonu v DirList.xml s linkami predlohy.
    /// </summary>
    /// <param name="workspace">otvorena instalacia (bez neulozenych zmien)</param>
    /// <param name="station">nova stanica (skontrolovana cez <see cref="Check" />)</param>
    /// <exception cref="IOException">zapis zlyhal, zmeny su vratene</exception>
    /// <exception cref="UnauthorizedAccessException">zapis zlyhal, zmeny su vratene</exception>
    /// <exception cref="XmlException">subor predlohy nie je XML, zmeny su vratene</exception>
    /// <exception cref="FormatException">subor predlohy ma chybny obsah, zmeny su vratene</exception>
    public static void AddStation(Iniss2Workspace workspace, NewStation station)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        ArgumentNullException.ThrowIfNull(station);
        var dataDir = workspace.DataDir;
        var group = workspace.Groups[station.Group];
        var groupDir = Path.Combine(dataDir, group.Group.Name);
        var stationDir = Path.Combine(groupDir, station.Directory);
        var templateGroup = workspace.Groups[station.TemplateGroup];
        var template = templateGroup.Stations[station.TemplateStation].Station;
        var templateDir = Path.Combine(dataDir, templateGroup.Group.Name, template.Directory);
        var names = group.Setup?.Names ?? GvStationsFile.Read(groupDir);
        var addName = names.All(name => name.Number != station.Station);

        var transaction = new FileTransaction([Path.Combine(dataDir, DirListXml.FileName), Path.Combine(groupDir, GvStationsFile.FileName)]);
        try
        {
            CopyDirectory(templateDir, stationDir);
            ClearTrainRecords(stationDir);
            DirListXml.WriteStationNumber(stationDir, station.Station);
            if (addName)
                GvStationsFile.Write(groupDir, [.. names, new GvStation(station.Station, station.Name.Trim())]);
            DirListXml.AddStation(dataDir, group.Group.Name,
                new DirListItem(station.Directory, station.Station, template.TableLine, template.AudioLine, station.Flags));
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException or XmlException or FormatException)
        {
            transaction.TryRollback();
            TryDeleteDirectory(stationDir);
            throw;
        }

        transaction.Commit();
        Serilog.Log.Information("Nova stanica {Station} ({Directory}) v grafikone {Group} z predlohy {Template}", station.Station, station.Directory,
            group.Group.Name, templateDir);
    }

    // log servera k suboru (napr. GV.XML.log) patri povodnemu grafikonu
    private static bool IsLog(string file) => string.Equals(Path.GetExtension(file), ".log", StringComparison.OrdinalIgnoreCase);

    private static void CopyDirectory(string source, string target)
    {
        Directory.CreateDirectory(target);
        foreach (var file in Directory.GetFiles(source).Where(file => !IsLog(file)))
            File.Copy(file, Path.Combine(target, Path.GetFileName(file)));
        foreach (var directory in Directory.GetDirectories(source))
            CopyDirectory(directory, Path.Combine(target, Path.GetFileName(directory)));
    }

    private static void ClearIfExists(string dir, string file, Action clear)
    {
        if (File.Exists(Path.Combine(dir, file)))
            clear();
    }

    // zaznamy vlakov stanice sa vyprazdnia na mieste (komentare ostanu); sady textov tabul ostanu bez textov vlakov, aby sa
    // texty dali pridat - server by sadu bez textov sam nezapisal
    private static void ClearTrainRecords(string stationDir)
    {
        var files = StationFiles.Load(stationDir);
        var empty = new StationFiles([], [], [], [], [], files.TextSets, []);
        foreach (var write in empty.Changes(stationDir, files).Where(write => write.IsNeeded))
            write.Write();
        TrainTextsFile.RemoveTrains(stationDir);
    }

    private static void TryDeleteDirectory(string dir)
    {
        try
        {
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            Serilog.Log.Warning(e, "Priecinok {Directory} sa po zlyhanom zapise nepodarilo zmazat", dir);
        }
    }
}
