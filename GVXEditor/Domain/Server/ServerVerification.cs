using System.Text.Json.Serialization;

namespace GVXEditor.Domain.Server;

/// <summary>
/// Vysledok overenia kodom servera (JSON mosta IniSrvBridge).
/// </summary>
internal sealed record ServerVerification
{
    /// <summary>Server by sa spustil.</summary>
    [JsonPropertyName("ok")]
    public bool Ok { get; init; }

    /// <summary>Chyba, pre ktoru by sa server nespustil; <see langword="null" /> = ziadna.</summary>
    [JsonPropertyName("fatal")]
    public string? Fatal { get; init; }

    /// <summary>Krok nacitania, v ktorom chyba nastala (subor alebo preklad).</summary>
    [JsonPropertyName("stage")]
    public string? Stage { get; init; }

    /// <summary>Chyby prekladu kodu VB.NET.</summary>
    [JsonPropertyName("compileErrors")]
    public IReadOnlyList<ServerCompileError> CompileErrors { get; init; } = [];

    /// <summary>Zaznamy, ktore server pri nacitani zapisal do Error.log.</summary>
    [JsonPropertyName("log")]
    public IReadOnlyList<string> Log { get; init; } = [];

    /// <summary>Trvanie nacitania v ms.</summary>
    [JsonPropertyName("milliseconds")]
    public long Milliseconds { get; init; }

    /// <summary>Pocet chyb, pre ktore by sa server nespustil.</summary>
    public int ErrorCount => (Fatal is null ? 0 : 1) + CompileErrors.Count;
}
