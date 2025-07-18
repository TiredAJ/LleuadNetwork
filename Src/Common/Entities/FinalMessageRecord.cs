using Common.Messaging;

using LiteDB;

namespace Common.Entities;

/// <summary>
/// Records the state of a message once it's dropped/consumed.
/// </summary>
public record FinalMessageRecord : BaseRecord
{
    [BsonIgnore]
    public const string COLL_NAME = "FinalMessageCollection";
    
    [BsonId]
    public ObjectId ID { get; init; }
    [BsonRef(ChallengeRecord.COLL_NAME)]
    public ChallengeRecord? Challenge { get; set; }
    public string MessageID { get; set; }
    public string Sender { get; set; }
    public string Destination { get; set; }
    public string FinalDestination { get; set; }
    public string Type { get; set; }
    public int Index { get; set; }
    public DateTime CreationTime { get; set; }
    public TimeSpan LifeSpan { get; set; }
    public TimeSpan ActiveTime { get; set; }
    public DateTime EndTime { get; set; }
    public int Hops { get; set; }
    public bool Consumed { get; set; }

    public FinalMessageRecord(){}

    public FinalMessageRecord(Message _Msg, string _CurLoc, bool _Consumed, ChallengeRecord? _Challenge) : base() {
        Challenge = _Challenge;
        MessageID = _Msg.ID;
        Sender = _Msg.SenderAddress;
        Destination = _Msg.DestinationAddress;
        FinalDestination = _CurLoc;
        Type = _Msg.MessageType;
        Index = _Msg.Index;
        CreationTime = _Msg.CreationTime;
        LifeSpan = _Msg.Lifespan;
        ActiveTime = _Msg.GetAliveTime();
        EndTime = DateTime.UtcNow;
        Hops = _Msg.Hops;
        Consumed = _Consumed;
    }
};
