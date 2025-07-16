namespace LleuadNetworkSim.Models.Messaging;

public class MessageGenerator
{
    static public Message DebugMessage
        => new Message("N/A", "N/A") {
            MessageType = "DEBUG"
        };

    static public ReadonlyMessage ToReadonly(Message _Msg)
        => new ReadonlyMessage(_Msg);

    static public Message DefaultMessage() {
        return new Message("", "");
    }
}
