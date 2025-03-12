namespace SharpGraph;

public class Node<TConnection> : BaseNode<int, TConnection>
{
    private Dictionary<TConnection, INode<int>> Connections;

    public Node(int _ID) {
        ID = _ID;
    }
}
