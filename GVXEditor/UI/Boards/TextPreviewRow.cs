namespace GVXEditor.UI.Boards;

/// <summary>
/// Riadok nahladu v okne Generovat texty tabul.
/// </summary>
/// <param name="Number">cislo vlaku pri odchode zo stanice</param>
/// <param name="Validity">datumova poznamka textu</param>
/// <param name="Text">vygenerovany text</param>
/// <param name="Current">text v sade, inak text, ktory by server ukazal bez neho</param>
/// <param name="Change">co sa so zaznamom stane</param>
internal sealed record TextPreviewRow(string Number, string Validity, string Text, string Current, string Change);
