using Godot;
using Godot.Logging;

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

    public override void SetData(IBaseRecord? _BaseRecord) {

        if (_BaseRecord is not FinalMessageRecord FMR)
        {
            GodotLogger.LogWarning("Couldn't cast baserecord to MJR");
            return;
        }

        MessageID.Text = FMR.MessageID;
        Sender.Text = FMR.Sender.Or("Missing Sender.");
        Destination.Text = FMR.Destination.Or("Missing Destination.");
        FinalDestination.Text = FMR.FinalDestination.Or("Disappeared.");
        Type.Text = FMR.Type.Or("Missing.");
        Index.Text = FMR.Index.ToString();
        CreationTime.Text = FMR.CreationTime.ToString("G");
        LifeSpan.Text = FMR.LifeSpan.ToString("g");
        ActiveTime.Text = FMR.ActiveTime.ToString("g");
        EndTime.Text = FMR.EndTime.ToString("G");
        Hops.Text = FMR.Hops.ToString();
        Consumed.Text = FMR.Consumed ? "true" : "false";
    }
}
