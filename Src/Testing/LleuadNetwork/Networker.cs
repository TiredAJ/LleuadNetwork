using System.Collections.Concurrent;
using System.IO.Pipelines;

using CSharpFunctionalExtensions;

using SharpGraph;

namespace LleuadNetwork;

public class Networker : Graph<NetNode>
{
    private ConcurrentBag<Pipe> Pipes;

    public NetNode AddNode(NetNode _NewNode) {
        
    }

    public void ConnectNodes(uint _IDA, uint _IDB, float _Cost) {
        Maybe<NetNode> A = Nodes.TryFind(_IDA);
        Maybe<NetNode> B = Nodes.TryFind(_IDB);

        if (A.HasNoValue || B.HasNoValue)
        { return; }

        Pipe PipeAB = new Pipe();
        Pipe PipeBA = new Pipe();

        NetLink AB = new NetLink(B.Value, _Cost, PipeAB.Reader, PipeBA.Writer);
        NetLink BA = new NetLink(A.Value, _Cost, PipeBA.Reader, PipeAB.Writer);
        
        
        A.Value.AddConnection(AB);
        B.Value.AddConnection(BA);
    }
    
    
    public Result ExportNetworkToFile() {
        
    }
    
    public Result ImportNetworkFromFile() {
        
    }

    private void SetupPipes() {
        
    }
}
