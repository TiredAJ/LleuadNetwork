using System.Diagnostics;

using Godot;

public partial class win_DetailView : Window
{
    public override void _Ready() {
        
        Debug.WriteLine("Hello from other widow!");
        
        base._Ready();
    }
}
