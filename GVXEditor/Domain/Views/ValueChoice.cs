namespace GVXEditor.Domain.Views;

/// <summary>
/// Hodnota na vyber: kluc, ktory sa zapisuje do suboru, a text pre obsluhu.
/// </summary>
/// <param name="Key">kluc ako v subore; prazdny = atribut chyba</param>
/// <param name="Text">text v zozname</param>
internal sealed record ValueChoice(string Key, string Text)
{
    /// <inheritdoc />
    public override string ToString() => Text;
}
