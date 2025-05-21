using System.Collections.Generic;
using System.Diagnostics;

using Godot;
using Godot.Logging;

namespace LleuadNetworkSim.Scripts.Nodes;

public partial class Packet : PathFollow2D
{
    [Export]
    private float Speed = 250f;

    [Export]
    private CompressedTexture2D[] PacketTextures = [];

    [Export]
    private Sprite2D Sprite = null!;

    private float PathLength = -1;
    
    private bool Run = false;

    public override void _EnterTree() {

        Node? Parent = GetParent();
        
        if (Parent is not null)
        {
            Run = true;

            PathLength = (Parent as NodeConnection)!.Length;
        }
        
        Sprite.SetGlobalRotationDegrees(0);
        
        base._EnterTree();
    }

    public override void _Process(double _Delta) {

        if (!Run)
        { return; }
        
        this.Progress += (float)(Speed * _Delta);

        if (ProgressRatio >= 0.98f)
        {
            (GetParent() as NodeConnection)?.PacketArrived();
            
            this.QueueFree();
        }
        
        base._Process(_Delta);
    }

    public void PathUpdated(float _Length) {
        
        PathLength = _Length;

        GetChild<Sprite2D>(0)
            .GlobalRotation = 0;
    }

    public void SetType(string _Type) {

        string Type = _Type.ToLower();
        
        if (Type.Contains("discovery"))
        { Sprite.Texture = PacketTextures[1]; }
        else
        { Sprite.Texture = PacketTextures[0]; }
        
        GodotLogger.LogInfo($"Set type to {_Type}");
    }
}