using System.Diagnostics;
using System.Threading.Channels;

namespace PipesTester;

public class Node
{
    public Node(string _ConnectionID) {
        ConnectionID = _ConnectionID;
    }

    private long TotalSentMessages = 0;
    private long TotalReceivedMessages = 0;

    public Channel<Message<string>> Connection { get; internal set; }

    public string ConnectionID { get; internal set; }

    public Task StartReading() => Task.Run(ReceiveLoop);

    public Task StartWriting() => Task.Run(WriteLoop);
    
    private async Task ReceiveLoop() {

        int ReceivedMessagesCount = 0;
        
        if (OnReceive is null)
        { return; }

        Task T = Connection.Reader.Completion;

        while (!T.IsCompleted)
        {
            Message<string> Received;

            try
            { Received = await Connection.Reader.ReadAsync(); }
            catch (ChannelClosedException exc)
            {
                Debug.WriteLine("Channel was closed on read."); 
                continue;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
            
            ReceivedMessagesCount++;
            
            OnReceive(Received);
        }

        TotalReceivedMessages += ReceivedMessagesCount;
    }

    private async Task WriteLoop() {
        int SentMessageCount = 0;
        
        if (Writer is null)
        { return; }

        for (int I = 0; I < 110; I++)
        {
            SentMessageCount++;

            Context CTX = new Context(SentMessageCount, DateTime.Now);
            
            Console.WriteLine(CTX.ToString());
            
            Message<string> ToWrite = Writer(CTX);

            await Connection.Writer.WriteAsync(ToWrite);
        }
        
        Connection.Writer.Complete();
        TotalSentMessages += SentMessageCount;
    }
    
    public Action<Message<string>>? OnReceive { get; set; }
    
    public Func<Context, Message<string>>? Writer { get; set; }
}
