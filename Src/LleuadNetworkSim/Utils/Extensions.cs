using System;
using System.Linq;
using System.Text;
using System.Buffers.Text;
using System.Text.Json.Nodes;

using Godot;

using LleuadNetworkSim.Models.Validation.Json;
using LleuadNetworkSim.Scripts.Nodes;

namespace LleuadNetworkSim.Utils;

static public class Extensions
{
    static public byte[] AddValue(this string _A, string _B) {

        byte[] ListA = Encoding.ASCII.GetBytes(_A);
        byte[] ListB = Encoding.ASCII.GetBytes(_B);

        return ListA.Zip(ListB)
                    .Select(X => Convert.ToByte(X.First + X.Second))
                    .ToArray();
    }

    static public string ToBase64(this Guid _UUID)
        => Base64Url.EncodeToString(_UUID.ToByteArray());

    static public string ToBase64Name(this Guid _UUID)
        => _UUID.ToBase64().TrimEnd('=')[10..];

    //thanks to fubo https://stackoverflow.com/a/15340481/19306828
    static public string ToFileSize(this long _Size) {
        if (_Size < 1024)
        { return $"{(_Size):F0} B"; }

        if ((_Size >> 10) < 1024)
        { return $"{(_Size / (float)1024):F1} KB"; }

        if ((_Size >> 20) < 1024)
        { return $"{((_Size >> 10) / (float)1024):F1} MB"; }

        if ((_Size >> 30) < 1024)
        { return $"{((_Size >> 20) / (float)1024):F1} GB"; }

        if ((_Size >> 40) < 1024)
        { return $"{((_Size >> 30) / (float)1024):F1} TB"; }

        if ((_Size >> 50) < 1024)
        { return $"{((_Size >> 40) / (float)1024):F1} PB"; }

        return $"{((_Size >> 50) / (float)1024):F0} EB";
    }

    static public Vector2 ToVec2(this PositionVectorVO _PVO)
        => new(_PVO.X, _PVO.Y);
    
    static public bool HasFlagFast(this UIMode _Value, UIMode _Flag)
        => (_Value & _Flag) != 0;

    static public int ToInt32(this JsonNode _JNode)
        => Convert.ToInt32(_JNode);
}
