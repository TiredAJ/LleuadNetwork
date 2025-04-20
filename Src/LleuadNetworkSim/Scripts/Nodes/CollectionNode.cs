using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using LleuadNetworkSim.Scripts.Nodes;

using PipesTester;

public partial class CollectionNode : Node
{
    [Export]
    private PackedScene ConnectionTemplate;
    
    [Export]
    private PackedScene NetworkNodeTemplate;

    #region Selecting
    
    private List<NetworkNode> SelectedNodes = [];
    
    /// <summary>
    /// Returns true if successfully selected
    /// </summary>
    public bool AddSelectedNode(NetworkNode _Node) {

        if (Mode != UIMode.NONE)
        { return false; }
        
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
    
    #endregion
    
    #region Spawning

    public UIMode Mode = UIMode.NONE;

    public void SpawnNode(Vector2 _Location) {

        if (Mode != UIMode.SPAWNING)
        { return; }
        
        NetworkNode SceneInstance = NetworkNodeTemplate.Instantiate() as NetworkNode;

        SceneInstance.Position = _Location;
        SceneInstance.Name = $"NetworkNode-" + Guid.NewGuid().ToString();
        
        AddChild(SceneInstance);        
    }
    #endregion

    #region Connections
    
    private Dictionary<string, (NetworkNode, NetworkNode)> ConnectedNodes = [];
    
    public void TryConnect() {

        if (Mode != UIMode.CONNECTING || SelectedNodes.Count != 2)
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

        NodeConnection ConnAB = ConnectionTemplate.Instantiate() as NodeConnection;
        NodeConnection ConnBA = ConnectionTemplate.Instantiate() as NodeConnection;
        
        ConnAB.Init(NodeA, NodeB);
        ConnBA.Init(NodeB, NodeA);
        
        NodeA.AddConnection(NodeB.Name, ConnAB);
        NodeB.AddConnection(NodeA.Name, ConnBA);
        
        AddChild(ConnAB);
        AddChild(ConnBA);
    }
    #endregion

    #region Messages
    public void TrySendMessage() {
        if (Mode != UIMode.MESSAGING || SelectedNodes.Count != 2)
        { return; }
        
        NetworkNode NodeA = SelectedNodes[0];
        NetworkNode NodeB = SelectedNodes[1];
        
        NodeA.SendMessage(NodeB.Name);        
    }    
    #endregion
}
