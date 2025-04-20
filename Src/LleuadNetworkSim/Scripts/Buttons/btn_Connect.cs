using Godot;
using System;

using LleuadNetworkSim.Scripts.Nodes;

public partial class btn_Connect : Button
{
    [Export]
    private CollectionNode CollNode;

    public override void _Pressed() {
        
        if (CollNode.Mode == UIMode.CONNECTING)
        { CollNode.Mode = UIMode.NONE; }
        else
        { CollNode.Mode = UIMode.CONNECTING; }
        
        base._Pressed();
    }
}
