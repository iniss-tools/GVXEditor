namespace GVXEditor.Domain.Stations.Texts;

/// <summary>
/// Co sa generuje do sady textov vlakov (TTVlaky.xml) - ako texty na tabuliach v GVDEditore.
/// </summary>
internal enum GeneratedTextKind
{
    /// <summary>Cielova stanica vlaku (udaj <c>CilNazev</c>).</summary>
    Destination,

    /// <summary>Vychodzia stanica vlaku (udaj <c>VychoziNazev</c>).</summary>
    Origin,

    /// <summary>Stanice kratkeho hlasenia do smeru bez cielovej (udaj <c>PoNazev</c>).</summary>
    StationsTo,

    /// <summary>Stanice kratkeho hlasenia zo smeru bez vychodzej (udaj <c>PredNazev</c>).</summary>
    StationsFrom
}
