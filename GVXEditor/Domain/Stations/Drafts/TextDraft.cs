using System.Globalization;
using ToolsCore.Iniss2.Tables;

namespace GVXEditor.Domain.Stations.Drafts;

/// <summary>Text vlaku na tabule (TTVlaky.xml) v jednej sade stanice.</summary>
internal sealed class TextDraft : RecordDraft
{
    /// <summary>Nazov sady, do ktorej text patri.</summary>
    public string Set { get; set; } = "";

    /// <summary>Text pre udaj tabule.</summary>
    public string Text { get; set; } = "";

    /// <summary>Cislo pisma; prazdne = pismo predlohy.</summary>
    public string Font { get; set; } = "";

    /// <summary>Koncept zaznamu.</summary>
    public static TextDraft Of(string set, TrainText text) => new()
    {
        Validity = text.Validity,
        EditedForDay = text.EditedForDay,
        Set = set,
        Text = text.Text,
        Font = text.Font == TrainTextSet.NoFont ? "" : text.Font.ToString(CultureInfo.InvariantCulture)
    };

    /// <summary>Cislo pisma; <see langword="null" />, ak text nie je cislo.</summary>
    public int? FontNumber => Font.Trim().Length == 0 ? TrainTextSet.NoFont :
        int.TryParse(Font.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out var font) ? font : null;

    /// <summary>Zaznam pre cislo vlaku.</summary>
    public TrainText To(string number) => new(number, Validity, Text, FontNumber ?? TrainTextSet.NoFont) { EditedForDay = EditedForDay };
}
