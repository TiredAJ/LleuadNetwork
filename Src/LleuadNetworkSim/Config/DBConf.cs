using Godot;

namespace LleuadNetworkSim.Config;

static public class DBConf
{
    static public string ConnectionString 
        => ProjectSettings.GlobalizePath("user://Data.db");

    static public string LPRCollName
        => "LPRCollection";

    static public string ChallengeCollName
        => "ChallengeCollection";
    
    static public string JourneyCollName 
        => "MessageJourneyCollection";
    
    static public string FinalMessageCollName 
        => "FinalMessageCollection";

    static public int MaxPageSize
        => 200;
}
