using Godot;

using LleuadNetworkSim.Models.Repo.Entities;

namespace LleuadNetworkSim.Scripts.UI;

public abstract partial class BaseRecordDisplay : HFlowContainer
{
    public void SetData<T>(T _BaseRecord) where T : BaseRecord
    {}
}
