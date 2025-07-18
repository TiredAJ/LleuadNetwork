using LiteDB;

namespace Common.Entities;

public record ChallengeRecord : BaseRecord
{
    [BsonIgnore]
    public const string COLL_NAME = "ChallengeCollection";
    
    [BsonId]
    public ObjectId? ID { get; init; }
    public int TotalNodesInvolved { get; set; }
    public int TotalMessagesInvolved { get; set; }
    public string ChallengeName { get; set; }
    public DateTime StartTime { get; set; }
    public TimeSpan RunTime { get; set; } = TimeSpan.Zero;

    public ChallengeRecord(){}

    public ChallengeRecord(int _TotalNodesInvolved, int _TotalMessagesInvolved, string _ChallengeName) : base() {
        TotalNodesInvolved = _TotalNodesInvolved;
        TotalMessagesInvolved = _TotalMessagesInvolved;
        ChallengeName = _ChallengeName;
    }
};
