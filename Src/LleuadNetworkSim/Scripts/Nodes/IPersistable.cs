using System.Text.Json.Nodes;

namespace LleuadNetworkSim.Scripts.Nodes;

public interface IPersistable
{
    public JsonNode Save();
    public void Load(JsonNode _JData);
}
