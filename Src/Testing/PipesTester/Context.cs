namespace PipesTester;

public struct Context(int _MessageID, DateTime _MsgDateTime)
{
    public int MessageID { get; set; } = _MessageID;
    public DateTime MsgDateTime { get; set; } = _MsgDateTime;

    public override string ToString() { return $"MessageID: {MessageID}, MsgDateTime: {MsgDateTime}"; }
}
