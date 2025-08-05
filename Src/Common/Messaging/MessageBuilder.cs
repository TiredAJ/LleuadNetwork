using System.Text;

using BuilderGenerator;

using static Common.Conf.Conf;

namespace Common.Messaging;

[BuilderFor(typeof(Message))]
public partial class MessageBuilder
{
    public MessageBuilder() {
        PostBuildAction = X => X.GenerateID();
    }
    
    static public MessageBuilder DebugMessage()
        => Default()
            .WithMessageType("DEBUG")
            .WithSenderAddress("N/A")
            .WithDestinationAddress("N/A");

    static public MessageBuilder Default()
        => new MessageBuilder()
            .WithSenderAddress("")
            .WithDestinationAddress("")
            .WithMessageEncoding(Encoding.UTF8)
            .WithMessageEncodingStr("UTF-8")
            .WithMaxMessageSize(MAX_MESSAGE_SIZE)
            .WithResponseRequired(false);
}
