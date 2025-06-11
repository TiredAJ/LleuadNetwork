using Godot;

namespace LleuadNetworkSim.Scripts.Buttons;

public partial class optbtn_Refresh : OptionButton
{
    [Export]
    private win_DetailView DetailView = null!;
    
    public override void _Ready() {

        ItemSelected += _Index => DetailView.SetAutoRefresh((DetailViewRefreshMode)_Index);
        
        base._Ready();
    }
}

public enum DetailViewRefreshMode
{
    MANUAL,
    ONE_SEC,
    FIVE_SEC,
    TEN_SEC
}
