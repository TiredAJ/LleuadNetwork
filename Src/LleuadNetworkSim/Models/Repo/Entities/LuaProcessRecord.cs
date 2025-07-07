using LiteDB;

using LleuadNetworkSim.Config;

namespace LleuadNetworkSim.Models.Repo.Entities;

public record LuaProcessRecord : BaseRecord
{
    [BsonId]
    public ObjectId? ID { get; init; }
    [BsonRef(DBConf.CHALLENGE_COLL_NAME)]
    public ChallengeRecord? Challenge { get; set; }
    required public string NodeID { get; init; }
    public string? Action { get; init; }
    public string? Information { get; init; }

    public override string ToString()
        => $"[{NodeID} @ {RecordCreationTime:s}]: {Action ?? ""} - {Information ?? ""}";
}
