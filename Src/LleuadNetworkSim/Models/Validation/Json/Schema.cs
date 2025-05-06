using System.ComponentModel.DataAnnotations;

namespace LleuadNetworkSim.Models.Validation.Json;

#pragma warning disable CS8618

public interface IBaseVO
{ }

public record CollectionNodeVO : IBaseVO
{
    static public int SchemaVersion => 2;
    public int _Version { get => 2; }
    
    [Required]
    public NetworkNodeVO[] NetworkNodes { get; set; }
}

public record NetworkNodeVO : IBaseVO
{
    static public int SchemaVersion => 2;
    public int _Version { get => 2; }
    
    [Required]
    public string Name { get; set; }
    
    public string[] Connections { get; set; }

    [Required]
    public PositionVectorVO Pos { get; set; }
}

public record PositionVectorVO : IBaseVO
{
    static public int SchemaVersion => 2;
    public int _Version { get => 2; }
    
    [Required]
    public float X { get; set; }
    
    [Required]
    public float Y { get; set; }
}

#pragma warning restore CS8618