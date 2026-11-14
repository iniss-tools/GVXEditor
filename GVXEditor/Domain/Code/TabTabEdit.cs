using System.ComponentModel;
using GVXEditor.Domain.Code.Drafts;
using ToolsCore.Iniss2.Tables;

namespace GVXEditor.Domain.Code;

/// <summary>
/// Tabulky jedneho suboru TabTab v okne Tabulky TabTab: spolocne tabulky grafikonu alebo tabulky stanice.
/// </summary>
internal sealed class TabTabEdit
{
    private TabTabEdit(int station, IReadOnlyList<TabTabTable> original)
    {
        Station = station;
        Original = original;
    }

    /// <summary>Poradie stanice v grafikone; -1 = spolocne tabulky grafikonu.</summary>
    public int Station { get; }

    /// <summary>Tabulky zo suboru.</summary>
    public IReadOnlyList<TabTabTable> Original { get; }

    /// <summary>Tabulky v poradi suboru.</summary>
    public BindingList<TableDraft> Tables { get; } = [];

    /// <summary>Koncept suboru.</summary>
    /// <param name="station">poradie stanice; -1 = spolocne tabulky grafikonu</param>
    /// <param name="tables">tabulky zo suboru</param>
    public static TabTabEdit Of(int station, IReadOnlyList<TabTabTable> tables)
    {
        ArgumentNullException.ThrowIfNull(tables);
        var edit = new TabTabEdit(station, tables);
        foreach (var table in tables)
            edit.Tables.Add(TableDraft.Of(table));
        return edit;
    }

    /// <summary>
    /// Tabulky z konceptu; bez zmeny vrati <see cref="Original" />.
    /// </summary>
    public IReadOnlyList<TabTabTable> Apply()
    {
        var tables = Tables.Select(table => table.To()).ToList();
        return Original.Count == tables.Count && Original.Zip(tables).All(pair => ReferenceEquals(pair.First, pair.Second)) ? Original : tables;
    }
}
