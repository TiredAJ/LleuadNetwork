using System.Linq.Expressions;

using CSharpFunctionalExtensions;

namespace SharpGraph;

public interface IGraph<TID, TNode> where TNode : INode<TID>
{
    #region Get
    public Maybe<TNode> GetNodeByID(int _ID);

    public IEnumerable<TNode> GetConnectedNodes(int _ID);

    public IEnumerable<TNode> GetAllNodes();
    #endregion

    #region Add
    public Maybe<TNode> Insert(TNode _NewNode);

    public bool TryInsert(TNode _NewNode, out TNode _SavedNode);
    #endregion

    #region Remove
    public TNode Remove(int _ID);
    #endregion

    #region Find
    public IEnumerable<TNode> Find(Expression<Func<TNode, bool>> _Expr);
    #endregion

    #region Checks
    public bool DoesNodeExist(TNode _Node);
    
    public bool DoesNodeExist(int _ID);
    #endregion
}
