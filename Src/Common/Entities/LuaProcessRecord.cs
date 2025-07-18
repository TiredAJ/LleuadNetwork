using LiteDB;

namespace Common.Entities;

public record LuaProcessRecord : BaseRecord
{
    [BsonIgnore]
    public const string COLL_NAME = "LPRCollection";
    
    [BsonId]
    public ObjectId? ID { get; init; }
    [BsonRef(ChallengeRecord.COLL_NAME)]
    public ChallengeRecord? Challenge { get; set; }
    required public string NodeID { get; init; }
    public string? Action { get; init; }
    public string? Information { get; init; }

    public override string ToString()
        => $"[{NodeID} @ {RecordCreationTime:s}]: {Action ?? ""} - {Information ?? ""}";
}
