using System.Globalization;
using GVXEditor.Domain.Boards;
using GVXEditor.Domain.Boards.Drafts;
using GVXEditor.Properties;
using ToolsCore.Iniss2.Tables;

namespace GVXEditor.Domain.Rules;

/// <summary>
/// Kontrola tabul a katalogovych predloh stanice podla servera: to, pre co server subor nenacita alebo sa nespusti, je chyba,
/// to, co len zapise do logu alebo nepouzije, upozornenie. Podmienky zobrazenia prelozi most kodom servera.
/// </summary>
internal static class BoardRules
{
    private static readonly char[] ArrowSeparators = [' ', ',', ';', ':', '\t'];
    private static readonly string[] LightTimeFormats = ["HH:mm:ss", "HH:mm"];

    private static string Format(string format, params object[] args) => string.Format(CultureInfo.CurrentCulture, format, args);

    /// <summary>
    /// Tabule a predlohy stanice.
    /// </summary>
    /// <param name="edit">koncept tabul stanice</param>
    /// <param name="context">okolie stanice (linky, kolaje, tabulky TabTab, banky pisem)</param>
    public static List<SetupProblem> Check(BoardsEdit edit, BoardContext context)
    {
        ArgumentNullException.ThrowIfNull(edit);
        ArgumentNullException.ThrowIfNull(context);
        var problems = new List<SetupProblem>();
        var where = Format(Resources.Loc_Station, Resources.Page_Boards, context.Station);
        var templates = new Dictionary<string, TemplateDraft>(StringComparer.Ordinal);
        foreach (var template in edit.Templates)
            templates.TryAdd(template.Key, template);

        CheckTables(edit, context, where, templates, problems);
        CheckTemplates(edit, context, where, problems);
        return problems;
    }

    private static void CheckTables(BoardsEdit edit, BoardContext context, string where, Dictionary<string, TemplateDraft> templates, List<SetupProblem> problems)
    {
        var keys = new HashSet<string>(StringComparer.Ordinal);
        var previousUsers = new Dictionary<string, int>(StringComparer.Ordinal);
        for (var t = 0; t < edit.Tables.Count; t++)
        {
            var table = edit.Tables[t];
            var at = Format(Resources.Loc_Board, where, table.Key.Length > 0 ? table.Key : "?");
            if (table.Key.Length == 0)
                problems.Add(new SetupProblem(true, Format(Resources.Board_NoKey, at)));
            else if (!keys.Add(table.Key))
                problems.Add(new SetupProblem(false, Format(Resources.Board_DuplicateKey, at)));

            if (table.AddressNumber is null)
                problems.Add(new SetupProblem(true, Format(Resources.Board_BadNumber, at, Resources.Board_Address, table.Address)));
            CheckLights(table, at, problems);
            CheckArrows(table, at, problems);

            if (table.Parts.Count == 0)
                problems.Add(new SetupProblem(true, Format(Resources.Board_NoParts, at)));

            var classes = table.Parts.Select(part => templates.GetValueOrDefault(part.CatalogKey)?.Class).OfType<string>().Distinct(StringComparer.Ordinal).ToList();
            if (classes.Count > 1)
                problems.Add(new SetupProblem(true, Format(Resources.Board_MixedClasses, at, string.Join(", ", classes))));
            var tableClass = classes.Count == 1 ? classes[0] : null;
            if (tableClass is not null && BoardValues.UnsupportedClasses.Contains(tableClass))
                problems.Add(new SetupProblem(true, Format(Resources.Board_UnsupportedClass, at, tableClass)));

            // linku server hlada, len ak tabula komunikuje (adresa a zariadenie); linka stanice z DirList ma prednost
            var communicates = table.AddressNumber is { } address && address != TableDefinition.NoAddress &&
                               tableClass is not null and not CatalogTable.Internet;
            if (communicates && context.StationLine.Length == 0 && table.Line.Length > 0 && context.Lines is { } lines && !lines.Contains(table.Line))
                problems.Add(new SetupProblem(true, Format(Resources.Board_UnknownLine, at, table.Line)));

            foreach (var part in table.Parts)
                CheckPart(edit, t, part, tableClass, context, Format(Resources.Loc_Part, at, part), templates, previousUsers, problems);
        }
    }

