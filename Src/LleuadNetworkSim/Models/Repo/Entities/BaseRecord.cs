using System;

namespace LleuadNetworkSim.Models.Repo.Entities;

public record BaseRecord : IBaseRecord
{
    public DateTime RecordCreationTime { get; init; }

    public BaseRecord() {
        RecordCreationTime = DateTime.UtcNow;
    }
}
