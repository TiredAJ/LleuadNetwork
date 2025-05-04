using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

using Godot;

using LleuadNetworkSim.Scripts.Objects;
using LleuadNetworkSim.Utils;
using LleuadNetworkSim.Utils.Validators.Json;

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
    public void SendMessage(string _ID) {

        if (!Connections.TryGetValue(_ID, out NodeConnection? NodeConn))
        { return; }
        
        Packet Packet = PacketTemplate.Instantiate<Packet>();
        Message Msg = new(this.Name, _ID, $"Hello from {this.Name}!! This is a payload");

        if (NodeConn == null)
        { return; }

        Packet.ZIndex -= NodeConn.FollowerCount;
        NodeConn.AddChild(Packet);
        NodeConn.SendMessage(Msg);
        NodeConn.FollowerCount++;
    }
    
    public async Task PacketReceived(Message _Msg) {
        /* handle packet - read channel */
        
        Debug.WriteLine($"Packet received at {this.Name} from {_Msg.SenderAddress}");
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