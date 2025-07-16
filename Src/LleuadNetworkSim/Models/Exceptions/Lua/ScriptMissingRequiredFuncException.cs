using System;

namespace LleuadNetworkSim.Models.Exceptions.Lua;

public class ScriptMissingRequiredFuncException(string _ScriptName, string _ExpectedFuncName)
    : Exception(DebugMessage(_ScriptName, _ExpectedFuncName))
{
    static private string DebugMessage(string _ScriptName, string _ExpectedFuncName)
        => $"Script [{_ScriptName}] does not contain required function [{_ExpectedFuncName}].";
}
