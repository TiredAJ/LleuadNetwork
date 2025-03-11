namespace SharpGraph;

public abstract class BaseNode<TID, TConnection> : INode<TID>
{
    required public TID ID;

    protected List<TConnection> Conns { get; set; } = new(); 

    public List<TConnection> GetConnections()
        => Conns;

    public TID GetID()
        => ID;
}
