using System.Numerics;

namespace SharpGraph;

public class BaseConnection<TID, TNode> where TNode : INode<TID> where TID : IEqualityOperators<TID, TID, bool>
{
    public BaseConnection(TNode _Node) {
        Node = _Node;
    }

    public TNode Node { get;  private set; }
}
