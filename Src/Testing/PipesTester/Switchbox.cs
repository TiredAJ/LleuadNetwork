using System.Threading.Channels;

namespace PipesTester;

public class Switchbox
{
    private Dictionary<string, Channel<Message<string>>> Connections = new();
    private BoundedChannelOptions BCODefault;

    public Switchbox() {
        BCODefault = new BoundedChannelOptions(100) {
            AllowSynchronousContinuations = false,
            SingleReader = true,
            SingleWriter = true,
            FullMode = BoundedChannelFullMode.DropWrite
        };
    }

    public void Connect(Node _N) => _N.Connection = GetOrMakeConnection(_N.ConnectionID);

    private Channel<Message<string>> GetOrMakeConnection(string _ID) {
        if (Connections.TryGetValue(_ID, out Channel<Message<string>>? Connection))
        { return Connection; }
        
        Connections.Add(_ID, Channel.CreateBounded<Message<string>>(BCODefault, (_Msg) => Console.WriteLine($"Dropped message {_Msg}")));
        return Connections[_ID];
    }
}
