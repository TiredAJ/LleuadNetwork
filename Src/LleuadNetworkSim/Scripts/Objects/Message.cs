using System;

namespace LleuadNetworkSim.Scripts.Objects;

public class Message
{
    private Headers IntHeaders = new Headers();

    #region Headers

    #region DefaultHeaders
    public string GetSenderAddress() => IntHeaders.GetHeader(Header.SENDER_ADDRESS);
    public string GetDestinationAddress() => IntHeaders.GetHeader(Header.RECEIVE_ADDRESS);
    public string GetType() => IntHeaders.GetHeader(Header.TYPE);
    public int GetIndex() => Convert.ToInt32(IntHeaders.GetHeader(Header.INDEX));
    public int GetPriority() => Convert.ToInt32(IntHeaders.GetHeader(Header.PRIORITY));
    #endregion

    #endregion
}
