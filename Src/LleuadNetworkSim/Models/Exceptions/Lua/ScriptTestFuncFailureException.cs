using System;

namespace LleuadNetworkSim.Models.Exceptions.Lua;

public class ScriptTestFuncFailureException(Exception _Exc) : Exception(DebugMessage(_Exc), _Exc)
{
    static private string DebugMessage(Exception _Exc)
        => $"Script failed test message with [{_Exc.Message}].";
}
