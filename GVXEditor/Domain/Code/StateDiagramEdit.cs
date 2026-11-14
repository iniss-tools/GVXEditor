using GVXEditor.Domain.Code.Drafts;
using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Code;

/// <summary>
/// Stavovy diagram jednej stanice v okne Stavovy diagram.
/// </summary>
internal sealed class StateDiagramEdit
{
    private StateDiagramEdit(int index, StateDiagram? original)
    {
        Index = index;
        Original = original;
    }

    /// <summary>Poradie stanice v grafikone.</summary>
    public int Index { get; }

    /// <summary>Diagram zo suboru; <see langword="null" />, ak subor chyba.</summary>
    public StateDiagram? Original { get; }

    /// <summary>Vyraz cisla grafu (<c>indg</c>).</summary>
    public string GraphIndex { get; set; } = "";

    /// <summary>Grafy v poradi suboru.</summary>
    public List<GraphDraft> Graphs { get; } = [];

    /// <summary>
    /// Koncept diagramu stanice; chybajuci subor je prazdny diagram (vznikne az s obsahom).
    /// </summary>
    public static StateDiagramEdit Of(int index, StateDiagram? diagram)
    {
        var edit = new StateDiagramEdit(index, diagram) { GraphIndex = diagram?.GraphIndex ?? "" };
        edit.Graphs.AddRange(diagram?.Graphs.Select(GraphDraft.Of) ?? []);
        return edit;
    }

    /// <summary>
    /// Diagram z konceptu; bez zmeny vrati <see cref="Original" />, prazdny novy diagram je <see langword="null" />.
    /// </summary>
    public StateDiagram? Apply()
    {
        var graphs = Graphs.Select(graph => graph.To()).ToList();
        if (Original is { } original)
        {
            return original.GraphIndex == GraphIndex && original.Graphs.Count == graphs.Count && original.Graphs.Zip(graphs).All(pair => ReferenceEquals(pair.First, pair.Second))
                ? original
                : new StateDiagram(GraphIndex) { Graphs = graphs };
        }

        return GraphIndex.Length == 0 && graphs.Count == 0 ? null : new StateDiagram(GraphIndex) { Graphs = graphs };
    }
}
