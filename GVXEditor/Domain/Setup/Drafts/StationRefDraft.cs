namespace GVXEditor.Domain.Setup.Drafts;

/// <summary>
/// Stanica v zozname (vzor stanic v hlaseni, trasa textu tabul) s priznakom.
/// </summary>
internal sealed class StationRefDraft
{
    /// <summary>Cislo stanice ako text v tabulke.</summary>
    public string Station { get; set; } = "";

    /// <summary>Stanica sa povie (vzor hlasenia) alebo je vybrana do textu (trasa).</summary>
    public bool Flag { get; set; }

    /// <summary>Cislo stanice; <see langword="null" />, ak text nie je cislo.</summary>
    public int? Number => SetupText.Station(Station);

    /// <summary>Koncept stanice.</summary>
    public static StationRefDraft Of(int station, bool flag) => new() { Station = SetupText.Number(station), Flag = flag };

    /// <summary>Kopia.</summary>
    public StationRefDraft Clone() => new() { Station = Station, Flag = Flag };
}
