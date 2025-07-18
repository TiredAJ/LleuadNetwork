using System.Diagnostics;
using System.Threading.Channels;

namespace PipesTester;

using ReaderConn = ChannelReader<Message<string>>;
using WriterConn = ChannelWriter<Message<string>>;

public class Node
{
    public Node(string _ID) {
        ID = _ID;
    }
    public long TotalSentMessages => _TotalSentMessages;
    public long _TotalSentMessages = 0;
    public long TotalReceivedMessages => _TotalReceivedMessages;
    public long _TotalReceivedMessages = 0;
    
    public Dictionary<string, Connection> Connections { get; internal set; } = new(); 
    
    public string ID { get; internal set; }

    public Task StartNode(CancellationToken? _CT = null) {

        List<WriterConn> Writers = Connections.Values.Select(X => X.Output).ToList(); 
        List<ReaderConn> Readers = Connections.Values.Select(X => X.Input).ToList(); 
        
        return Task.WhenAll([ReceiveLoop(Readers, _CT), WriteLoop(Writers, _CT)]);
    }
    
    private async Task ReceiveLoop(List<ReaderConn> _Readers, CancellationToken? _CToken = null) {

        if (OnReceive is null)
        { return; }
        
        List<Task> ReadTasks = [];
        
        ReadTasks.AddRange(_Readers.Select(Conn => ConnReadLoop(Conn, _CToken)));

        await Task.WhenAll(ReadTasks);
    }

    private async Task ConnReadLoop(ReaderConn _ChannelReader, CancellationToken? _CToken = null) {

        int ReceivedMessagesCount = 0;
        
        Task T = _ChannelReader.Completion;
        
        while (!T.IsCompleted && (_CToken is not null && !_CToken.Value.IsCancellationRequested))
        {
            Message<string> Received;

            try
            { Received = await _ChannelReader.ReadAsync(); }
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

        Interlocked.Add(ref _TotalReceivedMessages, ReceivedMessagesCount);
        
        _TotalReceivedMessages += ReceivedMessagesCount;
    }

    private async Task WriteLoop(List<WriterConn> _Writers, CancellationToken? _CancelToken = null) {
        
        if (OnReceive is null)
        { return; }
        
        List<Task> WriteTasks = [];
        
        WriteTasks.AddRange(_Writers.Select(Conn => ConnWriterLoop(Conn, _CancelToken)));

        await Task.WhenAll(WriteTasks);
    }

    private async Task ConnWriterLoop(WriterConn _Writer, CancellationToken? _CToken = null) {
        int SentMessageCount = 0;
        
        for (int I = 0; I < 110; I++)
        {
            SentMessageCount++;

            Context CTX = new Context(SentMessageCount, DateTime.Now);
            
            Console.WriteLine(CTX.ToString());

            if (_CToken is null || _CToken.Value.IsCancellationRequested)
            { continue; }

            Message<string> ToWrite = Writer(CTX);

            await _Writer.WriteAsync(ToWrite);
        }
        
        _Writer.Complete();

        Interlocked.Add(ref _TotalSentMessages, SentMessageCount);
    }
    
    public Action<Message<string>>? OnReceive { get; set; }
    
    public Func<Context, Message<string>>? Writer { get; set; }
}
