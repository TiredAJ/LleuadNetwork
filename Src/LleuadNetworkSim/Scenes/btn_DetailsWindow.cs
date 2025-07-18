using Godot;

using LleuadNetworkSim.Scripts.Nodes;

namespace LleuadNetworkSim.Scenes;

public partial class btn_DetailsWindow : Button
{
    [Export]
    private CollectionNode ColNode = null!;

    public override void _Toggled(bool _ToggleState) {

        ColNode.ToggleDetailWindow(_ToggleState);

        base._Toggled(_ToggleState);
    }
}
