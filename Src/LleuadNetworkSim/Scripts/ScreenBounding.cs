using Godot;
using System;

public partial class ScreenBounding : StaticBody2D
{
    private int Padding = 1;
    public override void _Ready()
    {
        GetWindow().SizeChanged += Resized;

        Resized();
        
        base._Ready();
    }

    private void Resized()
    {
        Vector2 Size = GetViewportRect().Size;

        foreach (var Child in GetChildren())
        {
            RemoveChild(Child);
            Child.QueueFree();
        }
        
        (Vector2 Vec, float Padding)[] Bounds = [
            (new Vector2(1, 0), 0),
            (new Vector2(-1, 0), -Size.X + Padding),
            (new Vector2(0, 1), 0),
            (new Vector2(0, -1), -Size.Y + Padding),
        ];        

        foreach (var Bound in Bounds)
        {
            AddChild(new CollisionShape2D()
                { 
                    Shape = new WorldBoundaryShape2D()
                    {
                        Normal = Bound.Vec,
                        Distance = Bound.Padding
                    } 
                }
            );            
        }
    }
}
