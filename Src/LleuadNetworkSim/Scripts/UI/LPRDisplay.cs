using Godot;

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

    public new void SetData(LuaProcessRecord _LPR) {
        RecordID.Text = _LPR.ID?.ToString().Or("");
        NodeID.Text = _LPR.NodeID.Or("Not given");
        Action.Text = _LPR.Action!.Or("");
        Information.Text = _LPR.Information!.Or("");
    }
}
