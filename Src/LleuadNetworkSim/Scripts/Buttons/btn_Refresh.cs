using Godot;

namespace LleuadNetworkSim.Scripts.Buttons;

public partial class btn_Refresh : Button
{
    [Export]
    private win_DetailView DetailView = null!;

    public override void _Pressed() {

        DetailView.Refresh();

        base._Pressed();
    }
}
