using Godot;

public partial class btn_Ok_ExceptionPopup : Button
{
    [Export]
    private Popup ParentPopup;

    public override void _Pressed() {
        
        ParentPopup.QueueFree();
        
        base._Pressed();
    }
}
