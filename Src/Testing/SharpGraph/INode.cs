using System.Numerics;

namespace SharpGraph;

public interface INode<out TID> where TID : IEqualityOperators<TID, TID, bool>
{
    public TID GetID();

    public IEnumerable<INode<TID>> GetConnectedNodes();
}
