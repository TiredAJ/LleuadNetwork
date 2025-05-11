using System;

namespace LleuadNetworkSim.Models.Lua;

public struct LuaScript
{
    public LuaScript() { }

    public string FileLoc { get; init; } = string.Empty;

    public string FileData { get; set; } = string.Empty;

    public bool PreLoaded { get; set; } = false;
}
