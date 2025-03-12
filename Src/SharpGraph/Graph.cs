using System.Linq.Expressions;

using CSharpFunctionalExtensions;

namespace SharpGraph;

public class Graph<TNode> : IGraph<TNode> where TNode : INode<uint>
{
    protected Dictionary<uint, TNode> Nodes = new();

    readonly protected uint UnassignableID;
    protected uint LastID = 1;

    public Graph(uint _UnassignableID, uint _LastID) {
        UnassignableID = _UnassignableID;
        LastID = _LastID;
    }

    public Graph() {
        UnassignableID = 0;
    }

    protected uint GetNextID()
        => ++LastID;
    
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

    public Maybe<TNode> GetNodeByID(uint _ID) 
        => Nodes.TryGetValue(_ID, out TNode? MaybeNode) ? MaybeNode : Maybe<TNode>.None;

    public IEnumerable<TNode> GetConnectedNodes(uint _ID) {
        Maybe<TNode> Result = GetNodeByID(_ID);

        return Result.HasValue ? Result.Value.GetConnectedNodes().Cast<TNode>() : [];
    }

    public IEnumerable<TNode> GetAllNodes()
        => Nodes.Values;

    public Maybe<TNode> Insert(TNode _NewNode) {
        if (_NewNode.GetID() == UnassignableID)
        {
            _NewNode.SetID(GetNextID());
        }
    }
    
    public bool TryInsert(TNode _NewNode, out TNode _SavedNode) { throw new NotImplementedException(); }
    public TNode Remove(uint _ID) { throw new NotImplementedException(); }
    public IEnumerable<TNode> Find(Expression<Func<TNode, bool>> _Expr) { throw new NotImplementedException(); }
    public bool DoesNodeExist(TNode _Node) { throw new NotImplementedException(); }
    public bool DoesNodeExist(uint _ID) { throw new NotImplementedException(); }
}