    private static void CheckLights(BoardDraft table, string at, List<SetupProblem> problems)
    {
        foreach (var (name, value) in new[] { ("lighttime0", table.LightTime0), ("lighttime1", table.LightTime1) })
        {
            if (value.Length > 0 && !DateTime.TryParseExact(value, LightTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                problems.Add(new SetupProblem(true, Format(Resources.Board_BadLightTime, at, name, value)));
        }

        foreach (var (name, value) in new[]
                 {
                     ("light0", table.Light0), ("light1", table.Light1), ("lightoff0", table.LightOff0), ("lightoff1", table.LightOff1),
                     ("lightOnTime", table.LightOnTime)
                 })
        {
            if (value.Length > 0 && !int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
                problems.Add(new SetupProblem(true, Format(Resources.Board_BadNumber, at, name, value)));
        }

        foreach (var (name, value) in new[] { ("lightIfEmpty0", table.LightIfEmpty0), ("lightIfEmpty1", table.LightIfEmpty1) })
        {
            if (value.Length > 0 && !bool.TryParse(value, out _))
                problems.Add(new SetupProblem(true, Format(Resources.Board_BadBool, at, name, value)));
        }
    }

    // revarrows: chybu server len zapise do logu; zaznamy sa cisluju cez vsetky casti
    private static void CheckArrows(BoardDraft table, string at, List<SetupProblem> problems)
    {
        var records = table.Parts.Sum(part => part.RecordsNumber ?? 0);
        foreach (var item in table.ReverseArrows.Split(ArrowSeparators, StringSplitOptions.RemoveEmptyEntries))
        {
            if (!int.TryParse(item, NumberStyles.Integer, CultureInfo.InvariantCulture, out var record) || record < 0 || record >= records)
                problems.Add(new SetupProblem(false, Format(Resources.Board_BadArrow, at, item)));
        }
    }

    private static void CheckPart(BoardsEdit edit, int tableIndex, PartDraft part, string? tableClass, BoardContext context, string at,
        Dictionary<string, TemplateDraft> templates, Dictionary<string, int> previousUsers, List<SetupProblem> problems)
    {
        if (!templates.ContainsKey(part.CatalogKey))
            problems.Add(new SetupProblem(true, Format(Resources.Board_UnknownTemplate, at, part.CatalogKey)));
        if (part.Kind.Length > 0 && !BoardValues.PartKinds.Contains(part.Kind, StringComparer.Ordinal))
            problems.Add(new SetupProblem(true, Format(Resources.Board_UnknownValue, at, "t", part.Kind)));
        if (part.RecordsNumber is null)
            problems.Add(new SetupProblem(true, Format(Resources.Board_BadNumber, at, Resources.Board_Records, part.Records)));
        foreach (var station in part.Stations.Split(' ', StringSplitOptions.RemoveEmptyEntries).Where(station => !int.TryParse(station, NumberStyles.Integer,
                     CultureInfo.InvariantCulture, out _)))
            problems.Add(new SetupProblem(true, Format(Resources.Board_BadNumber, at, Resources.Board_Stations, station)));

        if (part.WebSource.Length > 0 && Kind(part) != BoardValues.InfoKind)
            problems.Add(new SetupProblem(false, Format(Resources.Board_WebSourceNotInfo, at)));

        if (part.WindowDefinition.Trim().Length > 0)
        {
            if (tableClass != BoardValues.MultiWindowClass)
                problems.Add(new SetupProblem(false, Format(Resources.Board_WindowsUnused, at)));
            else if (WindowDefinitionError(part.WindowDefinition, part.RecordsNumber ?? 0) is { } error)
                problems.Add(new SetupProblem(true, Format(Resources.Board_BadWindows, at, part.WindowDefinition, error)));
        }

        foreach (var track in part.Tracks)
        {
            if (track.Priority.Trim().Length > 0 && PartDraft.TrackPriority(track.Priority) is null)
                problems.Add(new SetupProblem(true, Format(Resources.Board_BadNumber, at, Resources.Board_Priority, track.Priority)));
            if (context.Tracks is { } tracks && !tracks.Contains(track.Key))
                problems.Add(new SetupProblem(false, Format(Resources.Board_UnknownTrack, at, track.Key)));
        }

        CheckPrevious(edit, tableIndex, part, at, previousUsers, problems);
    }

    // prev: ako server - predchadzajuca tabula musi byt v subore skor a mat cast rovnakeho druhu, stanic a kolaji
    private static void CheckPrevious(BoardsEdit edit, int tableIndex, PartDraft part, string at, Dictionary<string, int> previousUsers,
        List<SetupProblem> problems)
    {
        if (part.Previous.Length == 0)
            return;
        var index = -1;
        for (var i = 0; i < edit.Tables.Count && index < 0; i++)
        {
            if (edit.Tables[i].Key == part.Previous)
                index = i;
        }

        if (index < 0)
        {
            problems.Add(new SetupProblem(false, Format(Resources.Board_UnknownPrevious, at, part.Previous)));
            return;
        }

        var matching = index < tableIndex && edit.Tables[index].Parts.Any(other => other != part && Kind(other) == Kind(part) &&
            Stations(other).SequenceEqual(Stations(part)) && other.Tracks.Select(track => track.Key).SequenceEqual(part.Tracks.Select(track => track.Key)));
        if (!matching)
        {
            problems.Add(new SetupProblem(false, Format(Resources.Board_PreviousMismatch, at, part.Previous)));
            return;
        }

        var users = previousUsers.GetValueOrDefault(part.Previous);
        if (users > 0)
            problems.Add(new SetupProblem(false, Format(Resources.Board_PreviousShared, at, part.Previous)));
        previousUsers[part.Previous] = users + 1;
    }

    private static string[] Stations(PartDraft part) => part.Stations.Split(' ', StringSplitOptions.RemoveEmptyEntries);

    // chybajuci druh je NODEF
    private static string Kind(PartDraft part) => part.Kind.Length > 0 ? part.Kind : BoardValues.PartKinds[0];

    /// <summary>
    /// Chyba rozdelenia zaznamov do multiokien (<c>windef</c>) ako ho cita server; <see langword="null" /> = bez chyby.
    /// </summary>
    /// <param name="text">hodnota atributu</param>
    /// <param name="records">pocet zaznamov casti</param>
    public static string? WindowDefinitionError(string text, int records)
    {
        ArgumentNullException.ThrowIfNull(text);
        foreach (var group in text.Split(';').Select(group => group.Trim()).Where(group => group.Length > 0))
        {
            var colon = group.IndexOf(':', StringComparison.Ordinal);
            if (!IsWindow(colon < 0 ? group : group[..colon]))
                return Format(Resources.Board_WindowNumber, colon < 0 ? group : group[..colon]);
            if (colon < 0)
                continue;

            foreach (var item in group[(colon + 1)..].Split(',').Select(item => item.Trim()).Where(item => item.Length > 0))
            {
                var dash = item.IndexOf('-', StringComparison.Ordinal);
                var bounds = dash < 0 ? [item] : new[] { item[..dash], item[(dash + 1)..] };
                var numbers = bounds.Select(bound => int.TryParse(bound.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var n) && n >= 0 && n < records
                    ? n
                    : (int?)null).ToList();
                if (numbers.Any(number => number is null))
                    return Format(Resources.Board_WindowRecord, item, records);
                if (numbers.Count == 2 && numbers[0] > numbers[1])
                    return Format(Resources.Board_WindowRecord, item, records);
            }
        }

        return null;
    }

    private static bool IsWindow(string text) => int.TryParse(text.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var window) && window is >= 0 and <= 7;

    private static void CheckTemplates(BoardsEdit edit, BoardContext context, string where, List<SetupProblem> problems)
    {
        // predlohy, ktore tabule pouzivaju - pri nich server kontroluje druh a banku pisem
        var used = edit.Tables.SelectMany(table => table.Parts).Select(part => part.CatalogKey).ToHashSet(StringComparer.Ordinal);
        var keys = new HashSet<string>(StringComparer.Ordinal);
        foreach (var template in edit.Templates)
        {
            var at = Format(Resources.Loc_Template, where, template.Key.Length > 0 ? template.Key : "?");
            if (template.Key.Length == 0)
                problems.Add(new SetupProblem(true, Format(Resources.Board_NoKey, at)));
            else if (!keys.Add(template.Key))
                problems.Add(new SetupProblem(false, Format(Resources.Board_DuplicateKey, at)));

            if (!BoardValues.Classes.Contains(template.Class, StringComparer.Ordinal))
                problems.Add(new SetupProblem(true, Format(Resources.Board_UnknownValue, at, "cls", template.Class)));
            if (used.Contains(template.Key))
            {
                if (BoardValues.BinaryFontClasses.Contains(template.Class))
                {
                    if (template.FontBank.Length == 0)
                        problems.Add(new SetupProblem(true, Format(Resources.Board_NoFontBank, at, template.Class)));
                    else if (!context.FontFileExists(template.FontBank))
                        problems.Add(new SetupProblem(true, Format(Resources.Board_NoFontFile, at, template.FontBank)));
                }
                else if (BoardValues.DefaultFontClasses.Contains(template.Class) && template.FontBank.Length > 0)
                {
                    problems.Add(new SetupProblem(true, Format(Resources.Board_FontBankUnused, at, template.Class)));
                }
            }

            foreach (var column in template.Columns)
                CheckColumn(column, context, Format(Resources.Loc_Column, at, column.Key.Length > 0 ? column.Key : "?"), problems);
        }
    }

    private static void CheckColumn(ColumnDraft column, BoardContext context, string at, List<SetupProblem> problems)
    {
        foreach (var (attribute, value, known) in new[]
                 {
                     ("inf", column.Content, BoardValues.Contents), ("align", column.Align, BoardValues.Aligns), ("trn", column.Translation, BoardValues.Translations)
                 })
        {
            if (value.Length > 0 && !known.Contains(value, StringComparer.Ordinal))
                problems.Add(new SetupProblem(true, Format(Resources.Board_UnknownValue, at, attribute, value)));
        }

        foreach (var (attribute, value) in new[] { ("fnt", column.Font), ("r", column.Row), ("c1", column.Start), ("c2", column.End) })
        {
            if (ColumnDraft.Number(value) is null)
                problems.Add(new SetupProblem(true, Format(Resources.Board_BadNumber, at, attribute, value)));
        }

        // tabulky prekladu podla sposobu (chyba zastavi server, ak nie je /NoStopOnTabErr)
        var (needs1, needs2) = column.Translation switch
        {
            "FREE" => (false, false),
            "TABLETIME" => (true, true),
            _ => (true, false)
        };
        foreach (var (number, name, needed) in new[] { (1, column.Table1, needs1), (2, column.Table2, needs2) })
        {
            if (needed && name.Length == 0)
                problems.Add(new SetupProblem(true, Format(Resources.Board_MissingTabTab, at, number, column.Translation)));
            else if (!needed && name.Length > 0)
                problems.Add(new SetupProblem(true, Format(Resources.Board_ExtraTabTab, at, number, column.Translation)));
            else if (name.Length > 0 && context.TabTabs is { } tabTabs && !tabTabs.Contains(name))
                problems.Add(new SetupProblem(true, Format(Resources.Board_UnknownTabTab, at, name)));
        }
    }
}
