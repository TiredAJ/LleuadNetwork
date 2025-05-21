using Godot;

using LleuadNetworkSim.Scripts.Nodes;

namespace LleuadNetworkSim.Scripts.Buttons;

public partial class btn_RunChallenge : Button
{
    [Export]
    private CollectionNode CollNode = null!;

    [Export]
    private cntr_MainButtons ButtonParent = null!;

    public override void _Pressed() {

        _ = CollNode.RunChallenge();
        
        ButtonParent.ClearMode();
        
        base._Pressed();
    }
}