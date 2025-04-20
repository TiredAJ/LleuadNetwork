using Godot;
using System;
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

    private Dictionary<string, NodeConnection> Connections = [];
    
    public override void _Ready() {
        
        Debug.WriteLine($"Node spawned at {Position}");
        
        base._Ready();
    }

    public override void _UnhandledInput(InputEvent @event)
    {        
        if (@event is InputEventMouseButton && !@event.IsPressed())
        { Lifted = false; }

        if (Lifted && @event is InputEventMouseMotion IEMM)
        {   
            Position += IEMM.Relative;
            //Debug.WriteLine($"Position set to {Position}");
        }
        
        base._UnhandledInput(@event);
    }

    public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx)
    {
        if (@event is InputEventMouseButton IEMM)
        {
            if (@event.IsPressed())
            {
                if (IEMM.ButtonIndex == MouseButton.Middle)
                { Lifted = true; }
                else if (IEMM.ButtonIndex == MouseButton.Left)
                { ToggleSelected(); }                 
                
                Debug.WriteLine("Clicked");
            }           
        }
        
        base._InputEvent(viewport, @event, shapeIdx);
    }

    public void AddConnection(string _ID, NodeConnection _Conn) {
        Connections.Add(_ID, _Conn);
    }

    public void SendMessage(string _ID) {
        Packet Message = PacketTemplate.Instantiate<Packet>();
        
        Connections[_ID].AddChild(Message);

        Connections[_ID].FollowerCount++;
    }

    private bool HandleUnselected() {
        (GetParent() as CollectionNode).RemoveSelectedNode(this);
        return false;
    }

    private bool HandleSelected() {
        return (GetParent() as CollectionNode).AddSelectedNode(this);
    }

    private void ToggleSelected() {
        if (Lifted == true)
        { return; }
        
        Selected = Selected ? HandleUnselected() : HandleSelected();
    }
}
