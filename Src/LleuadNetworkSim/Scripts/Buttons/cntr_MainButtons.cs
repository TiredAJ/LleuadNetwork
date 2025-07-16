using System.Collections.Generic;
using System.Linq;

using Godot;

namespace LleuadNetworkSim.Scripts.Buttons;

public partial class cntr_MainButtons : FlowContainer
{
    [Export]
    private btn_SelectionMode BTN_SelectionMode = null!;

    [Export]
    private btn_Spawn BTN_Spawn = null!;

    public void SelectionMode() {
        BTN_Spawn.ButtonPressed = false;
    }

    public void SpawnMode() {
        BTN_SelectionMode.ButtonPressed = false;
    }

    public void ClearMode() {
        BTN_SelectionMode.ButtonPressed = false;
        BTN_Spawn.ButtonPressed = false;
    }

    private IEnumerable<T> GetChildren<T>() where T : Node
        => GetChildren()
           .Where(X => !X.IsQueuedForDeletion())
           .OfType<T>();
}