using System.Threading.Tasks;

using Common.Json;

using Godot;

using LleuadNetworkSim.Scripts.Nodes;

namespace LleuadNetworkSim.Utils;

static public class Extensions
{
    static public Vector2 ToVec2(this PositionVectorVO _PVO)
        => new(_PVO.X, _PVO.Y);

    static public bool HasFlagFast(this UIMode _Value, UIMode _Flag)
        => (_Value & _Flag) != 0;
}

static public class TaskExtensions
{
    /// <summary>
    /// Fires and forgets a task. Thanks to Greg Gum https://stackoverflow.com/a/79593298/19306828; 
    /// </summary>
    static public void Fire(this Task _Task)
    {
        if (!_Task.IsCompleted || _Task.IsFaulted)
        { _ = ForgetAwaited(_Task); }

        return;

        static async Task ForgetAwaited(Task _Task)
        { await _Task.ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing); }
    }

    /// <summary>
    /// Runs and forgets a task in the background. Thanks to Mark https://stackoverflow.com/a/77652530/19306828  
    /// </summary>
    /// <param name="_Task"></param>
    static public void Background(this Task _Task) {
        _ = Task.Run(async () => await _Task);
    }
}
