using Godot;
using System;
using System.Diagnostics;

public partial class Packet : PathFollow2D
{
    [Export]
    public float Speed = 250f;

    private float PathLength;
    
    private bool Run = false;

    public override void _EnterTree() {

        if (GetParent() is not null)
        {
            Run = true;

            PathLength = (GetParent() as NodeConnection).Length;
        }
        
        base._EnterTree();
    }

    public override void _Process(double delta) {

        if (!Run)
        { return; }
        
        this.ProgressRatio += (float)(0.1f * delta);

        if (ProgressRatio >= 0.98f)
        {
            Debug.WriteLine($"Reached end of the line! progress: {this.Progress}, length: {this.PathLength}");

            (GetParent() as NodeConnection).FollowerCount--;
            
            this.QueueFree();            
        }
        
        base._Process(delta);
    }

    public void PathUpdated(float _Length) {
        
        PathLength = _Length;
    }
}
