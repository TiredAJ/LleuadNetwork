using System.Collections;
using System.Linq.Expressions;

using CSharpFunctionalExtensions;

namespace SharpGraph;

public class Graph<TID, TNode> : IGraph<TID, TNode> where TNode : INode<TID> where TID : notnull 
{
    protected Dictionary<TID, TNode> Nodes = new();
    public void Clear() 
        => Nodes.Clear(); 

    public bool Contains(TNode _Item) 
        => Nodes.ContainsValue(_Item);

    public void CopyTo(TNode[] _Array, int _ArrayIndex) 
        => Nodes.Values.CopyTo(_Array, _ArrayIndex);

    public bool Remove(TNode _Item) {
        if (Nodes.ContainsKey(_Item.GetID()))
        {
            
        }
    }
    public int Count { get => Nodes.Count; }
    public bool IsReadOnly { get => false; }
    public Maybe<TNode> GetNodeByID(int _ID) { throw new NotImplementedException(); }
    public IEnumerable<TNode> GetConnectedNodes(int _ID) { throw new NotImplementedException(); }
    public IEnumerable<TNode> GetAllNodes() { throw new NotImplementedException(); }
    public Maybe<TNode> Insert(TNode _NewNode) { throw new NotImplementedException(); }
    public bool TryInsert(TNode _NewNode, out TNode _SavedNode) { throw new NotImplementedException(); }
    public TNode Remove(int _ID) { throw new NotImplementedException(); }
    public IEnumerable<TNode> Find(Expression<Func<TNode, bool>> _Expr) { throw new NotImplementedException(); }
    public bool DoesNodeExist(TNode _Node) { throw new NotImplementedException(); }
    public bool DoesNodeExist(int _ID) { throw new NotImplementedException(); }
}
