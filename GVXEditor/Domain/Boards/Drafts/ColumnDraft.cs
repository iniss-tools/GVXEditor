using System.Globalization;
using ToolsCore.Iniss2.Tables;

namespace GVXEditor.Domain.Boards.Drafts;

/// <summary>
/// Udaj katalogovej predlohy v okne (riadok tabulky). Cisla su text, chybu hlasia pravidla okna.
/// </summary>
internal sealed class ColumnDraft
{
    /// <summary>Kluc.</summary>
    public string Key { get; set; } = "";

    /// <summary>Nazov.</summary>
    public string Name { get; set; } = "";

    /// <summary>Druh obsahu (<c>inf</c>); prazdny = Not.</summary>
    public string Content { get; set; } = "";

    /// <summary>Cislo pisma ako text.</summary>
    public string Font { get; set; } = "0";

    /// <summary>Zarovnanie; prazdne = LEFT.</summary>
    public string Align { get; set; } = "";

    /// <summary>Sposob prekladu.</summary>
    public string Translation { get; set; } = CatalogColumn.FreeTranslation;

    /// <summary>Prva tabulka TabTab.</summary>
    public string Table1 { get; set; } = "";

    /// <summary>Druha tabulka TabTab.</summary>
    public string Table2 { get; set; } = "";

    /// <summary>Riadok zaznamu od 0 ako text; zaporny = udaj sa neukazuje.</summary>
    public string Row { get; set; } = "0";

    /// <summary>Zaciatok udaja ako text.</summary>
    public string Start { get; set; } = "0";

    /// <summary>Koniec udaja (prvy za udajom) ako text.</summary>
    public string End { get; set; } = "0";

    /// <summary>Koncept udaja.</summary>
    public static ColumnDraft Of(CatalogColumn column)
    {
        ArgumentNullException.ThrowIfNull(column);
        return new ColumnDraft
        {
            Key = column.Key,
            Name = column.Name,
            Content = column.Content,
            Font = Text(column.Font),
            Align = column.Align,
            Translation = column.Translation,
            Table1 = column.Table1,
            Table2 = column.Table2,
            Row = Text(column.Row),
            Start = Text(column.Start),
            End = Text(column.End)
        };
    }

    /// <summary>Udaj suboru (neplatne cislo je 0).</summary>
    public CatalogColumn To() => new(Key, Content, Number(Row) ?? 0, Number(Start) ?? 0, Number(End) ?? 0, Align)
    {
        Name = Name,
        Font = Number(Font) ?? 0,
        Translation = Translation,
        Table1 = Table1,
        Table2 = Table2
    };

    /// <summary>Cislo z textu ako ho cita XmlSerializer servera; <see langword="null" />, ak text nie je cele cislo.</summary>
    public static int? Number(string text) =>
        int.TryParse(text.Trim(), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var number) ? number : null;

    private static string Text(int number) => number.ToString(CultureInfo.InvariantCulture);
}
