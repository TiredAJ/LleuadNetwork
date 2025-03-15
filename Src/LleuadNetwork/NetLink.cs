using System.IO.Pipelines;

using CSharpFunctionalExtensions;

using SharpGraph;

namespace LleuadNetwork;

public class NetLink : BaseConnection<uint, NetNode>
{
    private PipeReader OutputPipe;
    private PipeWriter InputPipe;
    
    public NetLink(NetNode _Node, float _LinkDistance, PipeReader _Output, PipeWriter _Input) : base(_Node) {
        LinkDistance = _LinkDistance;
        OutputPipe = _Output;
        InputPipe = _Input;
    }
    
    public float LinkDistance { get; set; }
}
