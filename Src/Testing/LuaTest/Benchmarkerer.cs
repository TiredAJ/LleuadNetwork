using BenchmarkDotNet.Attributes;

using Lua;

using LuaFunction = NLua.LuaFunction;

namespace LuaTest;

public class Benchmarkerer
{
    static private string LuaCode => @"
	local function add (a, b)
		return a + b;
	end
	";

    static private int ValA = 12;

    static private int ValB = 24;
    
    
    [Benchmark]
    public async Task<long> NLuaTest() {
        await Task.Run(() => {
                           using NLua.Lua lua = new NLua.Lua();

                           lua.DoString(LuaCode);
                           LuaFunction? ScriptFunc = lua["add"] as NLua.LuaFunction;

                           return (long)ScriptFunc.Call(3, 5)
                                                  .First();
                       });
    }

    [Benchmark]
    public async Task<long> NeoLuaTest() {
        using Neo.IronLua.Lua lua = new Neo.IronLua.Lua();

        dynamic env = lua.CreateEnvironment();
        env.dochunk(LuaCode, "test.lua");
        return env.add(ValA, ValB);
    }

    [Benchmark]
    public async Task<long> LuaCSharp() {

        LuaState lua = Lua.LuaState.Create();
        
        LuaValue[] LuaValues = await lua.DoStringAsync(LuaCode);
        
        Lua.LuaFunction Func = LuaValues[0].Read<Lua.LuaFunction>();

        LuaValue[] FuncResult = await Func.InvokeAsync(lua, [(double)ValA, (double)ValB]);
        
        return FuncResult[0].Read<long>();
    }
}
