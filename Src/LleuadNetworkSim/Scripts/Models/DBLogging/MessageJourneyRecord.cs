namespace LleuadNetworkSim.Scripts.Models.DBLogging;

/// <summary>
/// A point in the journey of a message. Should be generated on message reception.
/// </summary>
public record MessageJourneyRecord(
    string ID,
    string Sender,
    string Destination,
    string Type,
    string CurrentLocation,
    int Hops);
