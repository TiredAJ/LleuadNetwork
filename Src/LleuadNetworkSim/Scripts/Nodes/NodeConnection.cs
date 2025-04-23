using Godot;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

public partial class NodeConnection : Path2D
{
    [Export]
    public Liner Liner;
    
    public NetworkNode NodeA;
    public NetworkNode NodeB;

    private Vector2 PointAPrev;
    private Vector2 PointBPrev;

    public int FollowerCount = 0;
    public float Length = 0;

    public void Init(NetworkNode _A, NetworkNode _B) {
        NodeA = _A;
        NodeB = _B;

        if (IsNodeReady())
        {
            Debug.WriteLine("resetting curve!");
            
            ResetCurve();
            
            Length = ToLocal(NodeA.GlobalPosition).DistanceTo(ToLocal(NodeB.GlobalPosition));
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
    
    public override void _ExitTree() {

        foreach (var Child in GetChildren())
        { Child.QueueFree(); }
        
        base._ExitTree();
    }

    private void ResetCurve() {        
        Curve.ClearPoints();

        Vector2 PointA = ToLocal(NodeA.GlobalPosition); 
        Vector2 PointB = ToLocal(NodeB.GlobalPosition);

        if (PointA != PointAPrev || PointB != PointBPrev)
        {
            Length = PointA.DistanceTo(PointB);
            UpdateFollowers();
        }
        
        Curve.AddPoint(PointA);
        Curve.AddPoint(PointB);
        
        Liner.UpdatePoints(PointA, PointB);

        PointAPrev = PointA;
        PointBPrev = PointB;
    }

    private void UpdateFollowers() {
        if (FollowerCount > 0)
        {
            foreach (Packet P in GetChildren().Where(X => X is Packet))
            { P.PathUpdated(Length); }
        }
    }

    public void PacketArrived() {
        FollowerCount--;

        Task.Run(() => NodeB.PacketReceived());
    }
}
