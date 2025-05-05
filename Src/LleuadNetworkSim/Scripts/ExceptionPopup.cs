using System;

using Godot;

namespace LleuadNetworkSim.Scripts;

public partial class ExceptionPopup : PopupPanel
{
    [Export]
    private Label ExceptionName = null!;

    [Export]
    private TextEdit ExceptionDetails = null!;

    public Exception Exc { get; set; } = null!;

    public override void _EnterTree() {

        ExceptionName.Text = Exc.GetType().Name;
        ExceptionDetails.Text = Exc.Message;
        
        base._EnterTree();
    }
}