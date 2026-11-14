namespace GVXEditor.Domain.Rules;

/// <summary>
/// Zistenie kontroly vlaku.
/// </summary>
/// <param name="IsError">chyba - vlak sa neda ulozit; inak upozornenie</param>
/// <param name="Variant">poradie varianty od 0; -1 = cely vlak</param>
/// <param name="Stop">poradie bodu trasy od 0; -1 = cela varianta</param>
/// <param name="Message">text pre obsluhu</param>
internal sealed record TrainProblem(bool IsError, int Variant, int Stop, string Message);
