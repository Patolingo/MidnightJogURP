using GraphProcessor;
using System.Linq;
using UnityEditor.Experimental.GraphView;

public class DialogueGraphExecutor
{
    private DialogueGraph graph;
    private BaseNode currentNode;

    public DialogueGraphExecutor(DialogueGraph graph)
    {
        this.graph = graph;
    }

    public void Start()
    {
        // começa no primeiro LineNode ou num StartNode dedicado
        currentNode = graph.nodes.Find(n => n is StartNode);
    }

    public BaseNode GetCurrent() => currentNode;

    public void Advance(int portIndex = 0)
    {
        if (currentNode == null) return;

        // pega as conexões de saída do nó atual
        var outputEdges = graph.edges
            .Where(e => e.outputNode == currentNode)
            .ToList();

        if (outputEdges.Count == 0)
        {
            currentNode = null; // fim do diálogo
            return;
        }

        // portIndex usado pra BranchNode escolher qual saída seguir
        var edge = outputEdges.Count > portIndex
            ? outputEdges[portIndex]
            : outputEdges[0];

        currentNode = edge.inputNode;
    }
}