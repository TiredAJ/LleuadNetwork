using Godot;

using LleuadNetworkSim.Scripts;

public partial class WinDetailsList : ItemList
{
    [Export]
    private win_DetailView ParentDetailView = null!;

    public WinDetailsList() {
        this.ItemSelected += RecordSelected;
    }

    private void RecordSelected(long _Index) {
        ParentDetailView.SelectRecord((int)_Index);
    }
}
