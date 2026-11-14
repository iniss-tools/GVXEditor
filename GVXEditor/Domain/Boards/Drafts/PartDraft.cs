using System.ComponentModel;
using System.Globalization;
using ToolsCore.Iniss2.Tables;

namespace GVXEditor.Domain.Boards.Drafts;

/// <summary>
/// Cast tabule v okne. Cisla su text, chybu hlasia pravidla okna.
/// </summary>
internal sealed class PartDraft
{
    /// <summary>Cast zo suboru; <see langword="null" /> = nova cast.</summary>
    public TablePart? Original { get; private init; }

    /// <summary>Nazov; pri logickych tabuliach skupina.</summary>
    public string Name { get; set; } = "";

    /// <summary>Kluc katalogovej predlohy stanice (<c>ck</c>).</summary>
    public string CatalogKey { get; set; } = "";

    /// <summary>Druh casti (<c>t</c>); prazdny = NODEF.</summary>
    public string Kind { get; set; } = "";

    /// <summary>Pocet zaznamov ako text.</summary>
    public string Records { get; set; } = "1";

    /// <summary>Cisla stanic oddelene medzerou; prazdne = stanica tabule.</summary>
    public string Stations { get; set; } = "";

    /// <summary>Kluc predchadzajucej tabule.</summary>
    public string Previous { get; set; } = "";

    /// <summary>Kluc weboveho zdroja (len INFO).</summary>
    public string WebSource { get; set; } = "";

    /// <summary>Podmienka zobrazenia vlaku, kod VB.NET.</summary>
    public string Condition { get; set; } = "";

    /// <summary>Rozdelenie zaznamov do multiokien.</summary>
    public string WindowDefinition { get; set; } = "";

    /// <summary>Kolaje casti v poradi suboru.</summary>
    public BindingList<TrackRefDraft> Tracks { get; } = [];

    /// <summary>Pocet zaznamov; <see langword="null" />, ak text nie je cislo od 0.</summary>
    public int? RecordsNumber =>
        int.TryParse(Records.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out var records) ? records : null;

    /// <summary>Text v strome okna.</summary>
    public override string ToString() => Name.Length > 0 ? Name : CatalogKey;

    /// <summary>Koncept casti.</summary>
    public static PartDraft Of(TablePart part)
    {
        ArgumentNullException.ThrowIfNull(part);
        var draft = new PartDraft
        {
            Original = part,
            Name = part.Name,
            CatalogKey = part.CatalogKey,
            Kind = part.Kind,
            Records = part.Records.ToString(CultureInfo.InvariantCulture),
            Stations = part.Stations,
            Previous = part.Previous,
            WebSource = part.WebSource,
            Condition = part.Condition,
            WindowDefinition = part.WindowDefinition
        };
        foreach (var track in part.Tracks)
        {
            draft.Tracks.Add(new TrackRefDraft
            {
                Key = track,
                Priority = part.TrackPriorities.TryGetValue(track, out var priority) ? priority.ToString(CultureInfo.InvariantCulture) : ""
            });
        }

        return draft;
    }

    /// <summary>Cast suboru; bez zmeny vrati povodnu cast (ten isty objekt).</summary>
    public TablePart To()
    {
        var priorities = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var track in Tracks)
        {
            if (TrackPriority(track.Priority) is { } priority)
                priorities.TryAdd(track.Key, priority);
        }

        var part = new TablePart(Name, CatalogKey, Kind, RecordsNumber ?? 0)
        {
            Tracks = [.. Tracks.Select(track => track.Key)],
            TrackPriorities = priorities,
            Stations = Stations,
            Previous = Previous,
            WebSource = WebSource,
            Condition = Condition,
            WindowDefinition = WindowDefinition
        };
        return Original is { } original && BoardEquality.Same(original, part) ? original : part;
    }

    /// <summary>Priorita kolaje z textu; <see langword="null" /> = bez priority alebo text nie je cislo.</summary>
    public static int? TrackPriority(string text) =>
        int.TryParse(text.Trim(), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var priority) ? priority : null;
}
