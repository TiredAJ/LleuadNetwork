using System;

using LiteDB;

namespace LleuadNetworkSim.Models.Repo;

/// <summary>
/// Records the state of a message once it's dropped/consumed.
/// </summary>
public record FinalMessageRecord(
    ObjectId? ChallengeID,
    string MessageID,
    string Sender,
    string Destination,
    string FinalDestination,
    string Type,
    int Index,
    DateTime CreationTime,
    TimeSpan LifeSpan,
    TimeSpan ActiveTime,
    DateTime EndTime,
    int Hops,
    bool Consumed) {
    public FinalMessageRecord(Message.Message _Msg, string _CurLoc, bool _Consumed)
        : this(G_ChallengeID, _Msg.ID, _Msg.SenderAddress, _Msg.DestinationAddress, _CurLoc,
               _Msg.MessageType, _Msg.Index, _Msg.CreationTime, _Msg.Lifespan, _Msg.GetAliveTime(), 
               DateTime.UtcNow, _Msg.Hops, _Consumed) { }
};
