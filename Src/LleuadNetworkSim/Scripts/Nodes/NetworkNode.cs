using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

using Godot;
using Godot.Logging;

using LleuadNetworkSim.Models.Lua;
using LleuadNetworkSim.Models.Messaging;
using LleuadNetworkSim.Models.Repo;
using LleuadNetworkSim.Models.Validation;
using LleuadNetworkSim.Models.Validation.Json;
using LleuadNetworkSim.Utils;

using MoreLinq;

namespace LleuadNetworkSim.Scripts.Nodes;

public partial class NetworkNode : CharacterBody2D, IPersistable
{
    #region Family
    [Export]
    private Sprite2D SelectionRing = null!;

    [Export]
    private Label NameLabel = null!;
    
    [Export]
    private PackedScene PacketTemplate = null!;

    private CollectionNode CollNodeParent = null!;
    #endregion

    #region Selection and movement
    [Export]
    private bool Lifted = false;

    [Export]
    public bool Selected {
        get;
        set {
            field = value;
            SelectionRing.Visible = Selected;
        }
    } = false;
    
    private void RequestSelection() {
        if (Lifted)
        { return; }
        
        Selected = CollNodeParent.RequestSelection(this);
    }
    #endregion

    #region Connections
    readonly private Dictionary<string, NodeConnection?> Connections = [];
    
    public void AddConnection(string _ID, NodeConnection? _Conn) {
        Connections.Add(_ID, _Conn);
    }
    
    public void RemoveConnection(string _ID) {
        Connections.Remove(_ID);
    }
    #endregion

    #region Overrides
    public override void _Ready() {
        
        Debug.WriteLine($"Node spawned at {Position}");

        CollNodeParent = GetParent<CollectionNode>();

        NameLabel.Text = this.Name;
        
        base._Ready();
    }

    public override void _UnhandledInput(InputEvent _Event)
    {        
        if (_Event is InputEventMouseButton && !_Event.IsPressed())
        { Lifted = false; }

        if (Lifted && _Event is InputEventMouseMotion IEMM)
        { Position += IEMM.Relative; }
        
        base._UnhandledInput(_Event);
    }

    public override void _InputEvent(Viewport _Viewport, InputEvent _Event, int _ShapeIdx)
    {
        if (_Event is InputEventMouseButton IEMM && _Event.IsPressed())
        {
            switch (IEMM.ButtonIndex)
            { 
                case MouseButton.Middle:
                    Lifted = true;
                    break;
                case MouseButton.Left:
                    RequestSelection();
                    break;
                default:
                    break;
            }

            Debug.WriteLine("Clicked");
        }
        
        base._InputEvent(_Viewport, _Event, _ShapeIdx);
    }
    #endregion

    #region Packets and messaging
    private ConcurrentQueue<Message> Backlog = [];
    private LuaController LC = new();
    
    public Task StartNode(CancellationToken _CT)
        => Task.Run(() => {                        
                        LC.LoadScript(LuaScriptAssembler.AssembleScript().Result, this.Name);
                        
                        StartProcessing(_CT);
                        
                    }, _CT);

    public void DebugSendMessage(string _ID) {

        Message Msg = new(this.Name, _ID, $"Hello from {this.Name}!! This is a payload") 
            { Lifespan = TimeSpan.FromMinutes(2) };

        Backlog.Enqueue(Msg);
    }

    private void SendMessage(string _ID, Message _Msg) {
        if (!Connections.TryGetValue(_ID, out NodeConnection? NodeConn))
        { return; }
        
        if (NodeConn == null)
        { return; }

        _Msg.LastNodeID = this.Name;
        
        Packet Packet = PacketTemplate.Instantiate<Packet>();

        Packet.ZIndex -= NodeConn.FollowerCount;
        
        NodeConn.CallDeferredThreadGroup("add_child", Packet);
        NodeConn.SendMessage(_Msg);
        NodeConn.FollowerCount++;
    }

    private void SendMessage(int _Port, Message _Msg) {

        if (Connections.Count < _Port)
        { GodotLogger.LogWarning("Packet lost due to invalid port"); }

        string Conn = Connections.ElementAt(_Port).Key;

        SendMessage(Conn, _Msg);
    }

    private void StartProcessing(CancellationToken _CT) {

        LC.PortCount = Connections.Count;
        
        LC.

    }
    
    public void PacketReceived(Message _Msg) {

        Repo.LogEvent(new MessageJourneyRecord(_Msg, this.Name, RecordAction.Received));
        
        if (_Msg.DestinationAddress == this.Name)
        {
            ConsumeMessage(_Msg);
            return;
        }
        
        if (!NodeValidator.MessageValid(_Msg))
        {
            DropMessage(_Msg);
            return;
        }
        
        GodotLogger.LogInfo($"{_Msg.ID} was backlog'd by {this.Name}");
        
        Backlog.Enqueue(_Msg);
    }

    private void ConsumeMessage(Message _Msg) {
        Repo.LogEvent(new MessageJourneyRecord(_Msg, this.Name, RecordAction.Consumed));
        Repo.LogEvent(new FinalMessageRecord(_Msg, this.Name, true));

        Interlocked.Decrement(ref G_TotalMessagesInPlay_Ref);
        
        Debug.WriteLine($"{this.Name} consumed packet from {_Msg.SenderAddress}. There are {G_TotalMessagesInPlay} messages left");
    }

    private void DropMessage(Message _Msg) {
        Repo.LogEvent(new MessageJourneyRecord(_Msg, this.Name, RecordAction.Dropped));
        Repo.LogEvent(new FinalMessageRecord(_Msg, this.Name, false));
        
        Interlocked.Decrement(ref G_TotalMessagesInPlay_Ref);
        
        Debug.WriteLine($"Message was dropped by {this.Name} as it was no longer valid:" +
                        $" {_Msg.Hops} hops, {_Msg.GetAliveTime().TotalSeconds:N2}s alive time." +
                        $". There are {G_TotalMessagesInPlay} messages left");
    }
    #endregion

    #region Persistence
    public JsonObject Save() {
        JsonArray JArray = [];

        foreach (string Key in Connections.Keys)
        { JArray.Add(Key); }

        return new JsonObject{
            ["Name"] = this.Name.ToString(),
            ["Connections"] = JArray,
            ["Pos"] = new JsonObject() {
                ["X"] = Position.X,
                ["Y"] = Position.Y
            },
        };
    }
    
    public void Load(IBaseVO _VOData) {

        if (_VOData is not NetworkNodeVO VO)
        { throw new NotImplementedException(); }
        
        this.Name = VO.Name;
        this.Position = VO.Pos.ToVec2();
    }
    #endregion
}