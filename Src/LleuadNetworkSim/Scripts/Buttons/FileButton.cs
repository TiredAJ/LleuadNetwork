using Godot;

namespace LleuadNetworkSim.Scripts.Buttons;

public abstract partial class FileButton : Button
{
    [Export]
    protected FileDialog FD = null!;
    
    public override void _Pressed() {

        FD.Popup();
        
        base._Pressed();
    }
}
