namespace GVXEditor.Domain.Boards.Drafts;

/// <summary>
/// Kolaj casti tabule v okne: kluc z PositionsDef a priorita ako text (prazdna = bez priority).
/// </summary>
internal sealed class TrackRefDraft
{
    /// <summary>Kluc kolaje.</summary>
    public string Key { get; set; } = "";

    /// <summary>Priorita ako text; vacsie cislo ma prednost.</summary>
    public string Priority { get; set; } = "";
}
