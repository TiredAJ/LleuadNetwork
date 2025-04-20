using Godot;
using System;
using System.Diagnostics;

public partial class NodeConnection : Path2D
{
    [Export]
    public Liner Liner;
    
    public NetworkNode NodeA;
    public NetworkNode NodeB;

    public void Init(NetworkNode _A, NetworkNode _B) {
        NodeA = _A;
        NodeB = _B;

        if (IsNodeReady())
        {
            Debug.WriteLine("reseting curve!");
            
            ResetCurve();
        }
    }
    
    public override void _Ready() {

        this.Curve = new Curve2D();
        
        base._Ready();
    }

    public override void _Process(double delta) {
    
        ResetCurve();
        
        base._Process(delta);
    }

    private void ResetCurve() {        
        Curve.ClearPoints();

        Vector2 PointA = ToLocal(NodeA.GlobalPosition); 
        Vector2 PointB = ToLocal(NodeB.GlobalPosition); 
        
        Curve.AddPoint(PointA);
        Curve.AddPoint(PointB);
        
        Liner.UpdatePoints(PointA, PointB);
    }    
}
