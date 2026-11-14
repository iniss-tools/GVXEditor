using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Stations.Drafts;

/// <summary>Kolaj vlaku (Positions.xml).</summary>
internal sealed class PositionDraft : RecordDraft
{
    /// <summary>Kluc kolaje pri prichode.</summary>
    public string Arrival { get; set; } = "";

    /// <summary>Kluc kolaje pri odchode; prazdny = kolaj prichodu.</summary>
    public string Departure { get; set; } = "";

    /// <summary>Koncept zaznamu.</summary>
    public static PositionDraft Of(TrainPosition position) => new()
    {
        Validity = position.Validity,
        EditedForDay = position.EditedForDay,
        Arrival = position.ArrivalTrack,
        Departure = position.DepartureTrack == position.ArrivalTrack ? "" : position.DepartureTrack
    };

    /// <summary>Zaznam pre cislo vlaku.</summary>
    public TrainPosition To(string number)
    {
        // text sa nemeni (ani medzery) - zaznam bez upravy musi ostat po znakoch rovnaky
        return new TrainPosition(number, Arrival, Departure.Length > 0 ? Departure : Arrival, Validity) { EditedForDay = EditedForDay };
    }
}
