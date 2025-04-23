using Godot;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

public partial class NetworkNode : CharacterBody2D
{
    [Export]
    public bool Lifted = false;

    [Export]
    public bool Selected {
        get;
        set {
            field = value;
            SelectionRing.Visible = Selected;
        }
    } = false;

    [Export]
    private Sprite2D SelectionRing;

    [Export]
    private Label NameLabel;
    
    [Export]
    private PackedScene PacketTemplate;

    private CollectionNode CollNodeParent;

    private Dictionary<string, NodeConnection> Connections = [];
    
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

    public void AddConnection(string _ID, NodeConnection _Conn) {
        Connections.Add(_ID, _Conn);
    }

    public void SendMessage(string _ID) {

        if (!Connections.ContainsKey(_ID))
        { return; }
        
        Packet Message = PacketTemplate.Instantiate<Packet>();

        Message.ZIndex -= Connections[_ID].FollowerCount; 
        
        Connections[_ID].AddChild(Message);

        Connections[_ID].FollowerCount++;
    }

    private void RequestSelection() {
        if (Lifted)
        { return; }
        
        Selected = CollNodeParent.RequestSelection(this);
    }

    public async Task PacketReceived() {
        /* handle packet - read channel */
        
        Debug.WriteLine($"Packet received at {this.Name}");
    }
}
