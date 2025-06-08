using System;

namespace LleuadNetworkSim.Models.Repo;

public record BaseRecord
{
    public DateTime RecordCreationTime { get; init; }

    public BaseRecord() {
        RecordCreationTime = DateTime.UtcNow;
    }
}
