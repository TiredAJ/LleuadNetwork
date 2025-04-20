using System.Threading.Channels;

namespace PipesTester;

using Message = Message<string>;

public class Switchbox
{
    private Dictionary<string, Channel<Message>> Connections = new();
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

        MakeConnection(_NA, _NB);
    }

    private void MakeConnection(Node _NA, Node _NB) {

        string ConnStringAB = _NA.ID + _NB.ID;
        string ConnStringBA = _NB.ID + _NA.ID;

        Channel<Message> ChnnlAB = Connections.TryGetValue(ConnStringAB, out Channel<Message>? ConnectionAB) 
            ? ConnectionAB 
            : CreateConnection(ConnStringAB);
        
        Channel<Message> ChnnlBA = Connections.TryGetValue(ConnStringBA, out Channel<Message>? ConnectionBA) 
            ? ConnectionBA 
            : CreateConnection(ConnStringBA);
        
        _NA.Connections.Add(_NB.ID, new Connection(ChnnlAB, ChnnlBA));
        _NB.Connections.Add(_NA.ID, new Connection(ChnnlBA, ChnnlAB));
    }

    private Channel<Message> CreateConnection(string _ID) {

        Channel<Message> Chnnl =
            Channel.CreateBounded<Message>(BCODefault, (_Msg) => Console.WriteLine($"Dropped message {_Msg}"));
        
        Connections.Add(_ID, Chnnl);

        return Chnnl;
    }

    private string GetConnectionString(string _A, string _B) {
        byte[] Value = _A.AddValue(_B);

        return Convert.ToBase64String(Value);
    }
}
