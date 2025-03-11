namespace SharpGraph;

public class BaseConnection<TID, TNode> where TNode : INode<TID>
{
    public TNode Node { get; set; }
}
