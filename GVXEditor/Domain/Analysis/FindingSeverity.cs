namespace GVXEditor.Domain.Analysis;

/// <summary>
/// Zavaznost zistenia kontroly instalacie.
/// </summary>
internal enum FindingSeverity
{
    /// <summary>Server by sa nespustil alebo subor nenacital.</summary>
    Error,

    /// <summary>Server by s udajmi urobil nieco ine, nez obsluha caka.</summary>
    Warning,

    /// <summary>Na vedomie, napriklad zaznamy, ktore server nepouzije.</summary>
    Info
}
