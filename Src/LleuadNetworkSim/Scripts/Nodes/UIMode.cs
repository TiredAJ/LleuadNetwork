using System;

namespace LleuadNetworkSim.Scripts.Nodes;

[Flags]
public enum UIMode
{
    NONE        = 0,
    SPAWNING    = 1 << 1,
    CONNECTING  = 1 << 2,
    MESSAGING   = 1 << 3,
}

static public class UIModeExtensions
{
    static public bool HasFlag(this UIMode value, UIMode flag)
    { return (value & flag) != 0; }
}
