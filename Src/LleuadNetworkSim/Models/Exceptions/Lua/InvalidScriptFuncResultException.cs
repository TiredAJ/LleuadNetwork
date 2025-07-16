using System;

using MoonSharp.Interpreter;

namespace LleuadNetworkSim.Models.Exceptions.Lua;

public class InvalidScriptFuncResultException(DataType _FuncType, DynValue _FuncVal)
    : Exception(DebugMessage(_FuncType, _FuncVal))
{
    static private string DebugMessage(DataType _FuncType, DynValue _FuncVal)
        => $"Func returned invalid type, returned type was [{_FuncType}] with value [{_FuncVal}]." +
           $" Expected [{DataType.Number}]";
}
