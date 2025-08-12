using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

using Common.Challenge;
using Common.Entities;
using Common.Json;
using Common.Json.Exceptions;
using Common.Messaging;
using Common.Utils;

using CSharpFunctionalExtensions;

using Godot;
using Godot.DependencyInjection.Attributes;
using Godot.Logging;

using LiteDB;

using LleuadNetworkSim.Models.Lua;
using LleuadNetworkSim.Models.Repo;
using LleuadNetworkSim.Models.Validation;
using LleuadNetworkSim.Utils;
using LleuadNetworkSim.Utils.GodotUtils;

using MoonSharp.Interpreter;

using MoreLinq;

using static Common.Conf.Conf;

// ReSharper disable ArrangeMissingParentheses
// ReSharper disable RedundantJumpStatement

namespace LleuadNetworkSim.Scripts.Nodes;

public partial class CollectionNode : Node
{
    [Export]
    private PackedScene ConnectionTemplate = null!;

    [Export]
    private PackedScene NetworkNodeTemplate = null!;

    [Export]
    private PackedScene ExceptionPopupTemplate = null!;
    
    [Export]
    private PackedScene DetailViewScene {
        get => DetailViewSingleton.Scene;
        set => DetailViewSingleton = new Utils.GodotUtils.PackedSceneSingleton<win_DetailView> {Scene = value};
    }

    private PackedSceneSingleton<win_DetailView> DetailViewSingleton = null!;

    private InstancePool<NetworkNode> NodePool;

    public override void _Ready() {

        //register Lua UserData types
        UserData.RegisterType<Message>();
        UserData.RegisterType<ReadonlyMessage>();
        UserData.DefaultAccessMode = InteropAccessMode.Preoptimized;

#if DEBUG
        G_ChallengeID = new ObjectId(1751909515, 16211519, 1177, 8867193);
#endif

        AddChild(DetailViewSingleton.GetInstance());

        NodePool = new InstancePool<NetworkNode>(NetworkNodeTemplate);

        NodePool.Initialise().Fire();

        bool HasLoadedMap = false;
        bool HasLoadedScript = false;
        bool HasLoadedChallenge = false;

        foreach (string? Arg in OS.GetCmdlineArgs())
        {
            if (HasLoadedMap && HasLoadedChallenge && HasLoadedScript)
            { break; }

            if (!HasLoadedMap && Arg.Contains("--map="))
            {
                if (HasLoadedChallenge)
                { GodotLogger.LogWarning("Cannot load a map if challenge has been loaded!"); continue; }
                
                LoadMapFromFile(Arg[6..]);
                HasLoadedMap = true; continue;
            }

            if (!HasLoadedScript && Arg.Contains("--scripts="))
            {
                LoadScript(Arg[10..].Split(';'));
                HasLoadedScript = true; continue;
            }

            if (!HasLoadedChallenge && Arg.Contains("--challenge="))
            {
                if (HasLoadedMap)
                { GodotLogger.LogWarning("Cannot load a challenge if a map has been loaded!"); continue; }
                
                TryLoadChallenge(Arg[12..]);
                HasLoadedChallenge = true; continue;
            }
        }

        base._Ready();
    }

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
    /// <param name="_Node">Node to be selected</param>
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

        NetworkNode SceneInstance = NodePool.Get();

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

        _Node.Reset();
        
        NodePool.Return(_Node);
        
