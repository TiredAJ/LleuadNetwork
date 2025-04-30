using System.Text.Json.Nodes;

namespace LleuadNetworkSim.Scripts.Nodes;

public interface IPersistable
{
    public JsonObject Save();
    public void Load(JsonObject _JData);
}
