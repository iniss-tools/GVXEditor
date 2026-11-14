using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Stations.Drafts;

/// <summary>Motorovy osobny vlak (MOS.xml): v dnoch poznamky dostane druh MOs.</summary>
internal sealed class MosDraft : RecordDraft
{
    /// <summary>Koncept zaznamu.</summary>
    public static MosDraft Of(MosTrain train) => new() { Validity = train.Validity };

    /// <summary>Zaznam pre cislo vlaku.</summary>
    public MosTrain To(string number) => new(number, Validity);
}
