using System.IO.Pipelines;

using LleuadNetwork.Messages;

using SharpGraph;

namespace LleuadNetwork;

public class NetNode : Node<NetLink>
{
    public NetNode(uint _ID) : base(_ID) { }

    public void AddConnection(NetLink _Link) {
        Conns.Add(_Link);
    }

    public void SendMessage(IMessage _Msg) {
        
    }

    private void ReceiveMessage() {

        Task.Run(() => { });
    }
}
