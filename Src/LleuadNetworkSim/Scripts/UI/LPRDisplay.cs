using Godot;
using Godot.Logging;

using LleuadNetworkSim.Models.Repo.Entities;
using LleuadNetworkSim.Utils;

namespace LleuadNetworkSim.Scripts.UI;

public partial class LPRDisplay : BaseRecordDisplay
{
    [Export]
    private LineEdit RecordID = null!;
    [Export]
    private LineEdit NodeID = null!;
    [Export]
    private RichTextLabel Action = null!;
    [Export]
    private RichTextLabel Information = null!;

    public override void SetData(IBaseRecord? _BaseRecord) {
        
        if (_BaseRecord is not LuaProcessRecord LPR)
        {
            GodotLogger.LogWarning("Couldn't cast baserecord to LPR");
            return;
        }
        
        RecordID.Text = LPR.ID?.ToString().Or("");
        NodeID.Text = LPR.NodeID.Or("Not given");
        Action.Text = LPR.Action!.Or("");
        Information.Text = LPR.Information!.Or("");
    }
}
