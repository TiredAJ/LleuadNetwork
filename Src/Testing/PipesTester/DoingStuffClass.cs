namespace PipesTester;

public class DoingStuffClass
{
    static public Message<string> Write(Context _CTX) {
        Thread.Sleep(50);
                                
        return new Message<string>("Reader Node",
                                   $"Hello!! Sent: {_CTX.MsgDateTime:O}",
                                   _CTX.MessageID);
    }

    static public void Read(Message<string> _M) {
        Console.WriteLine($"Message received for {_M._Address}, with ID {_M.MessageID} and payload {_M.Payload}");
    }
}
