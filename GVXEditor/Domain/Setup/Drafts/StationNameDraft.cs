using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Setup.Drafts;

/// <summary>Nazov stanice grafikonu (GV.stanice.XML).</summary>
internal sealed class StationNameDraft
{
    /// <summary>Cislo stanice ako text v tabulke.</summary>
    public string Station { get; set; } = "";

    /// <summary>Nazov stanice.</summary>
    public string Name { get; set; } = "";

    /// <summary>Cislo stanice; <see langword="null" />, ak text nie je cislo.</summary>
    public int? Number => SetupText.Station(Station);

    /// <summary>Koncept nazvu.</summary>
    public static StationNameDraft Of(GvStation station)
    {
        ArgumentNullException.ThrowIfNull(station);
        return new StationNameDraft { Station = SetupText.Number(station.Number), Name = station.Name };
    }

    /// <summary>Stanica suboru.</summary>
    public GvStation To() => new(Number ?? 0, Name);
}
