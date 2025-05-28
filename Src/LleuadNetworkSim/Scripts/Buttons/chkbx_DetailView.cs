using Godot;

namespace LleuadNetworkSim.Scripts.Buttons;

public partial class chkbx_DetailView : CheckBox
{
    [Export]
    private win_DetailView DetailView = null!;

    public override void _Pressed() {

        Variant Type = GetMeta("View", "Script_Output");

        DetailView.ChangeView(Type.AsString());
        
        base._Pressed();
    }
}