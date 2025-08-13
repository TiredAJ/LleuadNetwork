using System.Numerics;
using System.Text.Json.Serialization;

using Common.Json;

namespace Common.Tests.JSON.Files;

public class ExampleUser : IBaseVO
{
    public int Age {
        get => DateTime.Now.Year - DOB.Year;
    }
    public DateOnly DOB { get; set; }
    public string Name { get; set; }
    public Vector2 Coordinates { get; set; }
    public bool IsAdmin { get; set; }
    public HashSet<SecurityPermission> Perms { get; set; }
    
    public int __Version { get => 1; }
    static public int GetVersion()
        => 1;
}

public record SecurityPermission
{
    public byte[] SecurityKey { get; set; }
    public string Name { get; set; }
    public string Passphrase { get; set; }
}

[JsonSerializable(typeof(ExampleUser))]
[JsonSerializable(typeof(List<ExampleUser>))]
[JsonSourceGenerationOptions(IncludeFields = true)]
public partial class UserJSONContext : JsonSerializerContext {

}
