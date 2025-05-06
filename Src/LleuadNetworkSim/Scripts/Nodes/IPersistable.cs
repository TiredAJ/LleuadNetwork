using System.Text.Json.Nodes;

using LleuadNetworkSim.Models.Validation.Json;

namespace LleuadNetworkSim.Scripts.Nodes;

public interface IPersistable
{
    public JsonObject Save();
    public void Load(IBaseVO _VOData);
}
