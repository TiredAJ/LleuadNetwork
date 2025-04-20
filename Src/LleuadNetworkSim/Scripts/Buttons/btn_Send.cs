using Godot;
using System;

public partial class btn_Send : Button
{
    [Export]
    public CollectionNode CollectionNode;
    
    public override void _Pressed() {
        
        CollectionNode.TrySendMessage();
        
        base._Pressed();
    }
}
