using System.ComponentModel.DataAnnotations;

using Godot;

using Newtonsoft.Json;

namespace Common.Json;

#pragma warning disable CS8618

public interface IBaseVO
{ }

public record MapVO : IBaseVO
{
    static public int SchemaVersion => 3;
    public int __Version { get => SchemaVersion; }

    [Required]
    public NetworkNodeVO[] NetworkNodes { get; set; }
    
    public HashSet<string> UniqueConnections { get; set; }
}

public record NetworkNodeVO : IBaseVO
{
    static public int SchemaVersion => 2;
    public int __Version { get => SchemaVersion; }

    [Required]
    public string Name { get; set; }
    
    [JsonIgnore]
    public string[] Connections { get; set; }

    [Required]
    public PositionVectorVO Pos { get; set; }
}

public record PositionVectorVO : IBaseVO
{
    static public int SchemaVersion => 2;
    public int __Version { get => SchemaVersion; }

    [Required]
    public float X { get; set; }

    [Required]
    public float Y { get; set; }

    public PositionVectorVO() {}
    
    public PositionVectorVO(float _X, float _Y) {
        X = _X;
        Y = _Y;
    }

    static public PositionVectorVO From(float _X, float _Y)
        => new(_X, _Y);
    
    static public implicit operator Vector2(PositionVectorVO _PVVO) 
        => new(_PVVO.X, _PVVO.Y);
    
    static public implicit operator PositionVectorVO(Vector2 _Vec2) 
        => new(_Vec2.X, _Vec2.Y);
}

#pragma warning restore CS8618
