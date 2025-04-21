using Godot;
using System;

public partial class btn_Delete : Button
{
    [Export]
    public CollectionNode CollectionNode;
    
    public override void _Pressed() {
        
        CollectionNode.HandleDelete();
        
        base._Pressed();
    }
}
