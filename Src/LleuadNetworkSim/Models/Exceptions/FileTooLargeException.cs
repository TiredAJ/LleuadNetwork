using System;

using LleuadNetworkSim.Utils;

namespace LleuadNetworkSim.Models.Exceptions;

public class FileTooLargeException(long _FileSize) : Exception(DebugMessage(_FileSize))
{
    static private string DebugMessage(long _FileSize)
        => $"The selected file is too large. File was [{_FileSize.ToFileSize()}], max size is 25MB.";
}
