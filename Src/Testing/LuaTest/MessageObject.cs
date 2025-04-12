namespace LuaTest;

[Lua.LuaObject]
public partial record MessageObject(string Address, string Data)
{
    [Lua.LuaMember("Address")]
    public string Address { get; set; } = Address;

    [Lua.LuaMember("Data")]    
    public string Data { get; set; } = Data;
}
