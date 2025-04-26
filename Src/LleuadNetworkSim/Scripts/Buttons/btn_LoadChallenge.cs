using Godot;

public partial class btn_LoadChallenge : Button
{
    [Export]
    private CollectionNode CollNode;

    [Export]
    private FileDialog FD;

    public override void _Ready() {
        
        FD.FileSelected += CollNode.TryLoadChallenge;
        
        base._Ready();
    }

    public override void _Pressed() {

        FD.Popup();
        
        base._Pressed();
    }
}