        RemoveChild(_Node);
    }
    #endregion

    #region Connections
    readonly private Dictionary<string, (NetworkNode, NetworkNode)> ConnectedNodes = [];
    readonly private Dictionary<string, (NodeConnection, NodeConnection)> Connections = [];

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

    //TODO: TEST - this
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
    
    //TODO: TEST - this
    private void ConnectNodes(HashSet<string> _Conn) {

        HashSet<string> NodeNames = _Conn.SelectMany(X => X.Split("--"))
            .ToHashSet();
        
        Dictionary<StringName, NetworkNode> Nodes = GetChildren()
            .OfType<NetworkNode>()
            .Where(X => NodeNames.Contains(X.Name))
            .ToDictionary(K => K.Name, V => V);
        
        foreach (string Conn in _Conn)
        {
            string ID_A = Conn.Split("--")[0];
            string ID_B = Conn.Split("--")[1];

            if (!Nodes.TryGetValue(ID_A, out NetworkNode? NodeA) || !Nodes.TryGetValue(ID_B, out NetworkNode? NodeB))
            {
                //TODO throw
                return;
            }

            ConnectNodes(NodeA, NodeB);
        }
    }
    #endregion

    #region Messages
    //TODO: TEST - maybe this
    public void TrySendMessage() {
        if (SelectedNodes.Count != 2)
        { return; }

        Mode = UIMode.MESSAGING;

        NetworkNode NodeA = SelectedNodes[0];
        NetworkNode NodeB = SelectedNodes[1];

        Interlocked.Increment(ref G_TotalMessagesInPlay_Ref);

        NodeA.DebugSendMessage(NodeB.Name);
    }
    #endregion

    #region Persist
    //TODO: TEST - maybe this
    public void SaveMap(string _Path) {

        if (!Path.HasExtension(_Path))
        {
            Debug.WriteLine("Map save has extension!");
            _Path = Path.ChangeExtension(_Path, "lnmap");
        }

        MapVO Map = new();

        List<NetworkNodeVO> Nodes = GetChildren()
            .OfType<NetworkNode>()
            .Select(X => X.ToVO())
            .ToList();
        
        Map.NetworkNodes = Nodes.ToArray();

        //unique connections only
        HashSet<string> TempConnections = [];
        
        foreach (NetworkNodeVO NN in Nodes)
        {
            foreach (string Conn in NN.Connections)
            { TempConnections.Add(MiscUtils.OrderedNames(NN.Name, Conn)); } 
        }

        Map.UniqueConnections = TempConnections;

        JSONHelper.SerialiseToFile(_Path, Map);
    }
    
    //TODO: TEST - maybe this
    public void LoadMapFromFile(string _Path) {
        FileValidator.ValidateFile(_Path, MAP_EXTENSION, this);
        
        Maybe<Exception> exc = JsonValidator.ValidateJson<MapVO>(_Path, out Stream? JStream);
        
        if (exc.HasValue)
        { ExceptionPopupWrapper.Throw(this, exc.Value); }

        if (JStream is null)
        { ExceptionPopupWrapper.Throw(this, new Exception("JStream was null!")); return; }

        Result<MapVO> MapRes = JSONHelper.Deserialise<MapVO>(JStream);
        
        if (MapRes.IsFailure)
        { ExceptionPopupWrapper.Throw(this, new JSONDeserialisationException(nameof(MapVO))); return; }
        
        LoadMap(MapRes.Value);
    }
    
    //TODO: TEST - maybe this
    private void LoadMap(MapVO _Map) {
        ClearTransientChildren();
        
        foreach (NetworkNodeVO NN in _Map.NetworkNodes)
        { LoadNetworkNode(NN); }
        
        ConnectNodes(_Map.UniqueConnections);
        
        if (DetailViewSingleton.HasInstance && DetailViewSingleton.GetInstance().IsVisible())
        { UpdateDetailView(_Map.NetworkNodes.Select(X => X.Name).ToList()); }
    }

    private void LoadNetworkNode(NetworkNodeVO _NodeVO) {
        NetworkNode NodeInstance = NodePool.Get();
        
        NodeInstance.Load(_NodeVO);
        
        AddChild(NodeInstance);
        
    }
    
    public void TryLoadChallenge(string _Path) {

        FileValidator.ValidateFile(_Path, CHALLENGE_EXTENSION, this);

        Challenge = MapChallenge.LoadChallenge(_Path).AsMaybe();

        if (Challenge.HasNoValue)
        { GodotLogger.LogError($"Failed to load challenge {_Path}"); return; }
        
        LoadMap(Challenge.Value.Map);
    }

    private void ClearTransientChildren() {
        Connections.Clear();
        ConnectedNodes.Clear();

        GetChildren<NodeConnection>().ForEach(X => X.QueueFree());
        GetChildren<NetworkNode>().ForEach(X => {
            X.Reset();
            NodePool.Return(X);
            RemoveChild(X);
        });
    }
    #endregion

    #region Challenges
    private Maybe<MapChallenge> Challenge = Maybe.None;
    private bool IsRunningChallenge;
    private CancellationTokenSource CTSource = null!;

    public async Task RunChallenge() {
        if (IsRunningChallenge)
        { ClearChallenge(); }

        CTSource = new CancellationTokenSource();

        IsRunningChallenge = true;

        if (Challenge.HasNoValue)
        {
            GodotLogger.LogInfo($"No challenge loaded, aborting challenge generation");
            return;
        }

        Dictionary<string, NetworkNode> NNs = GetChildren<NetworkNode>().ToDictionary(K => K.Name.ToString(), V => V);

        Dictionary<string, List<Message>> Data = Challenge.Value.GenerateChallenge(NNs.Keys.ToList());

        foreach (KeyValuePair<string, NetworkNode> KVP in NNs)
        { KVP.Value.Backlog = new ConcurrentQueue<Message>(Data[KVP.Key]); }

        G_TotalMessagesInPlay = Data.Sum(X => X.Value.Count);
        G_ChallengeID = DB.ChallengeColl()
                          .Insert(new ChallengeRecord(NNs.Count, G_TotalMessagesInPlay, Challenge.Value.Name));

        CancellationToken CT = CTSource.Token;

        LuaScript LS = await LuaScriptAssembler.AssembleScript(_PreLoad: true);

        List<Task> NodesStartup = [];
        NodesStartup.AddRange(NNs.Values.Select(NN => NN.StartNode(LS, CT)));

        DetailViewSingleton.GetInstance().UpdateRunning(true);

        await Task.WhenAll(NodesStartup);
        IsRunningChallenge = false;
    }

    private void ClearChallenge() {
        GetChildren<NodeConnection>().ForEach(X => X.ClearMessages());
        CTSource.Cancel();
        CTSource.Dispose();
        IsRunningChallenge = false;
    }

    public void StopChallenge() {
        CTSource.Cancel();
        ClearChallenge();
        DetailViewSingleton.GetInstance().UpdateRunning(false);
    }

    #endregion

    #region DetailsView
    private void UpdateDetailView(List<string> _NodeIDs) {
        DetailViewSingleton.GetInstance().UpdateNodes(_NodeIDs);
    }
    
    public void ToggleDetailWindow(bool _ToggleState) {
        DetailViewSingleton.GetInstance().Visible = _ToggleState;
    }
    #endregion

    //TODO: TEST - maybe these
    #region Utils
    private Maybe<T> GetChild<T>(Func<T, bool> _Predicate) where T : Node
        => GetChildren()
           .OfType<T>()
           .Where(X => !X.IsQueuedForDeletion())
           .FirstOrDefault(_Predicate)
           .AsMaybe();

    private T GetChild<T>() where T : Node {
        T[] Results = GetChildren()
                      .OfType<T>()
                      .Where(X => !X.IsQueuedForDeletion())
                      .ToArray();

        if (Results.Length != 1)
        { throw new InvalidDataException($"Expected one child but found {Results.Length}"); }

        return Results[0];
    }

    private Maybe<T> MaybeGetChild<T>() where T : Node {
        T[] Results = GetChildren()
                      .OfType<T>()
                      .Where(X => !X.IsQueuedForDeletion())
                      .ToArray();

        if (Results.Length != 1)
        { GodotLogger.LogWarning($"Expected one child but found {Results.Length}"); }

        return Results[0];
    }

    private IEnumerable<T> GetChildren<T>() where T : Node
        => GetChildren()
           .Where(X => !X.IsQueuedForDeletion())
           .OfType<T>();
    #endregion

    #region Scripts
    public void LoadScript(params string[] _Paths) {
        foreach (string P in _Paths)
        {
            try
            {
                if (IsDir(P))
                { LoadScriptDir(P).ForEach(LuaScriptAssembler.AddScript); }
                else if (IsFile(P))
                { LuaScriptAssembler.AddScript(P); }
            }
            catch (Exception Exc)
            { ExceptionPopupWrapper.Throw(this, Exc); }
        }
    }

    static private IEnumerable<string> LoadScriptDir(string _DirPath) {

        List<string> Paths = [];

        Paths.AddRange(Directory.GetFiles(_DirPath, "*.lua"));

        return Paths;
    }

    static private bool IsDir(string _Path)
        => Directory.Exists(_Path) && Path.GetDirectoryName(_Path) == _Path;

    static private bool IsFile(string _Path)
        => File.Exists(_Path);
    #endregion

    #region Tidy up
    [Inject]
    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    private IDBWrapper DB = null!;

    public override void _Notification(int _Notif)
    {
        if (_Notif == NotificationWMCloseRequest)
        {
            GodotLogger.LogDebug("Checkpointing DB");
            DB.Checkpoint();
        }

        base._Notification(_Notif);
    }
    #endregion
}
 
