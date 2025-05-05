using BenchmarkDotNet.Attributes;

using MoonSharp.Interpreter;

namespace LuaTest;

[MemoryDiagnoser]
public class Benchmarkerer
{
    static private string LuaCode => """
                                         function Load (Messages)
                                             print("Load called");

                                             for i,v in ipairs(Messages) do
                                                 Direct(v);
                                             end
                                         end

                                     	function Direct (Msg)
                                             print("Direct called");

                                             if Msg.Address == "Ya Mum" then
                                                 print("Ya Mum");
                                                 print(Msg.Data);
                                             else
                                                 print("Not Ya Mum");
                                                 print(Msg.Data);
                                             end
                                     	end
                                     	
                                     """;
    
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
    
    /*[Benchmark]
    public async Task NLuaTest() => await Task.Run(() => {
        using NLua.Lua lua = new NLua.Lua();

        lua.DoString(LuaCode);
        NLua.LuaFunction? ScriptFunc = lua["Load"] as NLua.LuaFunction;

        _ = ScriptFunc.Call(Messages)[0];
    });*/
 
    /*[Benchmark]
    public async Task NeoLuaTest() => await Task.Run(() => {
        using Neo.IronLua.Lua lua = new Neo.IronLua.Lua();

        dynamic env = lua.CreateEnvironment();
        env.dochunk(LuaCode, "test.lua");
        env.Load(Messages);
    });*/

    /*[Benchmark]
    public async Task LuaCSharp() {
        try
        {
            LuaState lua = Lua.LuaState.Create();

            Lua.LuaValue[] LuaValues = await lua.DoFileAsync("./Test.lua");
        
            Lua.LuaFunction Func = LuaValues[0].Read<Lua.LuaFunction>();

            foreach (MessageObject MSG in Messages)
            {
                Lua.LuaValue[] FuncResult = await Func.InvokeAsync(lua, [ ]);
                //FuncResult[0].Read<long>();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }*/

    [Benchmark]
    public void MoonsSharp() {

        Script script = new Script();
        
        script.DoString(LuaCode);
        
        script.Call(script.Globals["Direct"], )
    }
}
