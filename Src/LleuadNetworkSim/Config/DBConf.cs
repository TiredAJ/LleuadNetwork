using Godot;

namespace LleuadNetworkSim.Config;

static public class DBConf
{
    static public string ConnectionString
        => ProjectSettings.GlobalizePath("user://Data.db");

    public const string LPR_COLL_NAME = "LPRCollection";

    public const string CHALLENGE_COLL_NAME = "ChallengeCollection";

    public const string JOURNEY_COLL_NAME = "MessageJourneyCollection";

    public const string FINAL_MESSAGE_COLL_NAME = "FinalMessageCollection";

    static public int MaxPageSize
        => 200;

    static public int LPRecordVersion
        => 3;

    static public int ChallengeRecordVersion
        => 3;

    static public int JourneyRecordVersion
        => 3;

    static public int FinalMessageRecordVersion
        => 3;

    static public int RecordVersion
        => LPRecordVersion | ChallengeRecordVersion | JourneyRecordVersion | FinalMessageRecordVersion;
}
