namespace SharpGraph;

public class Node<TConnection> : BaseNode
{
    private Dictionary<TConnection, BaseNode> Connections;
}
