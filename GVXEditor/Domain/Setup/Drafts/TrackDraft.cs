using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Setup.Drafts;

/// <summary>Kolaj stanice (PositionsDef.xml).</summary>
internal sealed class TrackDraft
{
    /// <summary>Kluc kolaje, podla neho sa na kolaj odkazuju vlaky.</summary>
    public string Key { get; set; } = "";

    /// <summary>Nazov pre obsluhu.</summary>
    public string Name { get; set; } = "";

    /// <summary>Sektor.</summary>
    public string Sector { get; set; } = "";

    /// <summary>Text kolaje na tabuliach.</summary>
    public string TrackText { get; set; } = "";

    /// <summary>Text sektora na tabuliach.</summary>
    public string SectorText { get; set; } = "";

    /// <summary>Text nastupista na tabuliach.</summary>
    public string PlatformText { get; set; } = "";

    /// <summary>Text nastupista a kolaje na tabuliach.</summary>
    public string PlatformTrackText { get; set; } = "";

    /// <summary>Nahradny text kolaje na tabuliach.</summary>
    public string AltTrackText { get; set; } = "";

    /// <summary>Zvuk kolaje.</summary>
    public string TrackSound { get; set; } = "";

    /// <summary>Zvuk nastupista.</summary>
    public string PlatformSound { get; set; } = "";

    /// <summary>Zvuk sektora.</summary>
    public string SectorSound { get; set; } = "";

    /// <summary>Oddelovac v ponuke obsluhy, nie kolaj.</summary>
    public bool Separator { get; set; }

    /// <summary>Spolocna hodnota, nie skutocna kolaj (napriklad zruseny vlak).</summary>
    public bool Common { get; set; }

    /// <summary>Bez hlasenia.</summary>
    public bool NoAudio { get; set; }

    /// <summary>Priznak ALTERTRAN.</summary>
    public bool AlterTran { get; set; }

    /// <summary>Koncept kolaje.</summary>
    public static TrackDraft Of(TrackDefinition track)
    {
        ArgumentNullException.ThrowIfNull(track);
        return new TrackDraft
        {
            Key = track.Key,
            Name = track.Name,
            Sector = track.Sector,
            TrackText = track.TrackText,
            SectorText = track.SectorText,
            PlatformText = track.PlatformText,
            PlatformTrackText = track.PlatformTrackText,
            AltTrackText = track.AltTrackText,
            TrackSound = track.TrackSound,
            PlatformSound = track.PlatformSound,
            SectorSound = track.SectorSound,
            Separator = track.Flags.HasFlag(TrackFlags.Separator),
            Common = track.Flags.HasFlag(TrackFlags.Common),
            NoAudio = track.Flags.HasFlag(TrackFlags.NoAudio),
            AlterTran = track.Flags.HasFlag(TrackFlags.AlterTran)
        };
    }

    /// <summary>Kolaj suboru; text sa nemeni (ani medzery).</summary>
    public TrackDefinition To() => new()
    {
        Key = Key,
        Name = Name,
        Sector = Sector,
        TrackText = TrackText,
        SectorText = SectorText,
        PlatformText = PlatformText,
        PlatformTrackText = PlatformTrackText,
        AltTrackText = AltTrackText,
        TrackSound = TrackSound,
        PlatformSound = PlatformSound,
        SectorSound = SectorSound,
        Flags = (Separator ? TrackFlags.Separator : 0) | (Common ? TrackFlags.Common : 0) | (NoAudio ? TrackFlags.NoAudio : 0) |
                (AlterTran ? TrackFlags.AlterTran : 0)
    };
}
