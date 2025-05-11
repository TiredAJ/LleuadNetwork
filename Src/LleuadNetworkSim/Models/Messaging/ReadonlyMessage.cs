using System;
using System.Text;

using CSharpFunctionalExtensions;

using LleuadNetworkSim.Models.Exceptions;
using LleuadNetworkSim.Models.Exceptions.Messages;
using LleuadNetworkSim.Utils;

namespace LleuadNetworkSim.Models.Messaging;

public class ReadonlyMessage(string _SenderAddress, string _DestinationAddress, string? _Payload = null, int _MaxHops = 50)
    : Message(_SenderAddress, _DestinationAddress, _Payload, _MaxHops)
{
    #region Headers

    #region DefaultHeaders

    /// <summary>
    /// ID of message
    /// </summary>
    public new string ID {
        get => IntHeaders.GetHeader(Header.ID);
        init => IntHeaders.SetHeaderValue(Header.ID, value);
    }
    
    /// <summary>
    /// Address of the sender node.
    /// </summary>
    public new string SenderAddress {
        get => IntHeaders.GetHeader(Header.SENDER_ADDRESS);
        init => IntHeaders.SetHeaderValue(Header.SENDER_ADDRESS, value);
    }

    /// <summary>
    /// Address of the recipient node.
    /// </summary>
    public new string DestinationAddress {
        get => IntHeaders.GetHeader(Header.DESTINATION_ADDRESS);
        init => IntHeaders.SetHeaderValue(Header.DESTINATION_ADDRESS, value);
    }

    /// <summary>
    /// The type of message.
    /// </summary>
    public new string MessageType {
        get => IntHeaders.GetHeader(Header.TYPE);
        init => IntHeaders.SetHeaderValue(Header.TYPE, value);
    }

    /// <summary>
    /// The encoding used for the payload.
    /// </summary>
    public new Encoding MessageEncoding {
        get => Encoding.GetEncoding(IntHeaders.GetHeader(Header.ENCODING));
        init => IntHeaders.SetHeaderValue(Header.ENCODING, value.WebName);
    }

    /// <summary>
    /// The chunk index this message makes up of a blob. -1 if it's not a chunk.
    /// </summary>
    public new int Index {
        get => Convert.ToInt32(IntHeaders.GetHeader(Header.INDEX));
        init => IntHeaders.SetHeaderValue(Header.INDEX, value.ToString());
    }

    /// <summary>
    /// The priority of the message. Lower value = higher priority. So 0 is a higher priority than 5.
    /// </summary>
    public new int Priority {
        get => Convert.ToInt32(IntHeaders.GetHeader(Header.PRIORITY));
        init => IntHeaders.SetHeaderValue(Header.PRIORITY, value.ToString());
    }

    /// <summary>
    /// The date & time this message was made (and sent).
    /// </summary>
    public new DateTime CreationTime {
        get => DateTime.FromBinary(Convert.ToInt64(IntHeaders.GetHeader(Header.CREATION_TIME)));
        init => IntHeaders.SetHeaderValue(Header.CREATION_TIME, value.ToBinary().ToString());
    }

    /// <summary>
    /// How long the message should stay alive for.
    /// </summary>
    public new TimeSpan Lifespan {
        get => TimeSpan.FromMilliseconds(Convert.ToInt64(IntHeaders.GetHeader(Header.LIFESPAN)));
        init => IntHeaders.SetHeaderValue(Header.LIFESPAN, value.TotalMilliseconds.ToString());
    }

    /// <summary>
    /// How many total nodes the message has passed through.
    /// </summary>
    public new int Hops {
        get => Convert.ToInt32(IntHeaders.GetHeader(Header.HOPS));
        init => IntHeaders.SetHeaderValue(Header.HOPS, value.ToString());
    }

    /// <summary>
    /// If a message needs to be sent back to the sender to prove
    ///  the message was received.
    /// </summary>
    public new bool ResponseRequired {
        get => Convert.ToBoolean(IntHeaders.GetHeader(Header.RECEIVE_RESPONSE_REQUIRED));
        init => IntHeaders.SetHeaderValue(Header.RECEIVE_RESPONSE_REQUIRED, value.ToString());
    }

    /// <summary>
    /// The size of the payload of the message (in bytes).
    /// </summary>
    public new int MessageSize {
        get => Convert.ToInt32(IntHeaders.GetHeader(Header.MESSAGE_SIZE));
        init => IntHeaders.SetHeaderValue(Header.MESSAGE_SIZE, value.ToString());
    }

    /// <summary>
    /// The maximum size of the payload for this type of message (in bytes).
    /// </summary>
    public new int MaxMessageSize {
        get => Convert.ToInt32(IntHeaders.GetHeader(Header.MAX_MESSAGE_SIZE));
        init => IntHeaders.SetHeaderValue(Header.MAX_MESSAGE_SIZE, value.ToString());
    }

    /// <summary>
    /// If this message is a chunk, what the total size is (in bytes)
    ///  of it's blob (all chunks together). 
    /// </summary>
    public new long TotalSize {
        get => Convert.ToInt64(IntHeaders.GetHeader(Header.TOTAL_SIZE));
        init => IntHeaders.SetHeaderValue(Header.TOTAL_SIZE, value.ToString());
    }

    public new string LastNodeID {
        get => IntHeaders.GetHeader(Header.LAST_NODE_ID);
        init => IntHeaders.SetHeaderValue(Header.LAST_NODE_ID, value);
    }
    #endregion

    #endregion
    
    public ReadonlyMessage(Message _Msg) 
        : this(_Msg.SenderAddress, _Msg.SenderAddress, _Msg.Payload.GetValueOrDefault(), _Msg.MaxHops) {

        IntHeaders = _Msg.CloneHeaders();
    }
}
