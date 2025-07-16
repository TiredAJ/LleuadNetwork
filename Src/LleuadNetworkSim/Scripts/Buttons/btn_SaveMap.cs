using Godot;

namespace LleuadNetworkSim.Scripts.Buttons;

public partial class btn_SaveMap : FileButton
{
    [Export]
    private Nodes.CollectionNode CollNode = null!;

    public override void _Ready() {

        FD.FileMode = FileDialog.FileModeEnum.SaveFile;

        FD.FileSelected += CollNode.SaveMap;

        base._Ready();
    }
}