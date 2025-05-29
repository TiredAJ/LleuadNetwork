using Godot;
using System;

using LleuadNetworkSim.Scripts;

public partial class btn_Refresh : Button
{
    [Export]
    private win_DetailView DetailView;

    public override void _Pressed() {

        DetailView.Refresh();
        
        base._Pressed();
    }
}
