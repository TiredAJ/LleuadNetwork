using System.Text.Json.Serialization;

using Common.Json;
using Common.Messaging;

namespace Common.Challenge;

public class ChallengeVO
{
    /// <summary>
    /// The amount of nodes involved in this challenge.
    /// </summary>
    public int NodeCount { get; set; } = -1;

    /// <summary>
    /// The amount of messages involved in this challenge.
    /// </summary>
    public int ItemCount { get; set; } = -1;

    /// <summary>
    /// A map of message types to their distribution.
    /// </summary>
    [JsonIgnore]
    public Dictionary<MessageType, int> MessageDistribution { get; set; } = [];

    /// <summary>
    /// For serialisation use ONLY.
    /// </summary>
    public Dictionary<Guid, (MessageType, int)> SerialisableDict {
        get => MessageDistribution.ToDictionary(Key => Key.Key.MessageTypeGUID, Val => (MessageType: Val.Key, Distribution: Val.Value)); 
        set => MessageDistribution = value.ToDictionary(Key => Key.Value.Item1, Val => Val.Value.Item2);
    }

    /// <summary>
    /// The map used for this challenge.
    /// </summary>
    public MapVO Map { get; set; } = null!;

    /// <summary>
    /// The name of this challenge.
    /// </summary>
    public string Name { get; set; } = "DEFAULT";

    [JsonIgnore]
    public Dictionary<string, List<Message>> GeneratedMessages { get; set; } = [];
}
