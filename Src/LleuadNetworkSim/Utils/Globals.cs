using LiteDB;
// ReSharper disable InconsistentNaming

namespace LleuadNetworkSim.Utils;

static public class Globals
{
    static public int G_TotalMessagesInPlay_Ref;
    
    static public int G_TotalMessagesInPlay { 
        get => G_TotalMessagesInPlay_Ref; 
        set => G_TotalMessagesInPlay_Ref = value;        
    }
    static public ObjectId? G_ChallengeID { get; set; }
}
