using System;

using LleuadNetworkSim.Scripts.Models.Message;
using LleuadNetworkSim.Scripts.Objects;

namespace LleuadNetworkSim.Utils.Validators;

static public class NodeValidator
{
    static public bool MessageValid(Message _Msg) {
        _Msg.Hops++;
        
        if (_Msg.GetAliveTime() >= _Msg.Lifespan)
        { return false; }

        if (_Msg.Hops >= _Msg.MaxHops)
        { return false; }
        
        return true;
    }
}
