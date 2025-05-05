using Godot;

namespace LleuadNetworkSim.Scripts.Buttons;

public partial class btn_Send : Button
{
    [Export]
    private Nodes.CollectionNode CollectionNode = null!;
    
    public override void _Pressed() {
        
        CollectionNode.TrySendMessage();
        
        base._Pressed();
    }
}