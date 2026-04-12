using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "ConversationData", menuName = "Dialogue/Conversation")]
public class ConversationData : ScriptableObject
{
    public enum DialogueType
    {
        CONVERSATION,
        MONOLOGUE
    }

    public DialogueType dialogueType;

    [Header("Message Callbacks")]
    public Message OnEnterConversation = Message.Empty;
    public Message OnExitConversation = Message.Empty;

    [Space]

    public ConversationLine[] conversationLines;

    public int Length => conversationLines.Length;
    public ConversationLine GetLine(int index)
    {
        if (index < 0 || index >= conversationLines.Length) return null;

        return conversationLines[index];
    }
}

[System.Serializable]
public class ConversationLine
{
    public LocalizedString whoIsTalking;
    public LocalizedString content;

    [Space]
    [Header("Pauses")]
    public float delayToStart;
    public float delayToEnd;

    [Space]

    [Header("Message Callbacks")]
    public Message OnEnterLine = Message.Empty;
    public Message OnExitLine = Message.Empty;

    [Header("Question")]
    public QuestionLine[] answers;
}

[System.Serializable]
public class QuestionLine
{
    public LocalizedString questionLine;
    public ConversationData resultConversation;
    [Space]
    public Message OnSelected = Message.Empty;
}
