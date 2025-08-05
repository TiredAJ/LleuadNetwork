using Common.Conf;

using Godot;

using LleuadNetworkSim.Scripts.Nodes;

namespace LleuadNetworkSim.Scripts.Buttons;

public partial class btn_LoadChallenge : FileButton
{
    [Export]
    private CollectionNode CollNode = null!;

    public override void _Ready() {

        FD.Filters = [$"*{Conf.CHALLENGE_EXTENSION};LleuadNetwork Challenge file"];
        
        FD.FileSelected += CollNode.TryLoadChallenge;
        
        base._Ready();
    }
}
