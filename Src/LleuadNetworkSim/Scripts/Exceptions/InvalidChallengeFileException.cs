using System;

namespace LleuadNetworkSim.Scripts.Exceptions;

public class InvalidChallengeFileException(string _FilePath) : Exception(DebugMessage(_FilePath))
{
    static private string DebugMessage(string _FilePath)
        => $"The selected file [{_FilePath}] must be a .lnchallenge file!";
}
