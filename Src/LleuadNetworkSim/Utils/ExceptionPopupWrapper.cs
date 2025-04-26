using System;

using Godot;

namespace PipesTester;

public class ExceptionPopupWrapper
{
    static public void Throw(Node _Parent, Exception _Exc) {
        ExceptionPopup ExcPopup = ResourceLoader.Load<PackedScene>("res://Scenes/exception_popup.tscn")
                                                .Instantiate<ExceptionPopup>();

        ExcPopup.Exc = _Exc;
        
        _Parent.AddChild(ExcPopup);

        throw _Exc;
    }
}
