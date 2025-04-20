using Godot;
using System;

public partial class btn_Connect : Button
{
    [Export]
    public CollectionNode CollectionNode;

    public override void _Pressed() {
        
        CollectionNode.TryConnect();
        
        base._Pressed();
    }
}
