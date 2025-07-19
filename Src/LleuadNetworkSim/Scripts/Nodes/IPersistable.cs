using System.Text.Json.Nodes;

using Common.Json;

namespace LleuadNetworkSim.Scripts.Nodes;

public interface IPersistable
{
    public JsonObject Save();
    public void Load(IBaseVO _VOData);
}
