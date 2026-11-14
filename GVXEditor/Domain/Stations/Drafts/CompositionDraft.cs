using System.Globalization;
using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Stations.Drafts;

/// <summary>Radenie vlaku (Razeni.xml): postupnost zvukov.</summary>
internal sealed class CompositionDraft : RecordDraft
{
    // povod textu z klienta (editvl) sa v tabulke neupravuje, ostane pri zazname
    private CompositionOrigin? _origin;

    /// <summary>Cislo cielovej stanice (<c>est</c>); prazdne = kazdy ciel.</summary>
    public string EndStation { get; set; } = "";

    /// <summary>Typy hlasenia (<c>rep</c>).</summary>
    public string Reports { get; set; } = "";

    /// <summary>Zvuky oddelene medzerou v poradi prehravania.</summary>
    public string Sounds { get; set; } = "";

    /// <summary>Koncept zaznamu.</summary>
    public static CompositionDraft Of(TrainComposition composition) => new()
    {
        Validity = composition.Validity,
        EditedForDay = composition.EditedForDay,
        EndStation = composition.EndStation == 0 ? "" : composition.EndStation.ToString(CultureInfo.InvariantCulture),
        Reports = composition.Reports,
        Sounds = string.Join(' ', composition.Sounds),
        _origin = composition.Origin
    };

    /// <summary>Cislo cielovej stanice; <see langword="null" />, ak text nie je cislo.</summary>
    public int? EndStationNumber => EndStation.Trim().Length == 0 ? 0 :
        int.TryParse(EndStation.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out var station) ? station : null;

    /// <summary>Zaznam pre cislo vlaku.</summary>
    public TrainComposition To(string number) =>
        new(number, EndStationNumber ?? 0, Reports, Validity, Items(Sounds)) { EditedForDay = EditedForDay, Origin = _origin };
}
