using Godot;

using LleuadNetworkSim.Scripts.Nodes;

public partial class btn_Spawn : Button
{
    [Export]
    private CollectionNode CollNode;

    public override void _Toggled(bool _ToggledOn) {

        SwitchToggle(_ToggledOn);
        
        base._Toggled(_ToggledOn);
    }
    
    public void SwitchToggle(bool _Toggled) {
        CollNode.Mode = _Toggled ? UIMode.SPAWNING : UIMode.NONE;
        
        if (_Toggled)
        { GetParent<cntr_MainButtons>().SpawnMode(); }
    }
}
