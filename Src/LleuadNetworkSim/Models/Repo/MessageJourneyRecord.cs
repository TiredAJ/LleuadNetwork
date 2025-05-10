using LiteDB;

namespace LleuadNetworkSim.Models.Repo;

/// <summary>
/// A point in the journey of a message. Should be generated on message reception.
/// </summary>
public record MessageJourneyRecord(
    ObjectId? ChallengeID,
    string MessageID,
    string Sender,
    string Destination,
    string Type,
    int Hops,
    string CurrentLocation,
    RecordAction Action
    ) {
    public MessageJourneyRecord(Messaging.Message _Msg, string _CurrentLoc, RecordAction _Action)
        : this(G_ChallengeID, _Msg.ID, _Msg.SenderAddress, _Msg.DestinationAddress, _Msg.MessageType, 
               _Msg.Hops, _CurrentLoc, _Action) {}
};
