using System.Globalization;
using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Stations.Drafts;

/// <summary>Stanice v hlaseni vlaku (StaHlasVl.xml): cisla stanic oddelene medzerou.</summary>
internal sealed class ReportStationsDraft : RecordDraft
{
    /// <summary>Kratke hlasenie, stanice zo smeru (<c>b</c>).</summary>
    public string ShortFrom { get; set; } = "";

    /// <summary>Dlhe hlasenie, stanice zo smeru (<c>c</c>).</summary>
    public string LongFrom { get; set; } = "";

    /// <summary>Kratke hlasenie, stanice do smeru (<c>d</c>).</summary>
    public string ShortTo { get; set; } = "";

    /// <summary>Dlhe hlasenie, stanice do smeru (<c>e</c>).</summary>
    public string LongTo { get; set; } = "";

    /// <summary>Koncept zaznamu.</summary>
    public static ReportStationsDraft Of(TrainReportStations stations) => new()
    {
        Validity = stations.Validity,
        EditedForDay = stations.EditedForDay,
        ShortFrom = Text(stations.ShortFrom),
        LongFrom = Text(stations.LongFrom),
        ShortTo = Text(stations.ShortTo),
        LongTo = Text(stations.LongTo)
    };

    /// <summary>Vsetky polozky su cisla stanic.</summary>
    public bool IsValid => new[] { ShortFrom, LongFrom, ShortTo, LongTo }.SelectMany(Items).All(item => Number(item) is not null);

    /// <summary>Zaznam pre cislo vlaku; polozku, ktora nie je cislo, vynecha.</summary>
    public TrainReportStations To(string number) => new(number, Validity)
    {
        EditedForDay = EditedForDay,
        ShortFrom = Numbers(ShortFrom),
        LongFrom = Numbers(LongFrom),
        ShortTo = Numbers(ShortTo),
        LongTo = Numbers(LongTo)
    };

    private static string Text(IReadOnlyList<int> stations) => string.Join(' ', stations.Select(station => station.ToString(CultureInfo.InvariantCulture)));

    private static List<int> Numbers(string text) => [.. Items(text).Select(Number).OfType<int>()];

    private static int? Number(string text) => int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var number) ? number : null;
}
