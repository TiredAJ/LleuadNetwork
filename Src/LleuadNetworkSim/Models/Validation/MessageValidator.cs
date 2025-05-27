using Godot.Logging;

using LleuadNetworkSim.Models.Messaging;

namespace LleuadNetworkSim.Models.Validation;

static public class MessageValidator
{
    static public bool MessageValid(Message _Msg) {
        _Msg.Hops++;
        
        if (_Msg.GetAliveTime() >= _Msg.Lifespan)
        { return false; }

        if (_Msg.Hops >= _Msg.MaxHops)
        { return false; }
        
        return true;
    }
    
    static public bool MessageValid(Message _Original, Message _New) {
        _Original.Hops++;
        _New.Hops++;
        
        if (_Original.GetAliveTime() >= _Original.Lifespan)
        { return false; }

        if (_Original.Hops >= _Original.MaxHops)
        { return false; }
        
        return HasBeenTampered(_Original, _New);
    }

    static public bool HasBeenTampered(Message _Original, Message _New) {
        GodotLogger.LogWarning("HasBeenTampered has not been implemented yet.");
        
        return false;
    }
}
