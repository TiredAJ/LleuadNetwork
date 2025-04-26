using Godot;
using System;

public partial class btn_LoadMap : Button
{
    [Export]
    private CollectionNode CollNode;

    public override void _Pressed() {

        CollNode.LoadMap();
        
        base._Pressed();
    }
}
