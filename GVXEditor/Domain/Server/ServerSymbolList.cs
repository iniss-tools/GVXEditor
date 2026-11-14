using System.Text.Json.Serialization;

namespace GVXEditor.Domain.Server;

/// <summary>
/// Vysledok mosta s prepinacom <c>--symbols</c>.
/// </summary>
internal sealed record ServerSymbolList
{
    /// <summary>Mena servera.</summary>
    [JsonPropertyName("symbols")]
    public IReadOnlyList<ServerSymbol> Symbols { get; init; } = [];
}
