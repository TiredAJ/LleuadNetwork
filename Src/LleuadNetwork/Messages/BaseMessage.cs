using CSharpFunctionalExtensions;

using SharpGraph;

namespace LleuadNetwork.Messages;

public abstract class BaseMessage<T> : IMessage where T : notnull
{
    /// <summary>
    /// Metadata of the message
    /// </summary>
    protected Dictionary<string, string> Headers { get; private set; }
    
    /// <summary>
    /// The payload of the message
    /// </summary>
    protected Maybe<T> Payload;

    protected BaseMessage(T _Payload) {
        Headers = MessageUtils.GetDefaultHeaders();

        Payload = _Payload;

        Headers[MessageUtils.DefaultHeaders.PAYLOADTYPE] = typeof(T).FullName ?? "NoType";
        Headers[MessageUtils.DefaultHeaders.MESSAGETYPE] = "BasicMessage";
    }

    /// <summary>
    /// Attempts to retrieve the value of a specific header 
    /// </summary>
    /// <param name="_Key">The header to get the value for.</param>
    /// <returns>The value of the header or an empty string if it doesn't exist.</returns>
    public string GetHeader(string _Key)
        => Headers.GetValueOrDefault(_Key, string.Empty);

    /// <summary>
    /// Attempts to replace the value of header.
    /// </summary>
    /// <param name="_Key">The header to replace the value of.</param>
    /// <param name="_NewVal">The new value of the header.</param>
    /// <returns>The given value if the header exists, empty string if not.</returns>
    public string EditHeader(string _Key, string _NewVal) {
        if (!Headers.ContainsKey(_Key))
        { return string.Empty; }

        Headers[_Key] = _NewVal;
        return _NewVal;

    }

    /// <summary>
    /// Attempts to add a new header with a specific value.
    /// </summary>
    /// <param name="_Key">The new header.</param>
    /// <param name="_NewVal">The value of the new header.</param>
    /// <returns></returns>
    public string AddHeader(string _Key, string _NewVal) 
        => Headers.TryAdd(_Key, _NewVal) ? _NewVal : string.Empty;
    
    public Maybe<T> GetPayload()
        => Payload;

    public void SetPayload(T _Payload)
        => Payload = _Payload;

    public bool HasPayload()
        => Payload.HasValue;

    public virtual string MessageType()
        => Headers[MessageUtils.DefaultHeaders.MESSAGETYPE];
}

/// <summary>
/// An effective void type for empty messages
/// </summary>
public sealed class None
{
    private None() { }

    static public None GetInstance()
        => new None();
}