using Godot;
using System;

public partial class ExceptionPopup : PopupPanel
{
    [Export]
    private Label ExceptionName;

    [Export]
    private TextEdit ExceptionDetails;

    public Exception Exc;

    public override void _EnterTree() {

        ExceptionName.Text = Exc.GetType().Name;
        ExceptionDetails.Text = Exc.Message;
        
        base._EnterTree();
    }
}
