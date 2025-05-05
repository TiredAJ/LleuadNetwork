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

using LleuadNetworkSim.Scripts.Buttons;
using LleuadNetworkSim.Scripts.Models;
using LleuadNetworkSim.Scripts.Models.Message;
using LleuadNetworkSim.Scripts.Objects;
using LleuadNetworkSim.Utils;
using LleuadNetworkSim.Utils.Validators;
using LleuadNetworkSim.Utils.Validators.Json;

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

    public ConcurrentQueue<Message> Backlog = [];
    
    public Task StartNode(CancellationToken _Ct)
        => Task.Run(() => {
                        while (!_Ct.IsCancellationRequested)
                        {
                            if (!Backlog.IsEmpty && Backlog.TryDequeue(out Message? Msg))
                            {
                                try
                                { ProcessMessage(Msg); }
                                catch (Exception Exc)
                                {
                                    DropMessage(Msg);
                                    Debug.WriteLine($"Processing failed with {Exc.Message}");
                                }
                            }
                            Thread.Sleep(500);
                        }
                        return;
                    }, _Ct);

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

    private void SendMessage(int _Port) {

        if (Connections.Count < _Port)
        { GodotLogger.LogWarning("Packet lost due to invalid port"); }

        string Conn = Connections.ElementAt(_Port).Key;

        if (Backlog.TryDequeue(out Message? MSg))
        { SendMessage(Conn, MSg); }
    }

    private void ProcessMessage(Message _Msg) {
        string? Addr = Connections.ContainsKey(_Msg.DestinationAddress) 
                           ? _Msg.DestinationAddress 
                           : Connections.Keys
                                        .Shuffle()
                                        .FirstOrDefault(X => X != _Msg.SenderAddress && X != _Msg.LastNodeID);
        
        /*
         * Lua processing here
         */

        if (Addr is null)
        { DropMessage(_Msg); }
        else
        { SendMessage(Addr, _Msg); }
    }
    
    public void PacketReceived(Message _Msg) {

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
        Debug.WriteLine($"{this.Name} consumed packet from {_Msg.SenderAddress}");
    }

    private void DropMessage(Message _Msg) {
        Debug.WriteLine($"Message was dropped by {this.Name} as it was no longer valid:" +
                        $" {_Msg.Hops} hops, {_Msg.GetAliveTime().TotalSeconds:N2}s alive time");
        
        //log
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