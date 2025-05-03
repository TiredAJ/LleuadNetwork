using System.ComponentModel.DataAnnotations;

namespace LleuadNetworkSim.Utils.Validators.Json;

#pragma warning disable CS8618

public interface IBaseVO
{ }

public record CollectionNodeVO : IBaseVO
{
    [Required]
    public NetworkNodeVO[] NetworkNodes { get; set; }
}

public record NetworkNodeVO : IBaseVO
{
    [Required]
    public string Name { get; set; }
    
    public string[] Connections { get; set; }

    [Required]
    public PositionVectorVO Pos { get; set; }
}

public record PositionVectorVO : IBaseVO
{
    [Required]
    public float X { get; set; }
    
    [Required]
    public float Y { get; set; }
}

#pragma warning restore CS8618