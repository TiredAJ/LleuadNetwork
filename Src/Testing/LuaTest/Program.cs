using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace LuaTest;
class Program
{
    static private Benchmarkerer Temp = new Benchmarkerer();
    
    static void Main(string[] args) {
        BenchmarkRunner.Run<Benchmarkerer>();

        //Temp.LuaCSharp();
    }
}