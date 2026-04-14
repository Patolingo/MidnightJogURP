using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "ConversationData", menuName = "Dialogue/Conversation")]
public class ConversationData : ScriptableObject
{
    public DialogueGraph graph;

    public bool pausePlayerOnDialogue;
}