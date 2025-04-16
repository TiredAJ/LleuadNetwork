using System.Diagnostics;
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

        WriterNode.Writer = DoingStuffClass.Write;

        ReaderNode.OnReceive = DoingStuffClass.Read;
        
        Task ReadTask = ReaderNode.StartReading();
        Task WriteTask = WriterNode.StartWriting();

        Task.WaitAll(ReadTask, WriteTask);
    }
}
