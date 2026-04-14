using GraphProcessor;
using System.Linq;

public class DialogueGraphExecutor
{
    private DialogueGraph graph;
    private BaseNode currentNode;

    public DialogueGraph Graph => graph;

    public DialogueGraphExecutor(DialogueGraph graph)
    {
        this.graph = graph;
    }

    public void Start()
    {
        currentNode = graph.nodes.Find(n => n is StartNode);
    }

    public BaseNode GetCurrent() => currentNode;


    public void Advance()
    {
        var edge = graph.edges.FirstOrDefault(e => e.outputNode == currentNode);
        currentNode = edge?.inputNode;
    }
    public void AdvanceFromChoice(int choiceIndex)
    {
        var edges = graph.edges
            .Where(e => e.outputNode == currentNode && e.outputPortIdentifier == $"choice_{choiceIndex}")
            .ToList();

        currentNode = edges.Count > 0 ? edges[0].inputNode : null;

    }
}