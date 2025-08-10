using Godot;

using LleuadNetworkSim.Scripts.Nodes;

namespace LleuadNetworkSim.Scripts.Buttons;

public partial class btn_LoadScript : FileButton
{
    [Export]
    private CollectionNode CollNode = null!;

    public override void _Ready() {
       
        #if DEBUG
        FD.CurrentPath = "/home/aj/Repos/LleuadNetwork/Src/LleuadNetworkSim/Misc/ExampleFiles/LuaFiles";
        #endif
        
        FD.FilesSelected += CollNode.LoadScript;
        FD.FileSelected += _Path => CollNode.LoadScript(_Path);
        FD.DirSelected += _Path => CollNode.LoadScript(_Path);

        base._Ready();
    }

}
