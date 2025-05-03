using System.Runtime.CompilerServices;

namespace SharpGraph;

public abstract class BaseNode<TConnection> : INode<uint>
{
    required public uint ID;

    protected List<TConnection> Conns { get; set; } = new(); 

    public List<TConnection> GetConnections()
        => Conns;

    public uint GetID()
        => ID;

    public IEnumerable<INode<uint>> GetConnectedNodes()
        => throw new NotImplementedException();
}
