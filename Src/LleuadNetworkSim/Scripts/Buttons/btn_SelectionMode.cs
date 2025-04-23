using Godot;

using LleuadNetworkSim.Scripts.Nodes;

public partial class btn_SelectionMode : Button
{
    [Export]
    private SelectionButtonContainer SelectionContainer;

    [Export]
    private CollectionNode CollNode;

    public override void _Toggled(bool _ToggledOn) {

        SwitchToggle(_ToggledOn);
        
        base._Toggled(_ToggledOn);
    }
    
    public void SwitchToggle(bool _Toggled) {
        SelectionContainer.ToggleSelectionMode(_Toggled);

        CollNode.SelectionMode(_Toggled);

        if (_Toggled)
        { GetParent<cntr_MainButtons>().SelectionMode(); }
    }
}
