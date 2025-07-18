using Common.Entities;

using Godot;
using Godot.Logging;

namespace LleuadNetworkSim.Scripts.UI;

public partial class ChallengeDisplay : BaseRecordDisplay
{
    [Export]
    private LineEdit ChallengeName = null!;
    [Export]
    private LineEdit NodeCount = null!;
    [Export]
    private LineEdit MessageCount = null!;
    [Export]
    private LineEdit StartTime = null!;
    [Export]
    private LineEdit RunTime = null!;

    public override void SetData(IBaseRecord? _BaseRecord) {

        if (_BaseRecord is not ChallengeRecord CR)
        {
            GodotLogger.LogWarning("Couldn't cast baserecord to LPR");
            return;
        }

        ChallengeName.Text = CR.ChallengeName;
        NodeCount.Text = CR.TotalNodesInvolved.ToString();
        MessageCount.Text = CR.TotalMessagesInvolved.ToString();
        StartTime.Text = CR.StartTime.ToString("G");
        RunTime.Text = CR.RunTime.ToString("g");
    }
}
