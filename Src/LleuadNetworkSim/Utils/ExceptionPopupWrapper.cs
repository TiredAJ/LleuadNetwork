using System;

using Godot;

namespace LleuadNetworkSim.Utils;

public class ExceptionPopupWrapper
{
    static public void Throw(Node _Parent, Exception _Exc) {
        Scripts.ExceptionPopup ExcPopup = ResourceLoader.Load<PackedScene>("res://Scenes/exception_popup.tscn")
                                                        .Instantiate<Scripts.ExceptionPopup>();

        ExcPopup.Exc = _Exc;
        
        _Parent.AddChild(ExcPopup);

        throw _Exc;
    }
}
