using Godot;

using LleuadNetworkSim.Scripts.Nodes;

namespace LleuadNetworkSim.Scripts.Buttons;

public partial class btn_LoadMap : FileButton
{
    [Export]
    private CollectionNode CollNode = null!;

    public override void _Ready() {

        FD.FileMode = FileDialog.FileModeEnum.OpenFile;

        FD.FileSelected += CollNode.LoadMap;

        base._Ready();
    }
}