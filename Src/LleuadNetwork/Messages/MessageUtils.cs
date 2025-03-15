namespace LleuadNetwork.Messages;

static public class MessageUtils
{
    static private Dictionary<string, string> DefaultHeaderSet = new() {
        { "PayloadType", "DEFAULT" },
        { "PayloadSize", "0" },
        { "Protocol", "DEFAULT" },
        { "MessageType", "DEFAULT" },
    };
    
    
    static public Dictionary<string, string> GetDefaultHeaders() {
        return new Dictionary<string, string>(DefaultHeaderSet);
    }

    static public class DefaultHeaders
    {
        static public string PAYLOADTYPE => "PayloadType";
        static public string PAYLOADSIZE => "PayloadSize";
        static public string PROTOCOL => "Protocol";
        static public string MESSAGETYPE => "MessageType";
    }

    
}
