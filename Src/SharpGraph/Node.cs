namespace SharpGraph;

public class Node<TConnection> : BaseNode<TConnection>
{
    private Dictionary<TConnection, INode<uint>> Connections;

    public Node(uint _ID) {
        ID = _ID;
    }

    internal override void SetID(uint _ID)
        => ID = _ID;
}
