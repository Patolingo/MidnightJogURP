public interface IMessageCaller
{
    public Message Message { get; set; }
    
    public void Call(Message message);
    public void Call();
}
