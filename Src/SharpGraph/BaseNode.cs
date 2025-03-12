namespace SharpGraph;

public abstract class BaseNode<TConnection> : INode<uint>
{
    required public uint ID;

    protected List<TConnection> Conns { get; set; } = new(); 

    public List<TConnection> GetConnections()
        => Conns;

    public uint GetID()
        => ID;

    internal abstract void SetID(uint _ID);
}
