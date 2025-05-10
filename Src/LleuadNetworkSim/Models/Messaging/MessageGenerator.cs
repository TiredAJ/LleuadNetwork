namespace LleuadNetworkSim.Models.Messaging;

public class MessageGenerator
{
    static public Message DebugMessage 
        => new Message("N/A", "N/A") {
            MessageType = "DEBUG"
        };
}
