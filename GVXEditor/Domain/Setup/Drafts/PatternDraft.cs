using System.ComponentModel;
using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Setup.Drafts;

/// <summary>
/// Vzor stanic v hlaseni (StaHlasB.xml, StaHlasC.xml): stanice trasy vlaku v poradi a ktore z nich sa povedia.
/// </summary>
internal sealed class PatternDraft
{
    /// <summary>Stanice vzoru v poradi jazdy.</summary>
    public BindingList<StationRefDraft> Stops { get; } = [];

    /// <summary>Koncept vzoru.</summary>
    public static PatternDraft Of(IEnumerable<ReportPatternStop> stops)
    {
        var draft = new PatternDraft();
        foreach (var stop in stops)
            draft.Stops.Add(StationRefDraft.Of(stop.Station, stop.Reported));
        return draft;
    }

    /// <summary>Vzor suboru (stanice bez platneho cisla len po kontrole, ktora ich nepusti).</summary>
    public IReadOnlyList<ReportPatternStop> To() => [.. Stops.Select(stop => new ReportPatternStop(stop.Number ?? 0, stop.Flag))];
}
