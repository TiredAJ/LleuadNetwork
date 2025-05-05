using Godot;

namespace LleuadNetworkSim.Scripts.Buttons;

public partial class btn_SaveMap : Button
{
    [Export]
    private Nodes.CollectionNode CollNode = null!;
    
    [Export]
    private FileDialog FD = null!;

    public override void _Ready() {

        FD.FileMode = FileDialog.FileModeEnum.SaveFile;
        
        FD.FileSelected += CollNode.SaveMap;
        
        base._Ready();
    }

    public override void _Pressed() {

        FD.Popup();
        
        base._Pressed();
    }
}