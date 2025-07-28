using System.Text.Json.Serialization;

using Common.Challenge;
using Common.Messaging;

namespace Common.Json;

[JsonSourceGenerationOptions(WriteIndented = false, IncludeFields = true, UseStringEnumConverter = true)]
[JsonSerializable(typeof(MessageType))]
[JsonSerializable(typeof(ProtocolType))]
[JsonSerializable(typeof(List<MessageType>))]
[JsonSerializable(typeof(ChallengeVO))]
[JsonSerializable(typeof(MapVO))]
[JsonSerializable(typeof(NetworkNodeVO))]
[JsonSerializable(typeof(List<Message>))]
public partial class SourceGenerationContext : JsonSerializerContext
{ }
