namespace LleuadNetwork.Messages;

public class Message<T> : BaseMessage<T> where T : notnull
{
    protected Message(T _Payload) : base(_Payload)
    { }
}
