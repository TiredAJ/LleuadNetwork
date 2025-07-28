using BuilderGenerator;

namespace Common.Messaging;

[BuilderFor(typeof(Message))]
public partial class MessageBuilder
{
    static public MessageBuilder DebugMessage()
        => new MessageBuilder()
            .WithMessageType("DEBUG")
            .WithSenderAddress("N/A")
            .WithDestinationAddress("N/A");

    static public MessageBuilder Default()
        => new MessageBuilder()
            .WithSenderAddress("")
            .WithDestinationAddress("");
}
