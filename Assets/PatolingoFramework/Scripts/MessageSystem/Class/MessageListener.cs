using System;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class MessageListener : IMessageListener, IDisposable
{
    public Message waitingMessage;

    [Header("Config")]
    public bool removeOnReceiveMessage;


    [Header("Callback")]
    public UnityEvent<Message> OnMessageReceived;
    public event Action<Message> OnMessageListened;


    public MessageListener(Message message, bool removeOnReceiveMessage = false)
    {
        waitingMessage = message;
        this.removeOnReceiveMessage = removeOnReceiveMessage;

        MessageSystem.AddListener(this);
    }
    public MessageListener(Message message, bool removeOnReceiveMessage, Action<Message> onMessageReceived) : this(message, removeOnReceiveMessage)
    {
        this.OnMessageListened += onMessageReceived;
    }


    public void Listen(Message message)
    {
        if (waitingMessage.key.ToLower() != message.key.ToLower()) return;

        if(string.IsNullOrEmpty(waitingMessage.content) || waitingMessage.content.ToLower() == message.content.ToLower())
        {
            OnMessageListened?.Invoke(message);
            OnMessageReceived?.Invoke(message);

            if(removeOnReceiveMessage)
            {
                MessageSystem.RemoveListener(this);
            }
        }

    }

    public void UpdateMessage(Message message)
    {
        waitingMessage = message;
    }

    public void Dispose()
    {
        MessageSystem.RemoveListener(this);
    }
}
