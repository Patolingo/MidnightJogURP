using UnityEngine;

public class ConversationHandler : MonoBehaviour
{
    public ConversationData conversationData;

    public void StartConversation()
    {
        DialogueManager.Instance.StartConversation(conversationData);
    }
}
