using Godot;

using LleuadNetworkSim.Scripts.Nodes;

namespace LleuadNetworkSim.Scripts.Buttons;

public partial class btn_Delete : Button
{
    [Export]
    private CollectionNode CollectionNode = null!;

    public override void _Pressed() {

        CollectionNode.HandleDelete();

        base._Pressed();
    }
}