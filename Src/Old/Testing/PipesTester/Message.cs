namespace PipesTester;

using CSharpFunctionalExtensions;

public record Message<T>(string _Address, T? _Payload, int _MessageID = 0)
{
    public string Address { get; set; } = _Address;
    public int MessageID { get; set; } = _MessageID;
    public Maybe<T> Payload { get; set; } = Maybe.From(_Payload);

    static public Message<T> Blank() => new ("DEFAULT", default, 0);
}
