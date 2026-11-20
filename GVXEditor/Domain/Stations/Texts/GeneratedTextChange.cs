namespace GVXEditor.Domain.Stations.Texts;

/// <summary>
/// Co sa pri generovani stane so zaznamom vlaku v sade textov.
/// </summary>
internal enum GeneratedTextChange
{
    /// <summary>Vlak dostane novy zaznam.</summary>
    Add,

    /// <summary>Zaznam vlaku v sade sa nahradi (len pri nahradeni vsetkych textov).</summary>
    Replace,

    /// <summary>Vlak ma v sade vlastny text a ten ostane.</summary>
    Keep,

    /// <summary>Vlak ma text podla trasy alebo stanice, doplnenie ho neprekryje.</summary>
    RouteOrStationText,

    /// <summary>Server ukaze rovnaky text aj bez zaznamu.</summary>
    SameWithout,

    /// <summary>Vygenerovany text je prazdny, nezapise sa.</summary>
    Empty
}
