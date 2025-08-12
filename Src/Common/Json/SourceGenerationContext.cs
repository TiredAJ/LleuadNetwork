using System.Text.Json.Serialization;

using Common.Challenge;
using Common.Messaging;

namespace Common.Json;

[JsonSerializable(typeof(Dictionary<string, List<Message>>))]
[JsonSerializable(typeof(List<MessageType>))]
[JsonSourceGenerationOptions(WriteIndented = false, IncludeFields = true, UseStringEnumConverter = true, IgnoreReadOnlyFields = true)]
public partial class Collection_SrcGenCtx : JsonSerializerContext
{ }

[JsonSerializable(typeof(ChallengeVO))]
[JsonSerializable(typeof(MapVO))]
[JsonSerializable(typeof(NetworkNodeVO))]
[JsonSerializable(typeof(MessageType))]
[JsonSerializable(typeof(ProtocolType))]
[JsonSourceGenerationOptions(WriteIndented = false, IncludeFields = true, UseStringEnumConverter = true)]
public partial class VO_SrcGenCtx : JsonSerializerContext
{ }
