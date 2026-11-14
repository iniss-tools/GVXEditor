using GVXEditor.Domain.Analysis;
using Serilog;

namespace GVXEditor.UI.Main;

internal partial class FMain
{
    // prikaz Skontrolovat instalaciu: zobrazi panel kontroly a skontroluje hned
    private void ShowAnalysis()
    {
        if (_ctx.Workspace is { } workspace)
            Log.Information("Kontrola instalacie {DataDir}", workspace.DataDir);
        _dock.ShowPanel(_analysisPanel);
        RefreshAnalysis();
    }

    /// <summary>
    /// Prejde na grafikon a vlak zistenia.
    /// </summary>
    private void Navigate(Finding finding)
    {
        if (finding.Group < 0 || finding.Group >= tvData.Nodes.Count)
            return;

        tvData.SelectedNode = tvData.Nodes[finding.Group];
        if (finding.Train >= 0)
            SelectTrain(finding.Train);
        // panel kontroly moze byt v plavajucom okne
        Activate();
    }
}
