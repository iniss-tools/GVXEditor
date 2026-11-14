using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Stations.Drafts;

/// <summary>Dodatok vlaku (Dodatky.xml).</summary>
internal sealed class AdditionDraft : RecordDraft
{
    /// <summary>Kluc dodatku: nazov hlasenia v LogText alebo cislo hlasenia ConstReports.</summary>
    public string Key { get; set; } = "";

    /// <summary>Typy hlasenia (<c>rep</c>); prazdne = vsetky, <c>-</c> = ziadny.</summary>
    public string Reports { get; set; } = "";

    /// <summary>Koncept zaznamu.</summary>
    public static AdditionDraft Of(TrainAddition addition) => new()
    {
        Validity = addition.Validity,
        EditedForDay = addition.EditedForDay,
        Key = addition.Key,
        Reports = addition.Reports
    };

    /// <summary>Zaznam pre cislo vlaku.</summary>
    public TrainAddition To(string number) => new(number, Key, Reports, Validity) { EditedForDay = EditedForDay };
}
