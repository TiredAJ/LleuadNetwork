using Godot;

using LleuadNetworkSim.Models.Repo.Entities;

namespace LleuadNetworkSim.Scripts.UI;

public abstract partial class BaseRecordDisplay<T> : Control where T : BaseRecord
{
    public void SetData(T _BaseRecord){}
}
