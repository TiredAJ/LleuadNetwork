using Godot;

using LleuadNetworkSim.Scripts.Nodes;

namespace LleuadNetworkSim.Scripts.Buttons;

public partial class btn_SelectionMode : Button
{
    [Export]
    private SelectionButtonContainer SelectionContainer = null!;

    [Export]
    private CollectionNode CollNode = null!;

    [Export]
    private cntr_MainButtons ButtonParent = null!;

    public override void _Toggled(bool _ToggledOn) {

        SwitchToggle(_ToggledOn);

        base._Toggled(_ToggledOn);
    }

    public void SwitchToggle(bool _Toggled) {
        SelectionContainer.ToggleSelectionMode(_Toggled);

        CollNode.SelectionMode(_Toggled);

        if (_Toggled)
        { ButtonParent.SelectionMode(); }
    }
}