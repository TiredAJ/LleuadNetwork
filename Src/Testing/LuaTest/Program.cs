using System.Diagnostics;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using MoonSharp.Interpreter.REPL;
using MoonSharp.VsCodeDebugger;

namespace LuaTest;
class Program
{
    static private Benchmarkerer Temp = new Benchmarkerer();
    
    static void Main(string[] args) {
        //BenchmarkRunner.Run<Benchmarkerer>();
        /*UserData.RegisterType<MessageObject>();
        UserData.DefaultAccessMode = InteropAccessMode.Preoptimized;
        
        MoonSharpVsCodeDebugServer server = new();

        server.Start();
        
        Temp.MoonsSharp(server);*/

        Script Scrpt = new() {
            Options = {
                ScriptLoader = new ReplInterpreterScriptLoader() {
                    IgnoreLuaPathGlobal = true,
                    ModulePaths = [
                        "/usr/lib/lua/5.4/",
                        "/home/aj/.luarocks/lib/luarocks/rocks-5.4",
                        "/home/aj/.luarocks/lib/lua/5.4/?.so",
                        "/home/aj/.luarocks/share/lua/5.4/cjson/util.lua",
                        "/home/aj/.luarocks/share/lua/5.4/json2lua.lua",
                        "/home/aj/.luarocks/share/lua/5.4/lua2json.lua"
                    ]
                }
            }
        };
        
        Scrpt.Globals["Get_Value"] = (Func<int>)(() => 12);
        
        Scrpt.DoFile("./Lua/InfiniteLoop.lua");
        
        DynValue ProcessFunc = Scrpt.Globals.Get("Run");

        DynValue ProcessCoroutine = Scrpt.CreateCoroutine(ProcessFunc);
        
        ProcessCoroutine.Coroutine.AutoYieldCounter = 60000;
        
        Task T1 = Task.Run(() => {
                               DynValue Res = ProcessCoroutine.Coroutine.Resume();
                               
                               Console.Write(Res.ToDebugPrintString());
                           });

        Task.WaitAll([T1]);
    }
}