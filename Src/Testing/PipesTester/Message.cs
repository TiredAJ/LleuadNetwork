namespace PipesTester;

public record Message<T>(string Address, T Payload)
{
    public string Address { get; set; } = Address;
    public T Payload { get; set; } = Payload;
}
