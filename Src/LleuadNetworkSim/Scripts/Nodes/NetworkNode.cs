using Godot;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

using LleuadNetworkSim.Scripts.Nodes;
using LleuadNetworkSim.Scripts.Objects;
using LleuadNetworkSim.Utils;
using LleuadNetworkSim.Utils.Validators.Json;

public partial class NetworkNode : CharacterBody2D, IPersistable
{
    #region Family
    [Export]
    private Sprite2D SelectionRing;

    [Export]
    private Label NameLabel;
    
    [Export]
    private PackedScene PacketTemplate;

    private CollectionNode CollNodeParent;
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
    private Dictionary<string, NodeConnection> Connections = [];
    
    public void AddConnection(string _ID, NodeConnection _Conn) {
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

        if (!Connections.ContainsKey(_ID))
        { return; }
        
        Packet Packet = PacketTemplate.Instantiate<Packet>();
        Message Msg = new Message(this.Name, _ID, $"Hello from {this.Name}!! This is a payload");

        Packet.ZIndex -= Connections[_ID].FollowerCount;
        Packet.Msg = Msg;
        
        Connections[_ID].AddChild(Packet);

        Connections[_ID].FollowerCount++;
    }
    
    public async Task PacketReceived(Message _Msg) {
        /* handle packet - read channel */
        
        Debug.WriteLine($"Packet received at {this.Name}");
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

        NetworkNodeVO VO = (_VOData as NetworkNodeVO)!;
        
        this.Name = VO.Name;
        this.Position = VO.Pos.ToVec2();
    }
    #endregion
    
}
