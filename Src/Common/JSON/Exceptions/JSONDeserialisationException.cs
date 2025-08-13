namespace Common.Json.Exceptions;

public class JSONDeserialisationException(string _VO) : Exception(DebugMessage(_VO))
{
    static private string DebugMessage(string _VO)
        => $"Null was returned when attempting to deserialise JSON for type {_VO}";
}
