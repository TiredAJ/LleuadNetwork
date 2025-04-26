using Godot;
using System;

public partial class btn_SaveMap : Button
{
    [Export]
    private CollectionNode CollNode;
    
    public override void _Pressed() {

        CollNode.Save();
        
        base._Pressed();
    }
}
