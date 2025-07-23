using Common.Challenge;
using Common.Json;

namespace ChallengeGenerator;

public class ChallengeData
{
    public int NodeCount = -1;
    public int MessageCount = -1;
    //A map of message types to their distribution
    public Dictionary<MessageType, int> MessageDistribution = [];
    public MapVO? Map = null;
}
