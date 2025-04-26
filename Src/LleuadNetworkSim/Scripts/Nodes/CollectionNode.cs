using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Channels;

using CSharpFunctionalExtensions;

using Godot.Logging;

using LleuadNetworkSim.Scripts.Exceptions;
using LleuadNetworkSim.Scripts.Nodes;
using LleuadNetworkSim.Scripts.Objects;

using PipesTester;

public partial class CollectionNode : Node, IPersistable
{
    [Export]
    private PackedScene ConnectionTemplate;
    
    [Export]
    private PackedScene NetworkNodeTemplate;

    [Export]
    private PackedScene ExceptionPopupTemplate;
    
    #region Selecting
    
    private List<NetworkNode> SelectedNodes = [];

    public void SelectionMode(bool _Toggled) {

        GetChild<Button>(1).MouseFilter = _Toggled ? Control.MouseFilterEnum.Ignore : Control.MouseFilterEnum.Stop;

        if (!_Toggled)
        {
            foreach (NetworkNode NN in GetChildren().Where(X => X is NetworkNode NN))
            { NN.Selected = false; }
        }
        
        Mode = UIMode.NONE; 
    }
    
    public bool RequestSelection(NetworkNode _Node) {

        if (SelectedNodes.Contains(_Node))
        {
            SelectedNodes.Remove(_Node);
            return false;
        }
        
        if (SelectedNodes.Count == 2)
        {
            SelectedNodes[0].Selected = false;
            
            SelectedNodes.RemoveAt(0);
        }
        
        SelectedNodes.Add(_Node);

        return true;
    }
    
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

    public bool DeselectNode(NetworkNode _Node) {
        _Node.Selected = false;
        
        SelectedNodes.Remove(_Node);
        
        return false;
    }

    public void DeselectAll() {
        foreach (NetworkNode Node in SelectedNodes)
        { Node.Selected = false; }
        
        SelectedNodes.Clear();
    }
    
    #endregion
    
    #region Spawning

    public UIMode Mode = UIMode.NONE;

    public void SpawnNode(Vector2 _Location) {

        if (Mode != UIMode.SPAWNING)
        { return; }
        
        NetworkNode SceneInstance = NetworkNodeTemplate.Instantiate() as NetworkNode;

        SceneInstance.Position = _Location;
        SceneInstance.Name = $"NetworkNode-" + Guid.NewGuid().ToBase64();
        
        AddChild(SceneInstance);        
    }

    public void HandleDelete() {

        if (SelectedNodes.Count == 0)
        { return; }
        
        NetworkNode NodeA = SelectedNodes[0];
        DeleteNode(NodeA);

        if (SelectedNodes.Count == 2)
        {
            NetworkNode NodeB = SelectedNodes[1];
            DeleteNode(NodeB);
        }
        
        SelectedNodes.Clear();
    }

    private void DeleteNode(NetworkNode _Node) {

        List<string> DeletableKeys = ConnectedNodes.Where(X => (X.Value.Item1 == _Node || X.Value.Item2 == _Node))
                                    .Select(X => X.Key)
                                    .ToList();

        foreach (var Key in DeletableKeys)
        { ConnectedNodes.Remove(Key); }

        foreach (var KVP in Connections
                     .Where(X => DeletableKeys.Contains(X.Key)))
        {
            (NodeConnection ConnAB, NodeConnection ConnBA) = KVP.Value;
            
            ConnAB.QueueFree();
            ConnBA.QueueFree();
        }

        foreach (var Key in DeletableKeys)
        { Connections.Remove(Key); }
        
        _Node.QueueFree();
    }
    #endregion

    #region Connections
    
    //maybe persist
    private Dictionary<string, (NetworkNode, NetworkNode)> ConnectedNodes = [];
    //maybe persist
    private Dictionary<string, (NodeConnection, NodeConnection)> Connections = [];
    
    private BoundedChannelOptions BCODefault = new BoundedChannelOptions(20) {
        AllowSynchronousContinuations = false,
        SingleReader = true,
        SingleWriter = true,
        FullMode = BoundedChannelFullMode.Wait
    };
    
    public void TryConnect() {

        if (SelectedNodes.Count != 2)
        { return; }

        Mode = UIMode.CONNECTING;
        
        NetworkNode NodeA = SelectedNodes[0];
        NetworkNode NodeB = SelectedNodes[1];
        
        string ID = Convert.ToBase64String(NodeA.Name.ToString().AddValue(NodeB.Name));

        if (ConnectedNodes.ContainsKey(ID))
        {
            GodotLogger.LogWarning($"Connection ID {ID} already exists");
            return;
        }
        
        ConnectedNodes.Add(ID, (NodeA, NodeB));
        
        NodeConnection ConnAB = ConnectionTemplate.Instantiate() as NodeConnection;
        NodeConnection ConnBA = ConnectionTemplate.Instantiate() as NodeConnection;

        if (ConnAB is null || ConnBA is null)
        {
            GodotLogger.LogWarning($"Null connections: ConnAB: [{ConnAB}], ConnBA: [{ConnBA}]");
            return;
        }
        
        ConnAB.Name = $"NodeConnection-" + Guid.NewGuid().ToBase64();
        ConnBA.Name = $"NodeConnection-" + Guid.NewGuid().ToBase64();
        
        Channel<Message> ChannelAB = Channel.CreateBounded<Message>(BCODefault);
        Channel<Message> ChannelBA = Channel.CreateBounded<Message>(BCODefault);
        
        ConnAB.Init(NodeA, NodeB, ChannelAB, ChannelBA.Reader);
        ConnBA.Init(NodeB, NodeA, ChannelBA, ChannelAB.Reader);
        
        Connections.Add(ID, (ConnAB, ConnBA));
        
        NodeA.AddConnection(NodeB.Name, ConnAB);
        NodeB.AddConnection(NodeA.Name, ConnBA);
        
        AddChild(ConnAB);
        AddChild(ConnBA);
    }
    #endregion

    #region Messages

    private Maybe<MapChallenge> Challenge = Maybe.None;
    
    public void TrySendMessage() {
        if (SelectedNodes.Count != 2)
        { return; }

        Mode = UIMode.MESSAGING;
        
        NetworkNode NodeA = SelectedNodes[0];
        NetworkNode NodeB = SelectedNodes[1];
        
        NodeA.SendMessage(NodeB.Name);        
    }    
    #endregion

    #region Persist
    public JsonNode Save() {
        throw new NotImplementedException();
    }
    public void Load(JsonNode _JData) {
        throw new NotImplementedException();
    }
    
    public void SaveMap() {
        throw new NotImplementedException();
    }
    public void LoadMap() {
        throw new NotImplementedException();
    }
    
    public void TryLoadChallenge(string _Path) {
        
        if (Path.GetExtension(_Path) != ".lnchallenge") //25MB
        { ExceptionPopupWrapper.Throw(this, new InvalidChallengeFileException(_Path)); }

        long FileSize = new FileInfo(_Path).Length;

        if (FileSize > (25 * 1000 * 1000))
        { ExceptionPopupWrapper.Throw(this, new FileTooLargeException(FileSize)); }
        
        Challenge = new MapChallenge(_Path);
        Challenge.Value.GenerateChallenge();
    }
    
    public void Persist() {
        
    }
    public void Open() { 
        throw new NotImplementedException();
    }    
    #endregion

}
