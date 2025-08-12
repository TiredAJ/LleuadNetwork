using Godot;

using LleuadNetworkSim.Scripts.Nodes;

using static Common.Conf.Conf;

namespace LleuadNetworkSim.Scripts.Buttons;

public partial class btn_LoadChallenge : FileButton
{
    [Export]
    private CollectionNode CollNode = null!;

    public override void _Ready() {

        FD.Filters = [$"*{CHALLENGE_EXTENSION};LleuadNetwork Challenge file"];
        
        #if DEBUG
        FD.CurrentPath = "/home/aj/Repos/LleuadNetwork/Src/LleuadNetworkSim/Misc/ExampleFiles/Challenges";
        #endif
        
        FD.FileSelected += CollNode.TryLoadChallenge;
        
        base._Ready();
    }
}
