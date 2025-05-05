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
