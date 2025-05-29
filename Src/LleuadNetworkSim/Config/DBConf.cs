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

    static public int LPRecordVersion
        => 2;

    static public int ChallengeRecordVersion
        => 2;
    
    static public int JourneyRecordVersion 
        => 2;
    
    static public int FinalMessageRecordVersion 
        => 2;

    static public int RecordVersion
        => LPRecordVersion | ChallengeRecordVersion | JourneyRecordVersion | FinalMessageRecordVersion;
}
