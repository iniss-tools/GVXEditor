using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Stations.Drafts;

/// <summary>Vyluka vlaku (Vyluky.xml) - vlak ju ma pod jednym cislom v celom grafikone.</summary>
internal sealed class LockoutDraft : NoticeDraft
{
    /// <summary>Koncept zaznamu.</summary>
    public static LockoutDraft Of(TrainLockout lockout) => new()
    {
        Validity = lockout.Validity,
        EditedForDay = lockout.EditedForDay,
        Key = lockout.Key,
        From = Text(lockout.From),
        To = Text(lockout.To)
    };

    /// <summary>Zaznam pre cislo vlaku; datum, ktory sa neda precitat, ostane bez datumu.</summary>
    public TrainLockout ToRecord(string number) =>
        new(number, Key, Date(From) ?? DateOnly.MinValue, Date(To) ?? DateOnly.MinValue, Validity) { EditedForDay = EditedForDay };
}
