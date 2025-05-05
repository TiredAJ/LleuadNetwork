using System.Collections.Generic;
using System.Linq;

using Godot;

namespace LleuadNetworkSim.Scripts.Buttons;

public partial class SelectionButtonContainer : FlowContainer
{
    public void ToggleSelectionMode(bool _Toggled) {
        foreach (Button Children in GetChildren<Button>())
        { Children.Disabled = !_Toggled; }
    }
    
    private IEnumerable<T> GetChildren<T>() where T : Node
        => GetChildren()
           .Where(X => !X.IsQueuedForDeletion())
           .OfType<T>();
}