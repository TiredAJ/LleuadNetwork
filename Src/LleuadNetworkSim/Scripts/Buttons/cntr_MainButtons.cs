using Godot;
using System;
using System.Linq;

public partial class cntr_MainButtons : FlowContainer
{
    private btn_SelectionMode BTN_SelectionMode;
    private btn_Spawn BTN_Spawn;

    public override void _Ready() {

        foreach (Button BTN in GetChildren().Where(X => X is Button))
        {
            if (BTN.GetMeta("Type").AsString() == "Spawn")
            { BTN_Spawn = BTN as btn_Spawn; }
            else if ((BTN.GetMeta("Type").AsString() == "Select"))
            { BTN_SelectionMode = BTN as btn_SelectionMode; }
        }
        
        base._Ready();
    }

    public void SelectionMode() {
        //BTN_Spawn.SwitchToggle();
        BTN_Spawn.ButtonPressed = false;
    }

    public void SpawnMode() {
        //BTN_SelectionMode.SwitchToggle(false);
        BTN_SelectionMode.ButtonPressed = false;
    }
}
