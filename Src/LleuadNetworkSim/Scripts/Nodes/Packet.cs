using System.Diagnostics;

using Godot;

namespace LleuadNetworkSim.Scripts.Nodes;

public partial class Packet : PathFollow2D
{
    [Export]
    private float Speed = 150f; //250

    private float PathLength = -1;
    
    private bool Run = false;

    public override void _EnterTree() {

        Node? Parent = GetParent();
        
        if (Parent is not null)
        {
            Run = true;

            PathLength = (Parent as NodeConnection)!.Length;
        }
        
        GetChild<Sprite2D>(0)
            .SetGlobalRotationDegrees(0);
        
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
}