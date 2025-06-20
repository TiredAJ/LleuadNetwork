using LiteDB;

namespace LleuadNetworkSim.Models.Repo.Entities;

/// <summary>
/// A point in the journey of a message. Should be generated on message reception.
/// </summary>
public record MessageJourneyRecord : BaseRecord {
    public ObjectId? ChallengeID { get; set; }
    public string MessageID { get; set; }
    public string Sender { get; set; }
    public string Destination { get; set; }
    public string Type { get; set; }
    public int Hops { get; set; }
    public string CurrentLocation { get; set; }
    public RecordAction Action { get; set; }

    public MessageJourneyRecord(){}
    
    public MessageJourneyRecord(Messaging.Message _Msg, string _CurrentLoc, RecordAction _Action) : base() {
        ChallengeID = G_ChallengeID;
        MessageID = _Msg.ID;
        Sender = _Msg.SenderAddress;
        Destination = _Msg.DestinationAddress;
        Type = _Msg.MessageType;
        Hops = _Msg.Hops;
        CurrentLocation = _CurrentLoc;
        Action = _Action;
    }
};
