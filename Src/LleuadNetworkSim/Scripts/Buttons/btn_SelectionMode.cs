using Godot;

using LleuadNetworkSim.Scripts.Nodes;

public partial class btn_SelectionMode : Button
{
    [Export]
    private SelectionButtonContainer SelectionContainer;

    [Export]
    private CollectionNode CollNode;

    public override void _Toggled(bool _ToggledOn) {

        SelectionContainer.ToggleSelectionMode(_ToggledOn);

        CollNode.SelectionMode(_ToggledOn);
        
        base._Toggled(_ToggledOn);
    }
}
