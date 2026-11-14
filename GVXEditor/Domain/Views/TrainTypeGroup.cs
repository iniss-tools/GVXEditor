namespace GVXEditor.Domain.Views;

/// <summary>
/// Skupina druhu vlaku pre farby stlpca Druh v zozname vlakov (rovnake skupiny ako stlpec typu v GVDEditore).
/// </summary>
internal enum TrainTypeGroup
{
    /// <summary>Osobne a ostatne vlaky.</summary>
    Os,

    /// <summary>Rychliky.</summary>
    R,

    /// <summary>Vlaky vyssej kvality (EC, IC, RJ, EN...).</summary>
    X,

    /// <summary>Sluzobne a nakladne vlaky.</summary>
    Sl
}
