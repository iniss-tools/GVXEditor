using System.Text.Json.Serialization;

namespace GVXEditor.Domain.Server;

/// <summary>
/// Meno z programu servera pre dopĺňanie v editore kodu.
/// </summary>
internal sealed record ServerSymbol
{
    /// <summary>Meno, ktore sa vlozi (pri vyctoch a pomocnych triedach s menom typu, napr. <c>TABLETYPE.PLATFORM</c>).</summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = "";

    /// <summary>Druh: <c>member</c> a <c>method</c> su cleny <c>VlakS</c> (za bodkou), <c>global</c> ostatne mena.</summary>
    [JsonPropertyName("kind")]
    public string Kind { get; init; } = "";

    /// <summary>Zapis s parametrami a typom vo VB.NET; pretazenia na samostatnych riadkoch.</summary>
    [JsonPropertyName("signature")]
    public string Signature { get; init; } = "";
}
