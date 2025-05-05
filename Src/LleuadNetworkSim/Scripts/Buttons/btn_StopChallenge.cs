using Godot;

using LleuadNetworkSim.Scripts.Nodes;

namespace LleuadNetworkSim.Scripts.Buttons;

public partial class btn_StopChallenge : Button
{
    [Export]
    private CollectionNode CollNode = null!;

    [Export]
    private cntr_MainButtons ButtonParent = null!;

    public override void _Pressed() {

        CollNode.StopChallenge();
        
        ButtonParent.ClearMode();
        
        base._Pressed();
    }
}