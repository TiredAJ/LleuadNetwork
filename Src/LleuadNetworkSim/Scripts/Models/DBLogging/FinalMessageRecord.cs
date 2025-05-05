using System;

namespace LleuadNetworkSim.Scripts.Models.DBLogging;

/// <summary>
/// Records the state of a message once it's dropped/consumed.
/// </summary>
public record FinalMessageRecord(
    string NodeId,
    string Sender,
    string Destination,
    string Type,
    int Index,
    DateTime CreationTime,
    TimeSpan LifeSpan,
    TimeSpan ActiveTime,
    int Hops);
