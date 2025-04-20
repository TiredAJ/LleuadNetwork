using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

using PipesTester;

public partial class CollectionNode : Node
{
    [Export]
    private PackedScene Connection;
    
    
    private List<NetworkNode> SelectedNodes = [];

    private Dictionary<string, (NetworkNode, NetworkNode)> ConnectedNodes = [];
    
    /// <summary>
    /// Returns true if successfully selected
    /// </summary>
    public bool AddSelectedNode(NetworkNode _Node) {

        if (SelectedNodes.Count == 2)
        { return false; }
        
        SelectedNodes.Add(_Node);

        Debug.WriteLine($"{_Node.Name} was added to selection");        
        
        return true;
    }

    public void RemoveSelectedNode(NetworkNode _Node) {
        SelectedNodes.Remove(_Node);
        Debug.WriteLine($"{_Node.Name} was removed from selection");
    }

    public void TryConnect() {

        if (SelectedNodes.Count != 2)
        { return; }
        
        NetworkNode NodeA = SelectedNodes[0];
        NetworkNode NodeB = SelectedNodes[1];        
        
        string ID = Convert.ToBase64String(NodeA.Name.ToString().AddValue(NodeB.Name));

        if (ConnectedNodes.ContainsKey(ID))
        { return; }
        
        ConnectedNodes.Add(ID, (NodeA, NodeB));

        NodeA.Selected = false;
        NodeB.Selected = false;
        
        SelectedNodes.Clear();

        NodeConnection Conn = Connection.Instantiate() as NodeConnection;
        
        Conn.Init(NodeA, NodeB);
        
        AddChild(Conn);
    }
}
