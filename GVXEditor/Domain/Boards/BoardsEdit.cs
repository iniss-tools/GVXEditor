using System.ComponentModel;
using GVXEditor.Domain.Boards.Drafts;
using GVXEditor.Domain.Setup;

namespace GVXEditor.Domain.Boards;

/// <summary>
/// Tabule a katalogove predlohy jednej stanice v okne Tabule.
/// </summary>
internal sealed class BoardsEdit
{
    private BoardsEdit(int station, StationBoards original)
    {
        Station = station;
        Original = original;
    }

    /// <summary>Poradie stanice v grafikone.</summary>
    public int Station { get; }

    /// <summary>Tabule a predlohy zo suborov.</summary>
    public StationBoards Original { get; }

    /// <summary>Tabule v poradi suboru.</summary>
    public BindingList<BoardDraft> Tables { get; } = [];

    /// <summary>Katalogove predlohy v poradi suboru.</summary>
    public BindingList<TemplateDraft> Templates { get; } = [];

    /// <summary>Koncept tabul stanice.</summary>
    /// <param name="station">poradie stanice v grafikone</param>
    /// <param name="boards">tabule a predlohy zo suborov</param>
    public static BoardsEdit Of(int station, StationBoards boards)
    {
        ArgumentNullException.ThrowIfNull(boards);
        var edit = new BoardsEdit(station, boards);
        foreach (var table in boards.Tables)
            edit.Tables.Add(BoardDraft.Of(table));
        foreach (var template in boards.Catalog)
            edit.Templates.Add(TemplateDraft.Of(template));
        return edit;
    }

    /// <summary>
    /// Tabule z konceptu; bez zmeny vrati <see cref="Original" />, nezmeneny subor ostane tym istym zoznamom.
    /// </summary>
    public StationBoards Apply()
    {
        var tables = SetupEquality.Keep(Original.Tables, [.. Tables.Select(table => table.To())], ReferenceEquals);
        var catalog = SetupEquality.Keep(Original.Catalog, [.. Templates.Select(template => template.To())], ReferenceEquals);
        return ReferenceEquals(tables, Original.Tables) && ReferenceEquals(catalog, Original.Catalog) ? Original : new StationBoards(tables, catalog);
    }
}
