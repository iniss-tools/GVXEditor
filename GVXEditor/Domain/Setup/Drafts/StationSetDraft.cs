using System.ComponentModel;
using ToolsCore.Iniss2.Tables;

namespace GVXEditor.Domain.Setup.Drafts;

/// <summary>Sada textov podla stanice (TTStanice.xml).</summary>
internal sealed class StationSetDraft : TextSetDraft
{
    /// <summary>Texty pre stanice v poradi - plati prvy so stanicou.</summary>
    public BindingList<StationTextDraft> Texts { get; } = [];

    /// <summary>Koncept sady.</summary>
    public static StationSetDraft Of(StationTextSet set)
    {
        ArgumentNullException.ThrowIfNull(set);
        var draft = new StationSetDraft { Name = set.Name, Fonet = set.Fonet };
        foreach (var target in set.Targets)
            draft.Targets.Add(TargetDraft.Of(target));
        foreach (var text in set.Stations)
            draft.Texts.Add(StationTextDraft.Of(text));
        return draft;
    }

    /// <summary>Sada suboru.</summary>
    public StationTextSet To() => new(Name)
    {
        Fonet = Fonet,
        Targets = [.. Targets.Select(target => target.To())],
        Stations = [.. Texts.Select(text => text.To())]
    };
}
