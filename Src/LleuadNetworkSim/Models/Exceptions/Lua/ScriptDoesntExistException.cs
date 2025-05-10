using System;
using System.IO;

namespace LleuadNetworkSim.Models.Exceptions.Lua;

public class ScriptDoesntExistException(string _FilePath) : Exception(DebugMessage(_FilePath))
{
    static private string DebugMessage(string _FilePath)
        => $"The selected file [{Path.GetFileName(_FilePath)}] could not be found at [{_FilePath}].";
}
