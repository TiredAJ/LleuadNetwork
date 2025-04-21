using Godot;

using LleuadNetworkSim.Scripts.Nodes;

public partial class btn_Connect : Button
{
    [Export]
    private CollectionNode CollNode;

    public override void _Pressed() {
        
        CollNode.TryConnect();
        
        base._Pressed();
    }
}
