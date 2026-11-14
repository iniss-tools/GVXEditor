using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Stations.Drafts;

/// <summary>Jazyky hlasenia vlaku (Foreign.xml).</summary>
internal sealed class LanguagesDraft : RecordDraft
{
    /// <summary>Kluce jazykov oddelene medzerou; <c>1</c> = vsetky jazyky banky.</summary>
    public string Languages { get; set; } = "";

    /// <summary>Koncept zaznamu.</summary>
    public static LanguagesDraft Of(TrainLanguages languages) => new()
    {
        Validity = languages.Validity,
        EditedForDay = languages.EditedForDay,
        Languages = string.Join(' ', languages.Languages)
    };

    /// <summary>Zaznam pre cislo vlaku.</summary>
    public TrainLanguages To(string number) => new(number, Validity, Items(Languages)) { EditedForDay = EditedForDay };
}
