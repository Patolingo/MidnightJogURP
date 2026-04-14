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

    [Output, HideInInspector] public DialoguePort choiceOutput;

    public string identificationString;
    public LocalizedString whoIsTalking;
    public LocalizedString content;

    public float delayToStart;
    public float delayToEnd;

    public Message OnEnterLine;
    public Message OnExitLine;

    [SerializeField] public List<LocalizedString> choiceContents = new List<LocalizedString>();
    [SerializeField] public List<Message> onSelectedCallbacks = new List<Message>();

    public bool HasChoices => choiceContents.Count > 0;

    protected override void Process() { }

    [CustomPortBehavior(nameof(output))]
    IEnumerable<PortData> GetOutputPort(List<SerializableEdge> edges)
    {
        // só mostra a saída simples se não tiver choices
        if (!HasChoices)
        {
            yield return new PortData
            {
                displayName = "Out",
                displayType = typeof(DialoguePort),
                identifier = "output",
            };
        }
    }

    [CustomPortBehavior(nameof(choiceOutput))]
    IEnumerable<PortData> GetChoicePorts(List<SerializableEdge> edges)
    {
        for (int i = 0; i < choiceContents.Count; i++)
        {
            var key = choiceContents[i].TableEntryReference.Key;
            yield return new PortData
            {
                displayName = !string.IsNullOrEmpty(key) ? key : $"Choice {i}",
                displayType = typeof(DialoguePort),
                identifier = $"choice_{i}",
            };
        }
    }
    
    public string[] GetAnswers()
    {
        string[] answers = new string[choiceContents.Count];

        for (int i = 0; i < answers.Length; i++)
        {
            answers[i] = choiceContents[i].GetLocalizedString();
        }

        return answers;
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