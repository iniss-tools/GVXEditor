using System.Globalization;
using GVXEditor.Domain.Code;
using GVXEditor.Domain.Code.Drafts;
using GVXEditor.Properties;

namespace GVXEditor.Domain.Rules;

/// <summary>
/// Kontrola stavoveho diagramu a tabuliek TabTab bez prekladu: to, pre co server subor nenacita alebo nespusti, je chyba,
/// to, co server alebo klient pouzije inak, upozornenie. Preklad kodu kontroluje most kodom servera.
/// </summary>
internal static class CodeRules
{
    /// <summary>Priznaky stavu, ktore server pozna (neznamy priznak je chyba citania suboru).</summary>
    public static readonly IReadOnlyList<string> NodeFlags =
    [
        "AUXSTATE", "STOJI", "POTVRZENAKOLEJ", "NYNISTOJI", "ODBAVEN", "SHADOW", "JENAODJEZDOVE", "JENAPRIJEZDOVE", "JENASMEROVYCH",
        "JEZOBRAZENAPOZICE", "JEZOBRAZENAPOZICEPRIJ", "JEZOBRAZENAPOZICEODJ", "JEZOBRAZENAKOLEJ", "BLIKA", "NENASTUPOVAT",
        "NEAKTIVNIPRONENASTUPOVAT", "JENPRONENASTUPOVAT", "ISPRIJEZD", "ISODJEZD"
    ];

    /// <summary>Priznaky automatiky, ktore server pozna.</summary>
    public static readonly IReadOnlyList<string> AutomationFlags = ["NEVYPINATBEZKLIENTA", "NEVYPINATPOSTARTU"];

    private static string Format(string format, params object[] args) => string.Format(CultureInfo.CurrentCulture, format, args);

    /// <summary>
    /// Stavovy diagram stanice.
    /// </summary>
    /// <param name="edit">koncept diagramu</param>
    /// <param name="station">nazov stanice pre text zisteni</param>
    /// <param name="functions">funkcie skriptu hlaseni - hlasenie stavu server vola ako jeho metodu; <see langword="null" />, ak skript chyba</param>
    public static List<SetupProblem> Check(StateDiagramEdit edit, string station, IReadOnlyDictionary<string, HashSet<string>>? functions)
    {
        ArgumentNullException.ThrowIfNull(edit);
        var problems = new List<SetupProblem>();
        var where = Format(Resources.Loc_Station, Resources.Page_StateDiagram, station);
        if (edit.Graphs.Count > 0 && edit.GraphIndex.Trim().Length == 0)
            problems.Add(new SetupProblem(true, Format(Resources.Code_NoGraphIndex, where)));

        for (var g = 0; g < edit.Graphs.Count; g++)
        {
            var graph = edit.Graphs[g];
            var at = Format(Resources.Loc_Graph, where, graph.Description.Length > 0 ? graph.Description : (g + 1).ToString(CultureInfo.CurrentCulture));
            var keys = new HashSet<string>(StringComparer.Ordinal);
            var lastPosition = int.MinValue;
            foreach (var node in graph.Nodes)
            {
                var place = Format(Resources.Loc_Node, at, node.Key.Length > 0 ? node.Key : "?");
                if (node.Key.Length == 0)
                    problems.Add(new SetupProblem(true, Format(Resources.Code_NoNodeKey, place)));
                else if (!keys.Add(node.Key))
                    problems.Add(new SetupProblem(false, Format(Resources.Code_DuplicateNode, place)));

                foreach (var flag in node.Flags.Where(flag => !NodeFlags.Contains(flag)))
                    problems.Add(new SetupProblem(true, Format(Resources.Code_UnknownFlag, place, flag)));
                if (node.Report.Length > 0 && functions is not null && !functions.ContainsKey(node.Report))
                    problems.Add(new SetupProblem(false, Format(Resources.Code_UnknownReport, place, node.Report)));

                if (node.HasButton)
                {
                    if (node.Position is not { } position)
                        problems.Add(new SetupProblem(true, Format(Resources.Code_BadPosition, place, node.ButtonPosition)));
                    else if (position <= lastPosition)
                        problems.Add(new SetupProblem(false, Format(Resources.Code_PositionOrder, place, position)));
                    else
                        lastPosition = position;
                }

                if (node.HasAutomation)
                {
                    if (node.Time.Trim().Length == 0)
                        problems.Add(new SetupProblem(true, Format(Resources.Code_NoTime, place)));
                    foreach (var flag in node.AutomationFlags.Where(flag => !AutomationFlags.Contains(flag)))
                        problems.Add(new SetupProblem(true, Format(Resources.Code_UnknownFlag, place, flag)));
                }
            }
        }

        return problems;
    }

    /// <summary>
    /// Tabulky TabTab jedneho suboru.
    /// </summary>
    /// <param name="edit">koncept suboru</param>
    /// <param name="where">nazov suboru pre text zisteni (spolocne tabulky alebo stanica)</param>
    /// <param name="global">nazvy spolocnych tabuliek - stanica nesmie mat tabulku s rovnakym nazvom; pri spolocnych prazdne</param>
    public static List<SetupProblem> Check(TabTabEdit edit, string where, IReadOnlySet<string> global)
    {
        ArgumentNullException.ThrowIfNull(edit);
        ArgumentNullException.ThrowIfNull(global);
        var problems = new List<SetupProblem>();
        var names = new HashSet<string>(StringComparer.Ordinal);
        foreach (var table in edit.Tables)
        {
            var at = Format(Resources.Loc_Table, where, table.Name.Length > 0 ? table.Name : "?");
            if (table.Name.Trim().Length == 0)
                problems.Add(new SetupProblem(true, Format(Resources.Code_NoTableName, at)));
            else if (!names.Add(table.Name))
                problems.Add(new SetupProblem(true, Format(Resources.Code_DuplicateTable, at)));
            else if (global.Contains(table.Name))
                problems.Add(new SetupProblem(true, Format(Resources.Code_TableInGlobal, at)));

            var comparer = table.IgnoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
            var entries = new HashSet<(string, int)>(new EntryComparer(comparer));
            for (var i = 0; i < table.Entries.Count; i++)
            {
                var entry = table.Entries[i];
                var row = Format(Resources.Loc_Row, at, i + 1);
                if (entry.KeyFontNumber is not { } font)
                    problems.Add(new SetupProblem(true, Format(Resources.Code_BadFont, row, entry.KeyFont)));
                else if (!entries.Add((entry.KeyText, font)))
                    problems.Add(new SetupProblem(true, Format(Resources.Code_DuplicateEntry, row, entry.KeyText)));
                if (entry.ValueFontNumber is null)
                    problems.Add(new SetupProblem(true, Format(Resources.Code_BadFont, row, entry.ValueFont)));
            }
        }

        return problems;
    }

    /// <summary>Rovnost dvojice textu a pisma podla porovnania textu tabulky.</summary>
    private sealed class EntryComparer(StringComparer text) : IEqualityComparer<(string Text, int Font)>
    {
        public bool Equals((string Text, int Font) x, (string Text, int Font) y) => x.Font == y.Font && text.Equals(x.Text, y.Text);

        public int GetHashCode((string Text, int Font) obj) => HashCode.Combine(text.GetHashCode(obj.Text), obj.Font);
    }
}
