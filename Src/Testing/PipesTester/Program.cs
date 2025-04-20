namespace PipesTester;

class Program
{
    static private Switchbox SwitchBoxer = new Switchbox();

    static private Node NodeA = new("12");
    static private Node NodeB = new("12");
    
    static void Main() {
        SwitchBoxer.Connect(NodeA, NodeB);

        NodeA.Writer = DoingStuffClass.Write;
        NodeB.Writer = DoingStuffClass.Write;

        NodeA.OnReceive = DoingStuffClass.Read;
        NodeB.OnReceive = DoingStuffClass.Read;
        
        Task TaskA = NodeA.StartNode();
        Task TaskB = NodeB.StartNode();

        Task.WaitAll(TaskA, TaskB);
    }
}
