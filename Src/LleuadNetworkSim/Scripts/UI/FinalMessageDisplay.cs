using Godot;

using LleuadNetworkSim.Models.Repo.Entities;
using LleuadNetworkSim.Utils;

namespace LleuadNetworkSim.Scripts.UI;

public partial class FinalMessageDisplay : BaseRecordDisplay
{
    [Export]
    private LineEdit MessageID = null!;
    [Export]
    private LineEdit Sender = null!;
    [Export]
    private LineEdit Destination = null!;
    [Export]
    private LineEdit FinalDestination = null!;
    [Export]
    private LineEdit Type = null!;
    [Export]
    private LineEdit Index = null!;
    [Export]
    private LineEdit CreationTime = null!;
    [Export]
    private LineEdit LifeSpan = null!;
    [Export]
    private LineEdit ActiveTime = null!;
    [Export]
    private LineEdit EndTime = null!;
    [Export]
    private LineEdit Hops = null!;
    [Export]
    private LineEdit Consumed = null!;

    public FinalMessageRecord InitRecord { set => SetData(value); }
    
    public new void SetData(FinalMessageRecord _FMR) {
        MessageID.Text = _FMR.MessageID;
        Sender.Text = _FMR.Sender.Or("Missing Sender.");
        Destination.Text = _FMR.Destination.Or("Missing Destination.");
        FinalDestination.Text = _FMR.FinalDestination.Or("Disappeared.");
        Type.Text = _FMR.Type.Or("Missing.");
        Index.Text = _FMR.Index.ToString();
        CreationTime.Text = _FMR.CreationTime.ToString("G");
        LifeSpan.Text = _FMR.LifeSpan.ToString("g");
        ActiveTime.Text = _FMR.ActiveTime.ToString("g");
        EndTime.Text = _FMR.EndTime.ToString("G");
        Hops.Text = _FMR.Hops.ToString();
        Consumed.Text = _FMR.Consumed ? "true" : "false";
    }
}
