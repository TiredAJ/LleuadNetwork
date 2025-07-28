using System.Text.Json.Serialization;

namespace Common.Challenge;

/// <summary>
/// This represents the types of messages that can be sent. Each has a
/// protocol type, size-range, and percentage completion.
/// <para></para>
/// If an item needs to be split amongst multiple messages, % completion
/// is how many of those messages that need to arrive to be a success. So
/// if an item needs to be split between 20 messages and it's of type
/// <c>VIDEO</c> (80%) then 16 messages of the 20 need to arrive for the
/// item to be successful.
/// </summary>
public class MessageType
{
    required public string Name { get; set; }
    required public ProtocolType Protocol { get; set; }
    required public (long Min, long Max) SizeRange { get; set; }
    required public int CompletionPercentage { get; set; }

    [JsonIgnore]
    public Guid MessageTypeGUID { get; set; } = Guid.CreateVersion7();

    public override string ToString() {
        return $"{Name} ({Protocol.Name}) - [[{SizeRange.Min}B-{SizeRange.Min}B]] - {CompletionPercentage}%";
    }

    static public MessageType Default(int _CompletionPercentage = 100, string _Name = "DEFAULT") => new MessageType() {
        CompletionPercentage = _CompletionPercentage,
        Name = _Name,
        SizeRange = (0, 0),
        Protocol = new ProtocolType() { Name = "DEFAULT", IsOrdered = true }
    };
}
