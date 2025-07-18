using Common.Entities;

using Godot;

namespace LleuadNetworkSim.Scripts.UI;

public abstract partial class BaseRecordDisplay : HFlowContainer
{
    public abstract void SetData(IBaseRecord? _BaseRecord);
}
