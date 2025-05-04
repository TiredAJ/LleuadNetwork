using Godot;

public partial class btn_LoadMap : Button
{
    [Export]
    private LleuadNetworkSim.Scripts.Nodes.CollectionNode CollNode;
    
    [Export]
    private FileDialog FD;

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
