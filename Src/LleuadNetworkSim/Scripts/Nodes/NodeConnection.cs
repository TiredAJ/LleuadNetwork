using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;

using Common.Messaging;

using Godot;

namespace LleuadNetworkSim.Scripts.Nodes;

public partial class NodeConnection : Path2D
{
    [Export]
    private Liner Liner = null!;

    private NetworkNode NodeA = null!;
    private NetworkNode NodeB = null!;
    public int FollowerCount { get; set; } = 0;
    public float Length { get; private set; } = 0;

    private ChannelWriter<Message> CommsOutput = null!;
    private ChannelReader<Message> CommsInput = null!;

    private Vector2 PointAPrev;
    private Vector2 PointBPrev;

    public void Init(NetworkNode _A, NetworkNode _B, ChannelWriter<Message> _Output, ChannelReader<Message> _Input) {
        NodeA = _A;
        NodeB = _B;

        CommsOutput = _Output;
        CommsInput = _Input;

        if (!IsNodeReady())
        { return; }

        Debug.WriteLine("resetting curve!");

        ResetCurve();

        Length = ToLocal(NodeA.GlobalPosition).DistanceTo(ToLocal(NodeB.GlobalPosition));
    }

    public override void _Ready() {

        this.Curve = new Curve2D();

        base._Ready();
    }

    public override void _Process(double _Delta) {

        ResetCurve();

        base._Process(_Delta);
    }

    public override void _ExitTree() {

        foreach (Node? Child in GetChildren())
        { Child.QueueFree(); }

        NodeA.RemoveConnection(NodeB.Name);
        NodeB.RemoveConnection(NodeA.Name);

        base._ExitTree();
    }

    private void ResetCurve() {
        Curve.ClearPoints();

        Vector2 PointA = ToLocal(NodeA.GlobalPosition);
        Vector2 PointB = ToLocal(NodeB.GlobalPosition);

        if (PointA != PointAPrev || PointB != PointBPrev)
        {
            Length = PointA.DistanceTo(PointB);
            UpdateFollowers();
        }

        Curve.AddPoint(PointA);
        Curve.AddPoint(PointB);

        Liner.UpdatePoints(PointA, PointB);

        PointAPrev = PointA;
        PointBPrev = PointB;
    }

    private void UpdateFollowers() {
        if (FollowerCount <= 0)
        { return; }

        foreach (Node? Node in GetChildren().Where(X => X is Packet))
        {
            Packet? P = (Packet)Node;
            P.PathUpdated(Length);
        }
    }

    public void SendMessage(Message _Msg) {
        Task.Run(async () => {
                    //Debug.WriteLine($"Sent Message to {_Msg.DestinationAddress}");
                    await CommsOutput.WriteAsync(_Msg);
                });
    }

    public void PacketArrived(string _ChildName) {
        FollowerCount--;

        Packet? ArrivedChild = GetChildren<Packet>()
            .FirstOrDefault(X => X.Name == _ChildName);

        Task.Run(async () => {
                    Message Msg = await CommsInput.ReadAsync();
                    NodeB.MessageReceived(Msg);
                });

        if (ArrivedChild is null)
        { return; }

        RemoveChild(ArrivedChild);
        ArrivedChild.Free();
    }

    public void ClearMessages() {
        if (CommsInput.Count != 0)
        { _ = CommsInput.ReadAllAsync(); }

        foreach (Packet Child in GetChildren<Packet>())
        { Child?.QueueFree(); }
    }

    private IEnumerable<T> GetChildren<T>() where T : Node
        => GetChildren()
            .Where(X => !X.IsQueuedForDeletion())
            .OfType<T>();
}
