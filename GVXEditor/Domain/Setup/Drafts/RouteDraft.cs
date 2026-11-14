using System.ComponentModel;
using ToolsCore.Iniss2.Tables;

namespace GVXEditor.Domain.Setup.Drafts;

/// <summary>Text pre trasu vlaku (TTTrasy.xml).</summary>
internal sealed class RouteDraft
{
    /// <summary>Text, casti oddelene znakom #.</summary>
    public string Text { get; set; } = "";

    /// <summary>Cislo pisma; prazdne = pismo predlohy.</summary>
    public string Font { get; set; } = "";

    /// <summary>Stanice trasy v poradi jazdy, priznak = stanica je vybrana do textu.</summary>
    public BindingList<StationRefDraft> Stations { get; } = [];

    /// <summary>Koncept textu.</summary>
    public static RouteDraft Of(RouteText route)
    {
        ArgumentNullException.ThrowIfNull(route);
        var draft = new RouteDraft { Text = route.Text, Font = SetupText.Font(route.Font) };
        foreach (var station in route.Stations)
            draft.Stations.Add(StationRefDraft.Of(station.Station, station.Selected));
        return draft;
    }

    /// <summary>Text suboru.</summary>
    public RouteText To() => new(Text, SetupText.Font(Font) ?? TrainTextSet.NoFont)
    {
        Stations = [.. Stations.Select(station => new RouteStation(station.Number ?? 0, station.Flag))]
    };
}
