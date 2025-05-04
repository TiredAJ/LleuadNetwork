using Godot;
using System;

public partial class btn_Send : Button
{
    [Export]
    public LleuadNetworkSim.Scripts.Nodes.CollectionNode CollectionNode;
    
    public override void _Pressed() {
        
        CollectionNode.TrySendMessage();
        
        base._Pressed();
    }
}
