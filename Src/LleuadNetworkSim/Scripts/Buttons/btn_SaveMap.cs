using Godot;

using LleuadNetworkSim.Scripts.Nodes;

namespace LleuadNetworkSim.Scripts.Buttons;

public partial class btn_SaveMap : FileButton
{
    [Export]
    private CollectionNode CollNode = null!;

    public override void _Ready() {

        FD.FileMode = FileDialog.FileModeEnum.SaveFile;

        FD.FileSelected += CollNode.SaveMap;

        base._Ready();
    }
}