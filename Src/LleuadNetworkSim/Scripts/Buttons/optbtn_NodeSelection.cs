using Godot;
using Godot.Logging;

namespace LleuadNetworkSim.Scripts.Buttons;

public partial class optbtn_NodeSelection : OptionButton
{
    [Export]
    private win_DetailView DetailView = null!;

    public override void _Ready() {

        ItemSelected += _Index => {
                            if (_Index > int.MaxValue)
                            {
                                GodotLogger.LogWarning("Somehow, the selected index was larger than a max int");
                                return;
                            }

                            DetailView.ChangeSelectedNode(_Index < 0 ? null : GetItemText((int)_Index));
                        };
        
        base._Ready();
    }
}