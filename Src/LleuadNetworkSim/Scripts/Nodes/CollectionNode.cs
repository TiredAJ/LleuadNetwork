using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

using Godot;
using Godot.Logging;

using LleuadNetworkSim.Scripts.Models.Message;
using LleuadNetworkSim.Scripts.Objects;
using LleuadNetworkSim.Utils;
using LleuadNetworkSim.Utils.Validators;
using LleuadNetworkSim.Utils.Validators.Json;

using MoreLinq;

namespace LleuadNetworkSim.Scripts.Nodes;

public partial class CollectionNode : Node, IPersistable
{
    [Export]
    private PackedScene ConnectionTemplate = null!;
    
    [Export]
    private PackedScene NetworkNodeTemplate = null!;

    [Export]
    private PackedScene ExceptionPopupTemplate = null!;
    
    #region Selecting
    readonly private List<NetworkNode> SelectedNodes = [];

    public void SelectionMode(bool _Toggled) {

        GetChild<Button>(1).MouseFilter = _Toggled ? Control.MouseFilterEnum.Ignore : Control.MouseFilterEnum.Stop;

        if (!_Toggled)
        {
            foreach (NetworkNode NN in GetChildren<NetworkNode>())
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
    public UIMode Mode { get; set; } = UIMode.NONE;

    public void SpawnNode(Vector2 _Location) {

        if (Mode != UIMode.SPAWNING)
        { return; }
        
        NetworkNode SceneInstance = (NetworkNodeTemplate.Instantiate() as NetworkNode)!;

        SceneInstance.Position = _Location;
        SceneInstance.Name = Guid.NewGuid().ToBase64Name();
        
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

        foreach (string Key in DeletableKeys)
        { ConnectedNodes.Remove(Key); }

        foreach (KeyValuePair<string, (NodeConnection, NodeConnection)> KVP in Connections
                     .Where(X => DeletableKeys.Contains(X.Key)))
        {
            (NodeConnection ConnAB, NodeConnection ConnBA) = KVP.Value;
            
            ConnAB.QueueFree();
            ConnBA.QueueFree();
        }

        foreach (string Key in DeletableKeys)
        { Connections.Remove(Key); }
        
        _Node.QueueFree();
    }
    #endregion

    #region Connections
    private Dictionary<string, (NetworkNode, NetworkNode)> ConnectedNodes = [];
    private Dictionary<string, (NodeConnection, NodeConnection)> Connections = [];

    readonly private BoundedChannelOptions BCODefault = new BoundedChannelOptions(100) {
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

        ConnectNodes(NodeA, NodeB);
    }

    private void ConnectNodes(NetworkNode _NA, NetworkNode _NB) {
        
        string ID = Convert.ToBase64String(_NA.Name.ToString().AddValue(_NB.Name));

        if (ConnectedNodes.ContainsKey(ID))
        {
            GodotLogger.LogInfo($"Connection ID {ID} already exists, skipping...");
            return;
        }
        
        ConnectedNodes.Add(ID, (_NA, _NB));
        
        NodeConnection? ConnAB = ConnectionTemplate.Instantiate() as NodeConnection;
        NodeConnection? ConnBA = ConnectionTemplate.Instantiate() as NodeConnection;

        if (ConnAB is null || ConnBA is null)
        {
            GodotLogger.LogWarning($"Null connections: ConnAB: [{ConnAB}], ConnBA: [{ConnBA}]");
            return;
        }
        
        ConnAB.Name = Guid.NewGuid().ToBase64();
        ConnBA.Name = Guid.NewGuid().ToBase64();
        
        Channel<Message> ChannelAB = Channel.CreateBounded<Message>(BCODefault);
        Channel<Message> ChannelBA = Channel.CreateBounded<Message>(BCODefault);
        
        ConnAB.Init(_NA, _NB, ChannelBA.Writer, ChannelBA.Reader);
        ConnBA.Init(_NB, _NA, ChannelAB.Writer, ChannelAB.Reader);
        
        Connections.Add(ID, (ConnAB, ConnBA));
        
        _NA.AddConnection(_NB.Name, ConnAB);
        _NB.AddConnection(_NA.Name, ConnBA);
        
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
        
        NodeA.DebugSendMessage(NodeB.Name);        
    }    
    #endregion

    #region Persist
    
    public JsonObject Save() {
        
        JsonObject JData = new JsonObject();
        JsonArray JArray = new JsonArray();
        
        foreach (NetworkNode NN in GetChildren().OfType<NetworkNode>())
        { JArray.Add(NN.Save()); }
        
        JData.Add("NetworkNodes", JArray);

        return JData;
    }
    public void Load(IBaseVO _VOData) {
        
        if (_VOData is not CollectionNodeVO CollNodeVO)
        { throw new NotImplementedException(); }

        foreach (NetworkNodeVO NN in CollNodeVO.NetworkNodes)
        { LoadNetworkNode(NN); }

        foreach (NetworkNodeVO NN in CollNodeVO.NetworkNodes)
        { ConnectLoadedNodes(NN); }
    }
    
    public void SaveMap(string _Path) {

        if (!Path.HasExtension(_Path))
        {
            Debug.WriteLine("Map save has extension!");
            _Path = Path.ChangeExtension(_Path, "lnmap");
        }
        
        JsonObject JData = Save();
        
        using StreamWriter Writer = new (_Path);
        
        JsonSerializerOptions JSO = new() {
            AllowTrailingCommas = false,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            AllowOutOfOrderMetadataProperties = false
        }; 

        Writer.Write(JData.ToJsonString(JSO));
    }
    public void LoadMap(string _Path) {
        
        ClearTransientChildren();
        
        FileValidator.ValidateFile(_Path, ".lnmap", this);

        JsonNode JData = JsonValidator.ValidateJson<CollectionNodeVO>(_Path, this);

        CollectionNodeVO CollNodeVO = JData.Deserialize<CollectionNodeVO>()!;

        Load(CollNodeVO);
    }

    private void LoadNetworkNode(NetworkNodeVO _NodeVO) {
        NetworkNode SceneInstance = (NetworkNodeTemplate.Instantiate() as NetworkNode)!;
        
        SceneInstance.Load(_NodeVO);
        
        AddChild(SceneInstance);
    }

    private void ConnectLoadedNodes(NetworkNodeVO _NodeVO) {

        Maybe<NetworkNode> NA = GetChild<NetworkNode>(X => X.Name == _NodeVO.Name);

        if (NA.HasNoValue)
        { throw new NotImplementedException(); }
        
        foreach (string NBName in _NodeVO.Connections)
        {
            Maybe<NetworkNode> NB = GetChild<NetworkNode>(X => X.Name == NBName);

            if (NB.HasNoValue)
            { throw new NotImplementedException(); }
            
            ConnectNodes(NA.Value, NB.Value);
        }
    }
    
    public void TryLoadChallenge(string _Path) {
        
        FileValidator.ValidateFile(_Path, ".lnchallenge", this);
        
        Challenge = new MapChallenge(_Path);
        Challenge.Value.GenerateChallenge();
    }
    
    private void ClearTransientChildren() {
        Connections.Clear();
        ConnectedNodes.Clear();
        
        GetChildren<NodeConnection>().ForEach(X => X.Free());
        GetChildren<NetworkNode>().ForEach(X => X.Free());
    }
    #endregion
    
    #region Challenges

    private bool IsRunningchallenge = false;
    private CancellationTokenSource CTSource = new();
    
    public async Task RunChallenge() {

        if (IsRunningchallenge)
        { ClearChallenge(); }

        IsRunningchallenge = true;

        Dictionary<string, NetworkNode> NNs = GetChildren<NetworkNode>().ToDictionary(K => K.Name.ToString(), V => V);

        if (!Challenge.HasNoValue)
        {
            GodotLogger.LogInfo($"No challenge loaded, aborting challenge generation"); 
            
            return;
        }
        
        /*
        Dictionary<string, List<Message>> Data = Challenge.Value.GenerateChallenge(NNs.Keys.ToList());

        foreach (KeyValuePair<string, NetworkNode> KVP in NNs)
        { KVP.Value.Backlog = new ConcurrentQueue<Message>(Data[KVP.Key]); }
        
        */

        CancellationToken CT = CTSource.Token;
        
        List<Task> NodesStartup = [];
        NodesStartup.AddRange(NNs.Values.Select(NN => NN.StartNode(CT)));

        await Task.WhenAll(NodesStartup);
        IsRunningchallenge = false;
    }

    private void ClearChallenge() {
        GetChildren<NodeConnection>().ForEach(X => X.ClearMessages());
        CTSource.Cancel();
        IsRunningchallenge = false;
    }

    public void StopChallenge() {
        CTSource.Cancel();
        ClearChallenge();
    }
    
    #endregion
    
    #region Utils
    private Maybe<T> GetChild<T>(Func<T, bool> _Predicate) where T : Node
        => GetChildren()
           .OfType<T>()
           .Where(X => !X.IsQueuedForDeletion())
           .FirstOrDefault(_Predicate)
           .AsMaybe();

    private IEnumerable<T> GetChildren<T>() where T : Node
        => GetChildren()
           .Where(X => !X.IsQueuedForDeletion())
           .OfType<T>();
    #endregion
}