namespace GVXEditor.Domain;

/// <summary>
/// Vysledok ulozenia instalacie.
/// </summary>
/// <param name="Outcome">ako ulozenie dopadlo</param>
/// <param name="ChangedFiles">subory, ktore po nacitani zmenil iny program (pri <see cref="SaveOutcome.ChangedOnDisk" />)</param>
internal sealed record SaveResult(SaveOutcome Outcome, IReadOnlyList<string> ChangedFiles)
{
    /// <summary>Vysledok bez zmenenych suborov.</summary>
    public static SaveResult Of(SaveOutcome outcome) => new(outcome, []);
}
