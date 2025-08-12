namespace Common.Challenge;

//TODO: TEST - serialisation
public class ProtocolType
{
    required public bool IsOrdered { get; set; }
    required public string Name { get; set; }

    static public ProtocolType Default(string _Name = "DEFAULT", bool _IsOrdered = true)
        => new() { Name = _Name, IsOrdered = _IsOrdered };
}
