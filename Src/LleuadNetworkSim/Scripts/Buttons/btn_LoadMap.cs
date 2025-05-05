using Godot;

namespace LleuadNetworkSim.Scripts.Buttons;

public partial class btn_LoadMap : Button
{
    [Export]
    private Nodes.CollectionNode CollNode = null!;
    
    [Export]
    private FileDialog FD = null!;

    public override void _Ready() {
        
        FD.FileMode = FileDialog.FileModeEnum.OpenFile;

        FD.FileSelected += CollNode.LoadMap;
        
        base._Ready();
    }

    public override void _Pressed() {

        FD.Popup();
        
        base._Pressed();
    }
}