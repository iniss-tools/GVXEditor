using GVXEditor.Domain.Code;

namespace GVXEditor.UI.Code;

/// <summary>
/// Vysledok prekladu kodu mostom.
/// </summary>
/// <param name="Errors">chyby a varovania prekladu s miestom</param>
/// <param name="Fatal">chyba nacitania, pre ktoru sa preklad nedokoncil; <see langword="null" /> = ziadna</param>
/// <param name="Stage">krok, v ktorom nastala <paramref name="Fatal" /></param>
/// <param name="Failure">most zlyhal (text pre obsluhu); <see langword="null" /> = preklad prebehol</param>
internal sealed record CodeCheckResult(IReadOnlyList<CodeErrorPlace> Errors, string? Fatal, string? Stage, string? Failure);
