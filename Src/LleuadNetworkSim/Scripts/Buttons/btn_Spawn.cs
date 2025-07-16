using Godot;

using LleuadNetworkSim.Scripts.Nodes;

namespace LleuadNetworkSim.Scripts.Buttons;

public partial class btn_Spawn : Button
{
    [Export]
    private CollectionNode CollNode = null!;

    [Export]
    private cntr_MainButtons ButtonParent = null!;

    public override void _Toggled(bool _ToggledOn) {

        SwitchToggle(_ToggledOn);

        base._Toggled(_ToggledOn);
    }

    private void SwitchToggle(bool _Toggled) {
        CollNode.Mode = _Toggled ? UIMode.SPAWNING : UIMode.NONE;

        if (_Toggled)
        { ButtonParent.SpawnMode(); }
    }
}
