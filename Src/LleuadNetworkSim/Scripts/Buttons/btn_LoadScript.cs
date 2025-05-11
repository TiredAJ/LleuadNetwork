using Godot;

using LleuadNetworkSim.Scripts.Nodes;

namespace LleuadNetworkSim.Scripts.Buttons;

public partial class btn_LoadScript : FileButton
{
    [Export]
    private CollectionNode CollNode = null!;

    public override void _Ready() {

        FD.FilesSelected += CollNode.LoadScript;
        FD.FileSelected += _Path => CollNode.LoadScript(_Path);
        FD.DirSelected += _Path => CollNode.LoadScript(_Path);
        
        base._Ready();
    }
    
}