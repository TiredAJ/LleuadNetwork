using BenchmarkDotNet.Attributes;

using MoonSharp.Interpreter;
using MoonSharp.VsCodeDebugger;

namespace LuaTest;

[MemoryDiagnoser]
public class Benchmarkerer
{
    static private string LuaCode => "./Test.lua";
    
    static private string LuaCodeLocal => """
                                              function Load (Messages)
                                                  print("Load called");

                                                  for i,v in ipairs(Messages) do
                                                      Direct(v);
                                                  end
                                              end

                                              local function Direct (Msg)
                                                  print("Direct called");

                                                  if Msg.Address == "Ya Mum" then
                                                      print("Ya Mum");
                                                      print(Msg.Data);
                                                  else
                                                      print("Not Ya Mum");
                                                      print(Msg.Data);
                                                  end
                                          	end

                                              return Direct;
                                          	
                                          """;

    static private string LCSCode => """
                                         local function Direct (Msg)
                                             print("Direct called");

                                             --[[if Msg.Address == "Ya Mum" then
                                                 print("Ya Mum");
                                                 print(Msg.Data);
                                             else
                                                 print("Not Ya Mum");
                                                 print(Msg.Data);
                                             end--]]
                                     	end

                                         return Direct;
                                         
                                     """;

    static private List<MessageObject> Messages = [
        new ("Ya Mum",     "Message 1"),
        new ("Ya Fatha",   "Message 2"),
        new ("Ya Nan",     "Message 3"),
        new ("Ya Grandad", "Message 4"),
        new ("Ya Dog",     "Message 5"),
        new ("Ya Cat",     "Message 6"),
    ];

    [Benchmark]
    public void MoonsSharp(MoonSharpVsCodeDebugServer _Server) {

        string File1 = "./Test2.lua";
        string File2 = "./Test3.lua";
        string ScriptFile = "/home/aj/Desktop/AssembledScript.lua";
        
        ScriptAssembler.AddScript(1, File2);
        ScriptAssembler.AddScript(2, File1);

        Task T = ScriptAssembler.AssembleScript(ScriptFile);

        Task.WaitAll(T);
        
        try
        {
            Script script = new Script();
            
            script.Globals["Reg_Save"] = (Action<string, DynValue>)Save;
            script.Globals["Reg_Load"] = (Func<string, DynValue>)Load;

            script.DoFile(ScriptFile);
            
            _Server.AttachToScript(script, "LuaC");
            Console.ReadKey();
            
            script.Call(script.Globals["LoadMsgs"], Messages);
            
            _Server.Detach(script);
        }
        catch (InterpreterException e)
        { Console.WriteLine(e); }

        Console.WriteLine(Register.Count);
        
        return;
    }
    
    private Dictionary<string, DynValue> Register = [];

    private void Save(string _Key, DynValue _Val) => Register.TryAdd(_Key, _Val);

    private DynValue Load(string _Key) => Register[_Key];
}
