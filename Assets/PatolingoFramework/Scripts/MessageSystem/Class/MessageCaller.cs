public class MessageCaller : IMessageCaller
{
    public Message Message { get; set; }

    public MessageCaller(Message message)
    {
        Message = message;
    }

    public void Call(Message message)
    {
        message.Call();
    }

    public void Call()
    {
        Message.Call();
    }
}
