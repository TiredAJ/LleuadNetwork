using Godot;

using LleuadNetworkSim.Scripts.Nodes;

namespace LleuadNetworkSim.Scripts.Buttons;

public partial class btn_Send : Button
{
    [Export]
    private CollectionNode CollectionNode = null!;

    public override void _Pressed() {

        CollectionNode.TrySendMessage();

        base._Pressed();
    }
}