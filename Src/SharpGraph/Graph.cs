using System.Linq.Expressions;

using CSharpFunctionalExtensions;

namespace SharpGraph;

public class Graph<TNode> : IGraph<TNode> where TNode : class, INode<uint>
{
    protected Dictionary<uint, TNode> Nodes = new();

    readonly protected uint NullID;
    protected uint LastID = 1;

    public Graph(uint _NullID, uint _LastID) {
        NullID = _NullID;
        LastID = _LastID;
    }

    public Graph() {
        NullID = 0;
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

        uint NextID = GetNextID();

        if (Nodes.ContainsKey(NextID))
        { throw new Exception("ID already exists!"); /*TODO custom exc*/ }
        
        
        if (_NewNode.GetID() == NullID)
        { (_NewNode as NodeID)?.SetID(NextID); }

        return Nodes.TryAdd(NextID, _NewNode) ? _NewNode : Maybe<TNode>.None;

    }

    public bool TryInsert(TNode _NewNode, out TNode? _SavedNode) {

        Maybe<TNode> Res = Insert(_NewNode);

        if (Res.HasValue)
        {
            _SavedNode = Res.Value;
            return true;
        }

        _SavedNode = null;

        return false;
    }

    public Maybe<TNode> Remove(uint _ID) {

        bool R = Nodes.Remove(_ID, out TNode? Node);

        return R ? Node : Maybe<TNode>.None;
    }

    public IEnumerable<KeyValuePair<uint, TNode>> FindAny(Func<KeyValuePair<uint, TNode>, bool> _Expr) {
        return Nodes.Where(_Expr);
    }

    public bool DoesNodeExist(uint _ID) {
        return Nodes.ContainsKey(_ID);
    }
}
