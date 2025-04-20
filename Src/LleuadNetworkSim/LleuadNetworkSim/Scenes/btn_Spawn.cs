using Godot;
using System;

public partial class btn_Spawn : Button
{
    [Export]
    private Node NodeCollection;
    
    [Export]
    private PackedScene NodeScene;

    public override void _Pressed() {

        NetworkNode SceneInstance = NodeScene.Instantiate() as NetworkNode;

        SceneInstance.Lifted = true;
        SceneInstance.Position = GetViewport()
            .GetMousePosition();
        SceneInstance.Name = $"NetworkNode-" + Guid.NewGuid().ToString();
        
        NodeCollection.AddChild(SceneInstance);
        
        base._Pressed();
    }
}
