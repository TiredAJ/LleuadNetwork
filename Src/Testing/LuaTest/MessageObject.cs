namespace LuaTest;

public partial record MessageObject(string Address, string Data)
{
    public string Address { get; set; } = Address;    
    public string Data { get; set; } = Data;
}
