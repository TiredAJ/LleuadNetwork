using Godot;

namespace LleuadNetworkSim.Scripts.Buttons;

public partial class btn_LoadChallenge : FileButton
{
    [Export]
    private Nodes.CollectionNode CollNode = null!;

    public override void _Ready() {
        
        FD.FileSelected += CollNode.TryLoadChallenge;
        
        base._Ready();
    }
}