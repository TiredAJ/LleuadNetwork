using System;

namespace LleuadNetworkSim.Models.Repo;

public record ChallengeRecord(
    int TotalNodesInvolved,
    int TotalMessagesInvolved,
    string ChallengeName,
    DateTime StartTime,
    TimeSpan RunTime = new TimeSpan()) {
    public ChallengeRecord(int _TotalNodesInvolved, int _TotalMessagesInvolved, string _ChallengeName) 
        : this(_TotalNodesInvolved, _TotalMessagesInvolved, _ChallengeName, DateTime.UtcNow) {}
};
