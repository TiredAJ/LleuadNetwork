using LiteDB;

namespace LleuadNetworkSim.Models.Repo;

public record LuaProcessRecord : BaseRecord
{
    [BsonId]
    public ObjectId? ID { get; set; }
    public string NodeID { get; set; }
    public string? Action { get; set; }
    public string? Information { get; set; }

    public override string ToString()
        => $"[{NodeID} @ {RecordCreationTime:s}]: {Action ?? ""} - {Information ?? ""}";
}
