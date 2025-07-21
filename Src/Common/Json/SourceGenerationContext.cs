using System.Text.Json.Serialization;

namespace Common.Challenge.JSON;

[JsonSourceGenerationOptions(WriteIndented = true, IncludeFields = true, UseStringEnumConverter = true)]
[JsonSerializable(typeof(MessageType))]
[JsonSerializable(typeof(ProtocolType))]
[JsonSerializable(typeof(List<MessageType>))]
public partial class SourceGenerationContext : JsonSerializerContext
{ }
