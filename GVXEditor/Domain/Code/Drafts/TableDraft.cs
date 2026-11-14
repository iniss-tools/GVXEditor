using System.ComponentModel;
using ToolsCore.Iniss2.Tables;

namespace GVXEditor.Domain.Code.Drafts;

/// <summary>
/// Tabulka TabTab v okne: vyraz VB.NET v tvare na zobrazenie a slovnik.
/// </summary>
internal sealed class TableDraft
{
    // odsadenie viacriadkoveho vyrazu v subore: prvok expr je pod tabulkou
    private const string ExpressionIndent = "      ";
    private const string ExpressionClosing = "    ";

    /// <summary>Tabulka zo suboru; <see langword="null" /> = nova tabulka.</summary>
    public TabTabTable? Original { get; private init; }

    /// <summary>Nazov, na ktory sa odkazuje stlpec predlohy.</summary>
    public string Name { get; set; } = "";

    /// <summary>Popis pre citatela suboru.</summary>
    public string Description { get; set; } = "";

    /// <summary>Obsluha vidi prekodovany text.</summary>
    public bool ViewValues { get; set; }

    /// <summary>Slovnik nerozlisuje velkost pismen.</summary>
    public bool IgnoreCase { get; set; }

    /// <summary>Vyraz (telo funkcie); prazdny = bez vyrazu.</summary>
    public string Expression { get; set; } = "";

    /// <summary>Slovnik v poradi suboru.</summary>
    public BindingList<EntryDraft> Entries { get; } = [];

    /// <summary>Koncept tabulky.</summary>
    public static TableDraft Of(TabTabTable table)
    {
        ArgumentNullException.ThrowIfNull(table);
        var draft = new TableDraft
        {
            Original = table,
            Name = table.Name,
            Description = table.Description,
            ViewValues = table.ViewValues,
            IgnoreCase = table.IgnoreCase,
            Expression = EmbeddedCode.Display(table.Expression)
        };
        foreach (var entry in table.Entries)
            draft.Entries.Add(EntryDraft.Of(entry));
        return draft;
    }

    /// <summary>Tabulka suboru; bez zmeny vrati povodnu tabulku (ten isty objekt).</summary>
    public TabTabTable To()
    {
        var original = Original;
        var entries = Entries.Select(entry => entry.To()).ToList();
        var table = new TabTabTable(Name)
        {
            Description = Description,
            ViewValues = ViewValues,
            IgnoreCase = IgnoreCase,
            Expression = EmbeddedCode.Store(Expression, original?.Expression ?? "", ExpressionIndent, ExpressionClosing),
            Entries = original is not null && original.Entries.SequenceEqual(entries) ? original.Entries : entries
        };
        return original is not null && original.Name == table.Name && original.Description == table.Description && original.ViewValues == table.ViewValues &&
               original.IgnoreCase == table.IgnoreCase && original.Expression == table.Expression && ReferenceEquals(original.Entries, table.Entries)
            ? original
            : table;
    }
}
