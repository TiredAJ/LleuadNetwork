using System.Threading.Channels;

namespace PipesTester;

public class Node
{
    public Node(string _ConnectionID) {
        ConnectionID = _ConnectionID;
    }

    private int SentMessageCount = 0;
    private int ReceivedMessageCount = 0;

    public Channel<Message<string>> Connection { get; internal set; }

    public string ConnectionID { get; internal set; }

    public void StartReading() => Task.Run(ReceiveLoop);

    public void StartWriting() => Task.Run(WriteLoop);
    
    private async Task ReceiveLoop() {

        if (OnReceive is null)
        { return; }

        Task T = Connection.Reader.Completion;
        
        while (!T.IsCompleted)
        {
            Message<string> Received = await Connection.Reader.ReadAsync();

            Interlocked.Increment(ref ReceivedMessageCount);
            
            OnReceive(Received);
        }
    }

    private async Task WriteLoop() {
        if (Writer is null)
        { return; }

        for (int I = 0; I < 110; I++)
        {
            Console.WriteLine(SentMessageCount);
            
            Message<string> ToWrite = Writer(SentMessageCount);

            await Connection.Writer.WriteAsync(ToWrite);
            
            Interlocked.Increment(ref SentMessageCount);
        }
        
        Connection.Writer.Complete();
    }
    
    public Action<Message<string>>? OnReceive { get; set; }
    
    public Func<int, Message<string>>? Writer { get; set; }
}
