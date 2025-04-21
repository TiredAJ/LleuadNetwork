using Godot;
using System.Linq;

public partial class SelectionButtonContainer : FlowContainer
{
    public void ToggleSelectionMode(bool _Toggled) {
        foreach (Button Children in GetChildren().Where(X => X is Button))
        { Children.Disabled = !_Toggled; }
    }
}
