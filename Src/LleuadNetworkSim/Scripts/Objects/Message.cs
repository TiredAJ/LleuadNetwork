using System;
using System.Text;

using CSharpFunctionalExtensions;

using LleuadNetworkSim.Scripts.Objects.Exceptions;

namespace LleuadNetworkSim.Scripts.Objects;

public class Message
{
    private Headers IntHeaders;

    #region Headers

    #region DefaultHeaders

    public string ID {
        get => IntHeaders.GetHeader(Header.ID);
        set => IntHeaders.SetHeaderValue(Header.ID, value);
    }
    
    /// <summary>
    /// Address of the sender node.
    /// </summary>
    public string SenderAddress {
        get => IntHeaders.GetHeader(Header.SENDER_ADDRESS);
        set => IntHeaders.SetHeaderValue(Header.SENDER_ADDRESS, value);
    }

    /// <summary>
    /// Address of the recipient node.
    /// </summary>
    public string DestinationAddress {
        get => IntHeaders.GetHeader(Header.DESTINATION_ADDRESS);
        set => IntHeaders.SetHeaderValue(Header.DESTINATION_ADDRESS, value);
    }

    /// <summary>
    /// The type of message.
    /// </summary>
    public string MessageType {
        get => IntHeaders.GetHeader(Header.TYPE);
        set => IntHeaders.SetHeaderValue(Header.TYPE, value);
    }

    /// <summary>
    /// The encoding used for the payload.
    /// </summary>
    public Encoding MessageEncoding {
        get => Encoding.GetEncoding(IntHeaders.GetHeader(Header.ENCODING));
        set => IntHeaders.SetHeaderValue(Header.ENCODING, value.WebName);
    }

    /// <summary>
    /// The chunk index this message makes up of a blob. -1 if it's not a chunk.
    /// </summary>
    public int Index {
        get => Convert.ToInt32(IntHeaders.GetHeader(Header.INDEX));
        set => IntHeaders.SetHeaderValue(Header.INDEX, value.ToString());
    }

    /// <summary>
    /// The priority of the message. Lower value = higher priority. So 0 is a higher priority than 5.
    /// </summary>
    public int Priority {
        get => Convert.ToInt32(IntHeaders.GetHeader(Header.PRIORITY));
        set => IntHeaders.SetHeaderValue(Header.PRIORITY, value.ToString());
    }

    /// <summary>
    /// The date & time this message was made (and sent).
    /// </summary>
    public DateTime CreationTime {
        get => DateTime.FromBinary(Convert.ToInt64(IntHeaders.GetHeader(Header.CREATION_TIME)));
        set => IntHeaders.SetHeaderValue(Header.CREATION_TIME, value.ToBinary().ToString());
    }

    /// <summary>
    /// How long the message should stay alive for.
    /// </summary>
    public TimeSpan Lifespan {
        get => TimeSpan.FromMilliseconds(Convert.ToInt64(IntHeaders.GetHeader(Header.LIFESPAN)));
        set => IntHeaders.SetHeaderValue(Header.LIFESPAN, value.TotalMilliseconds.ToString());
    }

    /// <summary>
    /// How many total nodes the message has passed through.
    /// </summary>
    public int Hops {
        get => Convert.ToInt32(IntHeaders.GetHeader(Header.HOPS));
        set => IntHeaders.SetHeaderValue(Header.HOPS, value.ToString());
    }

    /// <summary>
    /// If a message needs to be sent back to the sender to prove
    ///  the message was received.
    /// </summary>
    public bool ResponseRequired {
        get => Convert.ToBoolean(IntHeaders.GetHeader(Header.RECEIVE_RESPONSE_REQUIRED));
        set => IntHeaders.SetHeaderValue(Header.RECEIVE_RESPONSE_REQUIRED, value.ToString());
    }

    /// <summary>
    /// The size of the payload of the message (in bytes).
    /// </summary>
    public int MessageSize {
        get => Convert.ToInt32(IntHeaders.GetHeader(Header.MESSAGE_SIZE));
        set => IntHeaders.SetHeaderValue(Header.MESSAGE_SIZE, value.ToString());
    }

    /// <summary>
    /// The maximum size of the payload for this type of message (in bytes).
    /// </summary>
    public int MaxMessageSize {
        get => Convert.ToInt32(IntHeaders.GetHeader(Header.MAX_MESSAGE_SIZE));
        set => IntHeaders.SetHeaderValue(Header.MAX_MESSAGE_SIZE, value.ToString());
    }

    /// <summary>
    /// If this message is a chunk, what the total size is (in bytes)
    ///  of it's blob (all chunks together). 
    /// </summary>
    public long TotalSize {
        get => Convert.ToInt64(IntHeaders.GetHeader(Header.TOTAL_SIZE));
        set => IntHeaders.SetHeaderValue(Header.TOTAL_SIZE, value.ToString());
    }

    #endregion

    #endregion

    public Message(string _SenderAddress, string _DestinationAddress, string? _Payload = null) {

        IntHeaders = new Headers();
        
        SenderAddress = _SenderAddress;
        DestinationAddress = _DestinationAddress;
        CreationTime = DateTime.Now;

        if (_Payload is not null)
        { Payload = _Payload; }
    }
    
    public Maybe<string> Payload {
        get;
        set {
            if (value.HasValue)
            {
                int Size = SizeInBytes(value.Value);

                if (Size > MaxMessageSize)
                { throw new PayloadTooLargeException(Size, MaxMessageSize); }

                MessageSize = Size;
            }
            
            field = value;
        }
    } = string.Empty;

    public bool IsValid() {
        if (SenderAddress == Headers.DEFAULT_VAL
            || DestinationAddress == Headers.DEFAULT_VAL
            || CreationTime == DateTime.MinValue)
        { return false; }

        if (MessageSize != SizeInBytes(Payload.Value))
        { return false; }

        return true;
    }

    private int SizeInBytes(string _Value) => MessageEncoding.GetByteCount(_Value);

    public Message Clone() {
        return new Message(SenderAddress, DestinationAddress, Payload.GetValueOrDefault())
        {
            CreationTime = CreationTime,
            Hops = Hops,
            Index = Index,
            Lifespan = Lifespan,
            MessageEncoding = MessageEncoding,
            MessageSize = MessageSize,
            MessageType = MessageType,
            ResponseRequired = ResponseRequired,
            MaxMessageSize = MaxMessageSize,
            Priority = Priority,
            TotalSize = TotalSize
        };
    }

    public override string ToString() {
        return $"[ID: {ID}],[Sender Addr: {SenderAddress}],[Destination Addr: {DestinationAddress}]," +
                $"[Type: {MessageType}],[Encoding: {MessageEncoding}],[Index: {Index}]," +
                $"[Priority: {Priority}],[Creation Time: {CreationTime}],[Lifespan: {Lifespan}]," +
                $"[Hops: {Hops}],[Response Req: {ResponseRequired}],[Size: {MessageSize}]," +
                $"[Max Size: {MaxMessageSize}],[Total Size: {TotalSize}]\n[Payload: {Payload}]";
    }
}
