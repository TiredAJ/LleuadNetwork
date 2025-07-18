using Godot;

using LleuadNetworkSim.Scripts.Nodes;

namespace LleuadNetworkSim.Scripts.Buttons;

public partial class btn_Connect : Button
{
    [Export]
    private CollectionNode CollNode = null!;

    public override void _Pressed() {

        CollNode.TryConnect();

        base._Pressed();
    }
}