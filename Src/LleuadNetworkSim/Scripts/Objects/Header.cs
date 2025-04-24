using System;
using System.Diagnostics;

namespace LleuadNetworkSim.Scripts.Objects;

public enum Header
{
    /// <summary>
    /// Address of the sender node.
    /// </summary>
    SENDER_ADDRESS,
    
    /// <summary>
    /// Address of the recipient node.
    /// </summary>
    RECEIVE_ADDRESS,
    
    /// <summary>
    /// The type of message.
    /// </summary>
    TYPE,
    
    /// <summary>
    /// The chunk index this message makes up of a whole piece of data. -1 if it's not a chunk.
    /// </summary>
    INDEX,
    
    /// <summary>
    /// The priority of the message. Lower value = higher priority. So 0 is a higher priority than 5.
    /// </summary>
    PRIORITY,
    
    /// <summary>
    /// The date & time this message was made (and sent).
    /// </summary>
    CREATION_TIME,
    
    /// <summary>
    /// How long the message should stay alive for.
    /// </summary>
    LIFESPAN,
    
    /// <summary>
    /// How many total nodes the message has passed through.
    /// </summary>
    HOPS,
    
    /// <summary>
    /// If a message needs to be sent back to the sender to prove
    ///  the message was received.
    /// </summary>
    RECEIVE_RESPONSE_REQUIRED,
    
    /// <summary>
    /// The size of the payload of the message (in bytes).
    /// </summary>
    MESSAGE_SIZE,
    
    /// <summary>
    /// The maximum size of the payload for this type of message (in bytes).
    /// </summary>
    MAX_SIZE,
    
    /// <summary>
    /// If this message is a chunk, what the total size (in bytes) is
    ///  of all chunks together. 
    /// </summary>
    TOTAL_SIZE
}

static public class Extensions
{
    static public string ToStr(this Header _H) => _H switch {
        Header.SENDER_ADDRESS => "SENDER_ADDRESS",
        Header.RECEIVE_ADDRESS => "RECEIVE_ADDRESS",
        Header.TYPE => "TYPE",
        Header.INDEX => "INDEX",
        Header.PRIORITY => "PRIORITY",
        Header.CREATION_TIME => "CREATION_TIME",
        Header.LIFESPAN => "LIFESPAN",
        Header.HOPS => "HOPS",
        Header.RECEIVE_RESPONSE_REQUIRED => "RECEIVE_RESPONSE_REQUIRED",
        Header.MESSAGE_SIZE => "MESSAGE_SIZE",
        Header.MAX_SIZE => "MAX_SIZE",
        Header.TOTAL_SIZE => "TOTAL_SIZE",
        _ => throw new ArgumentOutOfRangeException(nameof(_H), _H, null)
    };
}
