using GraphProcessor;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[System.Serializable]
[CreateAssetMenu(fileName = "DialogueGraph", menuName = "Dialogue/Graph")]
public class DialogueGraph : BaseGraph
{
    public enum DialogueType { CONVERSATION, MONOLOGUE }
    public DialogueType dialogueType;
}

[System.Serializable]
[NodeMenuItem("Dialogue/Start")]
public class StartNode : BaseNode
{
    [Output] public DialoguePort output;
    public Message OnEnterConversation;

    protected override void Process() { }
}

[System.Serializable]
[NodeMenuItem("Dialogue/End")]
public class EndNode : BaseNode
{
    [Input] public DialoguePort input;
    public Message OnExitConversation;

    protected override void Process() { }
}

[System.Serializable]
[NodeMenuItem("Dialogue/Line")]
public class LineNode : BaseNode
{
    [Input] public DialoguePort input;
    [Output] public DialoguePort output;

    public LocalizedString whoIsTalking;
    public LocalizedString content;

    public float delayToStart;
    public float delayToEnd;

    public Message OnEnterLine;
    public Message OnExitLine;

    protected override void Process() { }
}

// QuestionLine virou um nó próprio
// cada porta de saída é uma escolha
[System.Serializable]
[NodeMenuItem("Dialogue/Branch")]
public class BranchNode : BaseNode
{
    [Input] public DialoguePort input;

    // campo fantasma — só existe pra o sistema de ports ter um fieldInfo pra referenciar
    [Output(name = "Choices")] public DialoguePort choiceOutput;

    [SerializeField] public List<LocalizedString> choiceContents = new List<LocalizedString>();

    protected override void Process() { }

    [CustomPortBehavior(nameof(choiceOutput))]
    IEnumerable<PortData> GetChoicePorts(List<SerializableEdge> edges)
    {
        for (int i = 0; i < choiceContents.Count; i++)
        {
            var key = choiceContents[i].TableEntryReference.Key;
            Debug.Log($"Choice {i} key: '{key}'");

            yield return new PortData
            {
                displayName = !string.IsNullOrEmpty(key) ? key : $"Choice {i}",
                displayType = typeof(DialoguePort),
                identifier = $"choice_{i}",
            };
        }
    }
}

// resultConversation do QuestionLine original
[System.Serializable]
[NodeMenuItem("Dialogue/SubGraph")]
public class SubGraphNode : BaseNode
{
    [Input] public DialoguePort input;
    [Output] public DialoguePort output;

    public DialogueGraph subGraph; // arrasta outro asset de grafo aqui

    protected override void Process() { }
}