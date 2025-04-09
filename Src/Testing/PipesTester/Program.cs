using System.IO.Pipelines;

using Microsoft.VisualBasic;

namespace PipesTester;

class Program
{
    static private Switchbox SwitchBoxer = new Switchbox();

    static private Node WriterNode = new("12");
    static private Node ReaderNode = new("12");
    
    static void Main() {
        SwitchBoxer.Connect(WriterNode);
        SwitchBoxer.Connect(ReaderNode);

        WriterNode.Writer = (Count) => new Message<string>("Reader Node", 
                                                           $"{Count}: ");

        ReaderNode.OnReceive = (_m) => { Console.WriteLine($"Message received for {_m.Address}, with payload {_m.Payload}"); };
        
        WriterNode.StartWriting();
        ReaderNode.StartReading();
    }
}
