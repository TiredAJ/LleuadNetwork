using Godot;

using LleuadNetworkSim.Scripts.Nodes;

using static Common.Conf.Conf;

namespace LleuadNetworkSim.Scripts.Buttons;

public partial class btn_LoadMap : FileButton
{
    [Export]
    private CollectionNode CollNode = null!;

    public override void _Ready() {

        FD.FileMode = FileDialog.FileModeEnum.OpenFile;
        
        FD.Filters = [$"*{MAP_EXTENSION};LleuadNetwork Map file"];
        
        #if DEBUG
        FD.CurrentPath = "/home/aj/Repos/LleuadNetwork/Src/LleuadNetworkSim/Misc/ExampleFiles/Maps/";
        #endif

        FD.FileSelected += CollNode.LoadMapFromFile;

        base._Ready();
    }
}
