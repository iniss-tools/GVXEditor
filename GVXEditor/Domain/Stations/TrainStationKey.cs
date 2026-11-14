using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Stations;

/// <summary>
/// Vlak v stanici grafikonu: podla cisla pri odchode server hlada jeho zaznamy v suboroch stanice.
/// </summary>
/// <param name="StationIndex">poradie stanice v grafikone</param>
/// <param name="Station">stanica z DirList.xml</param>
/// <param name="Number">cislo vlaku pri odchode zo stanice</param>
internal sealed record TrainStationKey(int StationIndex, DirListItem Station, string Number)
{
    /// <summary>Stanica a cislo pre vyber v zozname.</summary>
    public string Label => $"{Station.Directory} ({Number})";

    /// <summary>Kluc sam - hodnota stlpca vyberu stanice v tabulke.</summary>
    public TrainStationKey Self => this;
}
