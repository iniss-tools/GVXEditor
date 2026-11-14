using System.Text.Json.Serialization;

namespace GVXEditor.Domain.Server;

/// <summary>
/// Chyba prekladu kodu VB.NET serverom (aj varovanie kompilatora server zastavi).
/// </summary>
internal sealed record ServerCompileError
{
    /// <summary>Co sa prekladalo: statedgm.xml, TabTab, Tables.xml alebo LogText.vb.</summary>
    [JsonPropertyName("source")]
    public string Source { get; init; } = "";

    /// <summary>Riadok (u vyrazov riadok vyrazu).</summary>
    [JsonPropertyName("line")]
    public int Line { get; init; }

    /// <summary>Stlpec.</summary>
    [JsonPropertyName("column")]
    public int Column { get; init; }

    /// <summary>Cislo chyby kompilatora, napr. BC30451.</summary>
    [JsonPropertyName("number")]
    public string Number { get; init; } = "";

    /// <summary>Text chyby.</summary>
    [JsonPropertyName("message")]
    public string Message { get; init; } = "";

    /// <summary>Kde vyraz je (subor, stanica, graf, uzol), ak ho server uvadza.</summary>
    [JsonPropertyName("description")]
    public string Description { get; init; } = "";

    /// <summary>Kod vyrazu.</summary>
    [JsonPropertyName("code")]
    public string Code { get; init; } = "";
}
