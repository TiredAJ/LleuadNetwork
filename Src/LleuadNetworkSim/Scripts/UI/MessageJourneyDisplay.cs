
using Godot;

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

    public new void SetData(MessageJourneyRecord _MJR) {
        ChallengeName.Text = _MJR.Challenge?.ChallengeName.Or("Not in challenge");
        MessageID.Text = _MJR.MessageID;
        Sender.Text = _MJR.Sender.Or("No sender");
        Destination.Text = _MJR.Destination.Or("No destination");
        MessageType.Text = _MJR.Type.Or("Unknown type");
        CurrentLocation.Text = _MJR.CurrentLocation.Or("Unknown location");
        Action.Text = _MJR.Action.ToStr();
    }
}
