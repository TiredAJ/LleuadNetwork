using LiteDB;

namespace LleuadNetworkSim.Models.Repo;

public record LuaProcessRecord
{
    [BsonId]
    public string NodeID { get; set; }
    
    public string? Action { get; set; }
    public string? Information { get; set; }
    
}
