namespace GVXEditor.Domain.Stations.Texts;

/// <summary>
/// Vygenerovany text vlaku v nahlade a co sa s nim pri zapise stane.
/// </summary>
/// <param name="Text">vygenerovany text</param>
/// <param name="Current">text vlaku v sade (viac zaznamov oddelenych |), inak text, ktory by server ukazal bez neho</param>
/// <param name="Change">co sa so zaznamom stane</param>
/// <param name="Changes">zapis meni sadu</param>
internal sealed record TextPreviewLine(GeneratedText Text, string Current, GeneratedTextChange Change, bool Changes);
