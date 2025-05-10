using System;

namespace LleuadNetworkSim.Models.Exceptions.Validation.File;

public class InvalidFileTypeException(string _FilePath, string _ExpectedFileType) : Exception(DebugMessage(_FilePath, _ExpectedFileType))
{
    static private string DebugMessage(string _FilePath, string _ExpectedFileType)
        => $"The selected file [{_FilePath}] must be a {_ExpectedFileType} file!";
}
