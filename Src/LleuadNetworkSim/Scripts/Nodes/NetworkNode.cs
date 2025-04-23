using Godot;
using System.Collections.Generic;
using System.Diagnostics;

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
    private PackedScene PacketTemplate;

    private CollectionNode CollNodeParent;

    private Dictionary<string, NodeConnection> Connections = [];
    
    public override void _Ready() {
        
        Debug.WriteLine($"Node spawned at {Position}");

        CollNodeParent = GetParent<CollectionNode>();
        
        base._Ready();
    }

    public override void _UnhandledInput(InputEvent @event)
    {        
        if (@event is InputEventMouseButton && !@event.IsPressed())
        { Lifted = false; }

        if (Lifted && @event is InputEventMouseMotion IEMM)
        { Position += IEMM.Relative; }
        
        base._UnhandledInput(@event);
    }

    public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx)
    {
        if (@event is InputEventMouseButton IEMM && @event.IsPressed())
        {
            if (IEMM.ButtonIndex == MouseButton.Middle)
            { Lifted = true; }
            else if (IEMM.ButtonIndex == MouseButton.Left)
            { RequestSelection(); }                 
            
            Debug.WriteLine("Clicked");
        }
        
        base._InputEvent(viewport, @event, shapeIdx);
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
}
