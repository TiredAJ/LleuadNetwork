using Godot;

using LleuadNetworkSim.Models.Validation.Json;
using LleuadNetworkSim.Scripts.Nodes;

namespace LleuadNetworkSim.Utils;

static public class Extensions
{
    static public Vector2 ToVec2(this PositionVectorVO _PVO)
        => new(_PVO.X, _PVO.Y);

    static public bool HasFlagFast(this UIMode _Value, UIMode _Flag)
        => (_Value & _Flag) != 0;
}
