using Godot;

using LleuadNetworkSim.Scripts.Nodes;

public partial class MainAreaButton : Button
{
    [Export]
    private LleuadNetworkSim.Scripts.Nodes.CollectionNode CollNode;

    public override void _Ready() {
        CollNode = GetParent<LleuadNetworkSim.Scripts.Nodes.CollectionNode>();
        
        base._Ready();
    }
    
    public override void _GuiInput(InputEvent @event) {
        
        if (CollNode.Mode != UIMode.SPAWNING)
        { return; }

        if (@event is InputEventMouseButton IEMB && IEMB.Pressed && IEMB.ButtonIndex == MouseButton.Left)
        { CollNode.SpawnNode(IEMB.Position); }
        
        base._GuiInput(@event);
    }
}
