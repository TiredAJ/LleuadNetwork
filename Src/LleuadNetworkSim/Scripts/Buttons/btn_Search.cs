using Godot;
using System;

using LleuadNetworkSim.Scripts;

public partial class btn_Search : Button
{
    [Export]
    public LineEdit SearchBar = null!;

    [Export]
    public win_DetailView DetailView = null!;
    
    public override void _Pressed() {
        
        string? SearchQ = SearchBar.Text;
        
        DetailView.ChangeSearchCriteria(SearchQ == string.Empty ? null : SearchQ);
        
        base._Pressed();
    }
}
