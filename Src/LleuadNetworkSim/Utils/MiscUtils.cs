using System;

namespace LleuadNetworkSim.Utils;

static public class MiscUtils
{
    static public string OrderedNames(string _A, string _B) {
        return string.Compare(_A, _B, StringComparison.Ordinal) < 0 ?
                   $"{_A}--{_B}" :
                   $"{_B}--{_A}";
    }
}
