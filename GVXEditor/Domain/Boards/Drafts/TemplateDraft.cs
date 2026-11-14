using System.ComponentModel;
using ToolsCore.Iniss2.Tables;

namespace GVXEditor.Domain.Boards.Drafts;

/// <summary>
/// Katalogova predloha v okne.
/// </summary>
internal sealed class TemplateDraft
{
    /// <summary>Predloha zo suboru; <see langword="null" /> = nova predloha.</summary>
    public CatalogTable? Original { get; private init; }

    /// <summary>Kluc, na ktory sa odkazuje cast tabule.</summary>
    public string Key { get; set; } = "";

    /// <summary>Nazov.</summary>
    public string Name { get; set; } = "";

    /// <summary>Druh zariadenia (<c>cls</c>).</summary>
    public string Class { get; set; } = CatalogTable.Internet;

    /// <summary>Banka pisem (<c>f</c>).</summary>
    public string FontBank { get; set; } = "";

    /// <summary>Udaje v poradi suboru.</summary>
    public BindingList<ColumnDraft> Columns { get; } = [];

    /// <summary>Text v zozname okna.</summary>
    public override string ToString() => Name.Length > 0 && Name != Key ? $"{Key} ({Name})" : Key;

    /// <summary>Koncept predlohy.</summary>
    public static TemplateDraft Of(CatalogTable table)
    {
        ArgumentNullException.ThrowIfNull(table);
        var draft = new TemplateDraft { Original = table, Key = table.Key, Name = table.Name, Class = table.Class, FontBank = table.FontBank };
        foreach (var column in table.Columns)
            draft.Columns.Add(ColumnDraft.Of(column));
        return draft;
    }

    /// <summary>Predloha suboru; bez zmeny vrati povodnu predlohu (ten isty objekt).</summary>
    public CatalogTable To()
    {
        var table = new CatalogTable(Key, Name, Class) { FontBank = FontBank, Columns = [.. Columns.Select(column => column.To())] };
        return Original is { } original && BoardEquality.Same(original, table) ? original : table;
    }
}
