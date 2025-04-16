using System.Threading.Channels;

namespace PipesTester;

public class Switchbox
{
    private Dictionary<string, Channel<Message<string>>> Connections = new();
    private BoundedChannelOptions BCODefault;

    public Switchbox() {
        BCODefault = new BoundedChannelOptions(50) {
            AllowSynchronousContinuations = false,
            SingleReader = true,
            SingleWriter = true,
            FullMode = BoundedChannelFullMode.Wait
        };
    }

    public void Connect(Node _NA, Node _NB) {
        if (_NA.Connections.ContainsKey(_NB.ID) && _NB.Connections.ContainsKey(_NA.ID))
        { return; }

        MakeConnection()
    }

    private Channel<Message<string>> MakeConnection(Node _NA, Node _NB) {
        if (Connections.TryGetValue(_ID, out Channel<Message<string>>? Connection))
        { return Connection; }

        CreateConnection(_ID);
        return Connections[_ID];
    }

    private void CreateConnection(string _ID) {
        Connections.Add(_ID, Channel.CreateBounded<Message<string>>(BCODefault, (_Msg) => Console.WriteLine($"Dropped message {_Msg}")));
    }
    
    private string GetConnectionString(string _A, string _B) => {
        //add string values together
    }  
}
