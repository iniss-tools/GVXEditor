using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Setup.Drafts;

/// <summary>Stanica, ktora sa predvolene povie v hlaseni do smeru (Default.Stations.xml).</summary>
internal sealed class DefaultStationDraft
{
    /// <summary>Cislo stanice ako text v tabulke.</summary>
    public string Station { get; set; } = "";

    /// <summary>Nazov pre orientaciu v subore, server ho nepouziva.</summary>
    public string Name { get; set; } = "";

    /// <summary>Stanica sa povie v kratkom hlaseni.</summary>
    public bool Short { get; set; } = true;

    /// <summary>Stanica sa povie v dlhom hlaseni.</summary>
    public bool Long { get; set; } = true;

    /// <summary>Cislo stanice; <see langword="null" />, ak text nie je cislo.</summary>
    public int? Number => SetupText.Station(Station);

    /// <summary>Koncept stanice.</summary>
    public static DefaultStationDraft Of(DefaultStation station)
    {
        ArgumentNullException.ThrowIfNull(station);
        return new DefaultStationDraft { Station = SetupText.Number(station.Station), Name = station.Name, Short = station.Short, Long = station.Long };
    }

    /// <summary>Stanica suboru.</summary>
    public DefaultStation To() => new(Number ?? 0, Name) { Short = Short, Long = Long };
}
