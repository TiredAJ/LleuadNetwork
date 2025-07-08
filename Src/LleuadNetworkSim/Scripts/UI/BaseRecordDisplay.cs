using Godot;

using LleuadNetworkSim.Models.Repo.Entities;

namespace LleuadNetworkSim.Scripts.UI;

public abstract partial class BaseRecordDisplay : HFlowContainer
{
    public abstract void SetData(IBaseRecord? _BaseRecord);
}
