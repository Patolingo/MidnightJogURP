using UnityEngine;

public class MessageCallerComponent : MonoBehaviour
{
    public Message message;

    [ContextMenu("Call Message")]
    public void Call()
    {
        message.Call();
    }
}
