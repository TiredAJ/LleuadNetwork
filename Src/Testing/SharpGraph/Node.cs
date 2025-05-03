namespace SharpGraph;

public class Node<TConnection> : BaseNode<TConnection>
{
    public Node(uint _ID) {
        ID = _ID;
    }
}


class NodeComparer : EqualityComparer<INode<uint>>
{
    public override bool Equals(INode<uint>? A, INode<uint>? B) {

        if (A is null || B is null)
        { return false; }
        
        IOrderedEnumerable<INode<uint>> AConns = A.GetConnectedNodes()
                                                  .OrderBy(X => X.GetID());
        IOrderedEnumerable<INode<uint>> BConns = B.GetConnectedNodes()
                                                  .OrderBy(X => X.GetID());
        
        return (A.GetID() == B.GetID()) && AConns.SequenceEqual(BConns);
    }
    
    public override int GetHashCode(INode<uint> obj) { throw new NotImplementedException(); }
}