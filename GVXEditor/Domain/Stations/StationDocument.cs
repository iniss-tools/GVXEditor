using GVXEditor.Domain.Boards;
using GVXEditor.Domain.Code;
using GVXEditor.Domain.Setup;
using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Stations;

/// <summary>
/// Stanica grafikonu: subory, ktore server pri uprave vlaku prepisuje, a nastavenie stanice, ktore len cita.
/// </summary>
/// <param name="Station">stanica z DirList.xml</param>
/// <param name="Files">subory stanice; <see langword="null" />, ak sa niektory neda nacitat</param>
/// <param name="Setup">nastavenie stanice (kolaje, typy hlaseni, vzory, texty tabul); <see langword="null" />, ak sa neda nacitat</param>
/// <param name="Error">preco sa subory nedaju nacitat; <see langword="null" /> = nacitane</param>
internal sealed record StationDocument(DirListItem Station, StationFiles? Files, StationSetup? Setup, string? Error)
{
    /// <summary>Preco sa nastavenie stanice neda nacitat.</summary>
    public string? SetupError { get; init; }

    /// <summary>Stavovy diagram a tabulky TabTab stanice; <see langword="null" />, ak sa nedaju nacitat.</summary>
    public StationCode? Code { get; init; }

    /// <summary>Preco sa kod stanice neda nacitat.</summary>
    public string? CodeError { get; init; }

    /// <summary>Tabule a katalogove predlohy stanice; <see langword="null" />, ak sa nedaju nacitat.</summary>
    public StationBoards? Boards { get; init; }

    /// <summary>Preco sa tabule stanice nedaju nacitat.</summary>
    public string? BoardsError { get; init; }

    /// <summary>Kolaje stanice z PositionsDef.xml.</summary>
    public IReadOnlyList<TrackDefinition> Tracks => Setup?.Tracks ?? [];
}
