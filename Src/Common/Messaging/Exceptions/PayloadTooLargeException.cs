namespace Common.Messaging.Exceptions;

public class PayloadTooLargeException(int _Length, int _MaxLength) : Exception(DebugMessage(_Length, _MaxLength))
{
    static private string DebugMessage(int _Length, int _MaxLength) {
        return $"The given payload of length [{_Length}]bytes exceeds the max payload size of [{_MaxLength}]bytes.";
    }
}
