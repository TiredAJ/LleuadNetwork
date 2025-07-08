
using Godot;
using Godot.Logging;

using LleuadNetworkSim.Models.Repo.Entities;
using LleuadNetworkSim.Utils;

namespace LleuadNetworkSim.Scripts.UI;

public partial class MessageJourneyDisplay : BaseRecordDisplay
{
    [Export]
    private LineEdit ChallengeName = null!;
    [Export]
    private LineEdit MessageID = null!;
    [Export]
    private LineEdit Sender = null!;
    [Export]
    private LineEdit Destination = null!;
    [Export]
    private LineEdit MessageType = null!;
    [Export]
    private LineEdit CurrentLocation = null!;
    [Export]
    private LineEdit Action = null!;
    [Export]
    private LineEdit Hops = null!;

    public override void SetData(IBaseRecord? _BaseRecord) {
        
        if (_BaseRecord is not MessageJourneyRecord MJR)
        {
            GodotLogger.LogWarning("Couldn't cast baserecord to MJR");
            return;
        }
        
        ChallengeName.Text = MJR.Challenge?.ChallengeName.Or("Not in challenge");
        MessageID.Text = MJR.MessageID;
        Sender.Text = MJR.Sender.Or("No sender");
        Destination.Text = MJR.Destination.Or("No destination");
        MessageType.Text = MJR.Type.Or("Unknown type");
        CurrentLocation.Text = MJR.CurrentLocation.Or("Unknown location");
        Action.Text = MJR.Action.ToStr();
    }
}
