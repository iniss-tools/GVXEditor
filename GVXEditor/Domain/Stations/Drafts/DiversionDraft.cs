using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Stations.Drafts;

/// <summary>Odklon vlaku (Odklony.xml) v jednej stanici - cislo je cislo vlaku pri odchode z nej.</summary>
internal sealed class DiversionDraft : NoticeDraft
{
    /// <summary>Stanica a cislo vlaku v nej.</summary>
    public TrainStationKey? Station { get; set; }

    /// <summary>Koncept zaznamu.</summary>
    public static DiversionDraft Of(TrainDiversion diversion, TrainStationKey station) => new()
    {
        Validity = diversion.Validity,
        EditedForDay = diversion.EditedForDay,
        Station = station,
        Key = diversion.Key,
        From = Text(diversion.From),
        To = Text(diversion.To)
    };

    /// <summary>Zaznam; datum, ktory sa neda precitat, ostane bez datumu.</summary>
    public TrainDiversion? ToRecord() => Station is { } station
        ? new TrainDiversion(station.Number, station.Station.Station, Key, Date(From) ?? DateOnly.MinValue, Date(To) ?? DateOnly.MinValue,
            Validity) { EditedForDay = EditedForDay }
        : null;
}
