using System.Collections;
using System.Linq.Expressions;
using System.Numerics;

using CSharpFunctionalExtensions;

namespace SharpGraph;

public class Graph<TID, TNode> : IGraph<TID, TNode> where TNode : INode<TID> where TID : IEqualityOperators<TID, TID, bool>
{
    protected Dictionary<TID, TNode> Nodes = new();

    protected TID UnassignableID;

    public Graph(TID _UnassignableID) {
        UnassignableID = _UnassignableID;
    }
    
    
    public void Clear() 
        => Nodes.Clear(); 

    public bool Contains(TNode _Node) 
        => Nodes.ContainsValue(_Node);

    public void CopyTo(TNode[] _Array, int _ArrayIndex) 
        => Nodes.Values.CopyTo(_Array, _ArrayIndex);

    public bool Remove(TNode _Node) 
        => Nodes.Remove(_Node.GetID());
        
    public int Count 
        => Nodes.Count;

    public Maybe<TNode> GetNodeByID(TID _ID) 
        => Nodes.TryGetValue(_ID, out TNode? MaybeNode) ? MaybeNode : Maybe<TNode>.None;

    public IEnumerable<TNode> GetConnectedNodes(TID _ID) {
        Maybe<TNode> Result = GetNodeByID(_ID);

        return Result.HasValue ? Result.Value.GetConnectedNodes().Cast<TNode>() : [];
    }

    public IEnumerable<TNode> GetAllNodes()
        => Nodes.Values;

    public Maybe<TNode> Insert(TNode _NewNode) {
        if (_NewNode.GetID() == UnassignableID)
        {
            
        }
    }
    public bool TryInsert(TNode _NewNode, out TNode _SavedNode) { throw new NotImplementedException(); }
    public TNode Remove(int _ID) { throw new NotImplementedException(); }
    public IEnumerable<TNode> Find(Expression<Func<TNode, bool>> _Expr) { throw new NotImplementedException(); }
    public bool DoesNodeExist(TNode _Node) { throw new NotImplementedException(); }
    public bool DoesNodeExist(int _ID) { throw new NotImplementedException(); }
}
