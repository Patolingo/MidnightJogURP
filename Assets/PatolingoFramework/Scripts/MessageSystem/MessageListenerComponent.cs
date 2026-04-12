using UnityEngine;
using UnityEngine.Events;

public class MessageListenerComponent : MonoBehaviour
{
    public Message waitingMessage;
    public bool removeOnMessageReceived;
    public UnityEvent<Message> onReceiveMessage;

    private MessageListener listener;

    private void Start()
    {
        listener = new MessageListener(waitingMessage, removeOnMessageReceived, onReceiveMessage.Invoke);
    }

    private void OnValidate()
    {
#if DEBUG
        listener?.UpdateMessage(waitingMessage);
#endif
    }

    private void OnDestroy()
    {
        listener?.Dispose();
    }

}