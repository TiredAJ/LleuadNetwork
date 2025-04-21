using Godot;

using LleuadNetworkSim.Scripts.Nodes;

public partial class btn_Spawn : Button
{
    [Export]
    private CollectionNode CollNode;
    
    public override void _Pressed() {

        if (CollNode.Mode == UIMode.SPAWNING)
        { CollNode.Mode = UIMode.NONE; }
        else
        { CollNode.Mode = UIMode.SPAWNING; }
        
        base._Pressed();
    }
}
