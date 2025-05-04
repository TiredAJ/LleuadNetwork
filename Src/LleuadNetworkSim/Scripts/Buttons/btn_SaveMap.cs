using Godot;
using System;

public partial class btn_SaveMap : Button
{
    [Export]
    private LleuadNetworkSim.Scripts.Nodes.CollectionNode CollNode;
    
    [Export]
    private FileDialog FD;

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
