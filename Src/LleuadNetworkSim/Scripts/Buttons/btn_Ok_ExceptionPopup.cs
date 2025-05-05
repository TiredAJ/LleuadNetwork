using Godot;

namespace LleuadNetworkSim.Scripts.Buttons;

public partial class btn_Ok_ExceptionPopup : Button
{
    [Export]
    private Popup ParentPopup = null!;

    public override void _Pressed() {
        
        ParentPopup.QueueFree();
        
        base._Pressed();
    }
}