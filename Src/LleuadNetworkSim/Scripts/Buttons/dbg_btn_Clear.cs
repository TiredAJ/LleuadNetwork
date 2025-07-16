using Godot;

namespace LleuadNetworkSim.Scripts.Buttons;

public partial class dbg_btn_Clear : Button
{
    [Export]
    private win_DetailView DetailView = null!;

    public override void _Pressed() {

        DetailView.Clear();

        base._Pressed();
    }
}