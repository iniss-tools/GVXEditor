using System.ComponentModel;
using ToolsCore.Iniss2.Tables;

namespace GVXEditor.Domain.Setup.Drafts;

/// <summary>Sada textov podla trasy (TTTrasy.xml).</summary>
internal sealed class RouteSetDraft : TextSetDraft
{
    /// <summary>Texty pre trasy v poradi - plati prvy, ktoreho stanice sa zhoduju s trasou vlaku.</summary>
    public BindingList<RouteDraft> Routes { get; } = [];

    /// <summary>Koncept sady.</summary>
    public static RouteSetDraft Of(RouteTextSet set)
    {
        ArgumentNullException.ThrowIfNull(set);
        var draft = new RouteSetDraft { Name = set.Name, Fonet = set.Fonet };
        foreach (var target in set.Targets)
            draft.Targets.Add(TargetDraft.Of(target));
        foreach (var route in set.Routes)
            draft.Routes.Add(RouteDraft.Of(route));
        return draft;
    }

    /// <summary>Sada suboru.</summary>
    public RouteTextSet To() => new(Name)
    {
        Fonet = Fonet,
        Targets = [.. Targets.Select(target => target.To())],
        Routes = [.. Routes.Select(route => route.To())]
    };
}
