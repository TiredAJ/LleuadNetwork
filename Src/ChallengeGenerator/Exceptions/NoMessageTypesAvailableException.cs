namespace ChallengeGenerator.Exceptions;

public class NoMessageTypesAvailableException(string _MessageTypesFilePath) : Exception(DebugMessage(_MessageTypesFilePath))
{
    static private string DebugMessage(string _MessageTypesFilePath)
        => $"No message types could be loaded! Please check {_MessageTypesFilePath}";
}
