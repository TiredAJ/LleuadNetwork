using System;
using System.Linq;
using System.Text;
using System.Buffers.Text;
using System.Text.Json.Nodes;

using Common.Entities;

// ReSharper disable InvalidXmlDocComment

namespace Common.Utils;

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

    static public int ToInt32(this JsonNode _JNode)
        => Convert.ToInt32(_JNode);

    static public int ToInt(this double _D)
        => Convert.ToInt32(_D);

    static public int ToInt(this double? _D)
        => Convert.ToInt32(_D);

    /// <summary>
    /// Returns an alternate string if this string is empty.
    /// </summary>
    /// <param name="_Alternate">Alternative to return if the provided string is empty.</param>
    static public string Or(this string _Str, string _Alternate)
        => _Str is "" or " " ? _Alternate : _Str;

    /// <summary>
    /// Returns an alternate value if the given int is less than 0.
    /// </summary>
    /// <param name="_Alternate">Alternative to return if the provided int is &lt; 0.</param>
    static public int Or(this int _Val, int _Alternate)
        => _Val < 0 ? _Alternate : _Val;

    /// <summary>
    /// Returns an alternate value if the given int is &lt; _LessThan.
    /// </summary>
    /// <param name="_LessThan">A value to compare the provided int against.</param>
    /// <param name="_Alternate">Alternative to return if the provided int is &lt; 0.</param>
    static public int Or(this int _Val, int _LessThan, int _Alternate)
        => _Val < _LessThan ? _Alternate : _Val;

    static public string ToStr(this RecordAction _RA) {
        return _RA switch {
            RecordAction.RECEIVED => "RECEIVED",
            RecordAction.CONSUMED => "CONSUMED",
            RecordAction.DROPPED => "DROPPED",
            RecordAction.SENT => "SENT",
            _ => "N/A"
        };
    }
}
