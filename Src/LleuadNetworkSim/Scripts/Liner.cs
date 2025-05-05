using Godot;

namespace LleuadNetworkSim.Scripts;

public partial class Liner : Line2D
{
    private Vector2 PointA;
    private Vector2 PointB;

    public override void _Ready() {
        DefaultColor = Color.Color8(0, 0, 0, 255);
        Width = 10f;
        
        base._Ready();
    }

    public void UpdatePoints(Vector2 _PointA, Vector2 _PointB) {
        PointA = _PointA;
        PointB = _PointB;
        
        _Draw();
    }

    public override void _Draw() {

        Points = [PointA, PointB];
        
        base._Draw();
    }
}