namespace GVXEditor.Domain.Rules;

/// <summary>
/// Zistenie kontroly nastavenia grafikonu, stanice alebo hlaseni obsluhy.
/// </summary>
/// <param name="IsError">chyba - nastavenie sa neda potvrdit; inak upozornenie</param>
/// <param name="Message">text pre obsluhu</param>
internal sealed record SetupProblem(bool IsError, string Message);
