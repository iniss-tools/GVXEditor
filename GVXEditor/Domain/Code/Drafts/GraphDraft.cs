using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Code.Drafts;

/// <summary>
/// Graf stavoveho diagramu v okne.
/// </summary>
internal sealed class GraphDraft
{
    /// <summary>Graf zo suboru; <see langword="null" /> = novy graf.</summary>
    public StateGraph? Original { get; private init; }

    /// <summary>Popis grafu.</summary>
    public string Description { get; set; } = "";

    /// <summary>Stavy v poradi - vlak postupuje v smere zoznamu.</summary>
    public List<NodeDraft> Nodes { get; } = [];

    /// <summary>Koncept grafu.</summary>
    public static GraphDraft Of(StateGraph graph)
    {
        ArgumentNullException.ThrowIfNull(graph);
        var draft = new GraphDraft { Original = graph, Description = graph.Description };
        draft.Nodes.AddRange(graph.Nodes.Select(NodeDraft.Of));
        return draft;
    }

    /// <summary>Graf suboru; bez zmeny vrati povodny graf.</summary>
    public StateGraph To()
    {
        var nodes = Nodes.Select(node => node.To()).ToList();
        if (Original is { } original && original.Description == Description && original.Nodes.Count == nodes.Count &&
            original.Nodes.Zip(nodes).All(pair => ReferenceEquals(pair.First, pair.Second)))
            return original;
        return new StateGraph(Description) { Nodes = nodes };
    }
}
