using Godot;

using LleuadNetworkSim.Scripts.Nodes;

namespace LleuadNetworkSim.Scripts.Buttons;

public partial class MainAreaButton : Button
{
    [Export]
    private CollectionNode CollNode = null!;

    public override void _Ready() {
        CollNode = GetParent<CollectionNode>();

        base._Ready();
    }

    public override void _GuiInput(InputEvent _Event) {

        if (CollNode.Mode != UIMode.SPAWNING)
        { return; }

        if (_Event is InputEventMouseButton IEMB && IEMB.Pressed && IEMB.ButtonIndex == MouseButton.Left)
        { CollNode.SpawnNode(IEMB.Position); }

        base._GuiInput(_Event);
    }
}