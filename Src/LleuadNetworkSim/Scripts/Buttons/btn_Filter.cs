using Godot;

using LleuadNetworkSim.Scripts;

public partial class btn_Filter : Button
{
    [Export]
    public LineEdit FilterBar = null!;

    [Export]
    public win_DetailView DetailView = null!;

    public override void _Pressed() {

        string? FilterQ = FilterBar.Text;

        DetailView.ChangeFilterCriteria(FilterQ == string.Empty ? null : FilterQ);

        base._Pressed();
    }
}
