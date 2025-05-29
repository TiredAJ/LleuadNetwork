using Godot;
using System;

using LleuadNetworkSim.Scripts;

public partial class lstbx_Refresh : OptionButton
{
    [Export]
    private win_DetailView DetailView;

    public override void _Pressed() {

        var Item = GetItemText(Selected);
        
        base._Pressed();
    }

}
