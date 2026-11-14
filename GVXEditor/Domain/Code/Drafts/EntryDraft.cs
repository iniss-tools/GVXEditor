using System.Globalization;
using ToolsCore.Iniss2.Tables;

namespace GVXEditor.Domain.Code.Drafts;

/// <summary>
/// Polozka slovnika tabulky TabTab v okne: text a pismo kluca a hodnoty. Pismo ako text: prazdne = bez pisma (-1),
/// <c>*</c> = akekolvek pismo v kluci, pismo vstupu v hodnote (-2).
/// </summary>
internal sealed class EntryDraft
{
    /// <summary>Text kluca (pred prekodovanim).</summary>
    public string KeyText { get; set; } = "";

    /// <summary>Pismo kluca ako text.</summary>
    public string KeyFont { get; set; } = "*";

    /// <summary>Text hodnoty (po prekodovani).</summary>
    public string ValueText { get; set; } = "";

    /// <summary>Pismo hodnoty ako text.</summary>
    public string ValueFont { get; set; } = "*";

    /// <summary>Pismo kluca; <see langword="null" />, ak text nie je pismo.</summary>
    public int? KeyFontNumber => Font(KeyFont);

    /// <summary>Pismo hodnoty; <see langword="null" />, ak text nie je pismo.</summary>
    public int? ValueFontNumber => Font(ValueFont);

    /// <summary>Koncept polozky.</summary>
    public static EntryDraft Of(TabTabEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        return new EntryDraft { KeyText = entry.KeyText, KeyFont = Font(entry.KeyFont), ValueText = entry.ValueText, ValueFont = Font(entry.ValueFont) };
    }

    /// <summary>Polozka suboru.</summary>
    public TabTabEntry To() => new(KeyText, KeyFontNumber ?? TabTabTable.NoFont, ValueText, ValueFontNumber ?? TabTabTable.NoFont);

    /// <summary>Pismo ako text v tabulke okna.</summary>
    public static string Font(int font) => font switch
    {
        TabTabTable.NoFont => "",
        TabTabTable.AnyFont => "*",
        _ => font.ToString(CultureInfo.InvariantCulture)
    };

    /// <summary>Pismo z textu; <see langword="null" />, ak text nie je pismo.</summary>
    public static int? Font(string text) => text.Trim() switch
    {
        "" => TabTabTable.NoFont,
        "*" => TabTabTable.AnyFont,
        var value => int.TryParse(value, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var font) && font >= -2 ? font : null
    };
}
