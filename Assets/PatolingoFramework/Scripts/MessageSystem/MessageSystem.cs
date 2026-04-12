using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-99)]
public class MessageSystem : PersistentSingleton<MessageSystem>
{

    public bool debugMessageSystem;

    private List<IMessageListener> messageListeners = new List<IMessageListener>();

    private MessageListener printListener;

    private void Start()
    {
        printListener = new MessageListener(new Message("print", ""), false, msg => ShowDebug(msg.content));

#if !DEBUG
        debugMessageSystem = false;
#endif

        ShowDebug("Started!");
    }

    #region Message Calling
    private void Internal_CallMessage(Message message)
    {
        ShowDebug("Calling Message " + message.ToString());

        if (messageListeners == null || messageListeners.Count == 0) return;

        for (int i = 0; i < messageListeners.Count; i++)
        {
            messageListeners[i].Listen(message);
        }
    }

    public static void CallMessage(Message message) => Instance?.Internal_CallMessage(message);
    public static void Call(Message message) => CallMessage(message);
    public static void CallMessage(string key, string content) => Instance?.Internal_CallMessage(new Message(key, content));
    public static void Call(string key, string content) => CallMessage(key, content);
    #endregion

    #region Listener Handling
    private void Internal_AddListener(IMessageListener listener)
    {
        if (messageListeners == null) messageListeners = new List<IMessageListener>();

        if (messageListeners.Contains(listener)) return;

        ShowDebug("Listener Added");

        messageListeners.Add(listener);
    }
    private void Internal_RemoveListener(IMessageListener listener)
    {
        if (messageListeners == null) return;

        if(messageListeners.Contains(listener))
        {
            ShowDebug("Listener Removed");
            messageListeners.Remove(listener);
        }
    }


    public static void AddListener(IMessageListener listener) => Instance?.Internal_AddListener(listener);
    public static void RemoveListener(IMessageListener listener) => Instance?.Internal_RemoveListener(listener);
    #endregion

    private void ShowDebug(string message)
    {
        if(debugMessageSystem)
            Debug.Log("[MessageSystem] " + message);
    }
}


[System.Serializable]
public struct Message
{
    public string key;
    public string content;

    public static Message Empty => new Message("", "");

    public Message(string key, string content)
    {
        this.key = key;
        this.content = content;
    }

    public void Call()
    {
        MessageSystem.Call(this);
    }

    public override string ToString()
    {
        return key + " : " + content;
    }
}
