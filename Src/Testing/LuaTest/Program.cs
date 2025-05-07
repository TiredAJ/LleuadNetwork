using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

using MoonSharp.Interpreter;
using MoonSharp.VsCodeDebugger;

namespace LuaTest;
class Program
{
    static private Benchmarkerer Temp = new Benchmarkerer();
    
    static void Main(string[] args) {
        //BenchmarkRunner.Run<Benchmarkerer>();
        UserData.RegisterType<MessageObject>();
        UserData.DefaultAccessMode = InteropAccessMode.Preoptimized;
        
        MoonSharpVsCodeDebugServer server = new();

        server.Start();
        
        Temp.MoonsSharp(server);
    }
}