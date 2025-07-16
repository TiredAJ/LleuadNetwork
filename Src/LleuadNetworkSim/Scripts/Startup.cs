using Godot;
using Godot.Logging;
using Godot.Logging.Targets;

namespace LleuadNetworkSim.Scripts;

public partial class Startup : Node2D
{
    public override void _Ready() {

        LogConfiguration Conf = new();
        Conf.RegisterTarget(new GDPrintTarget("GodotConsole"));

        FormatRule Formatting = new FormatRule()
        {
            FormatText = "[${level}][${classname}.${methodname}] ${message}",
            FormatLogLevel = LogLevel.Info
        };

        Conf.ApplyFormattingRule(Formatting);

        // Set the configuration
        GodotLogger.SetConfiguration(Conf);

        base._Ready();
    }
}