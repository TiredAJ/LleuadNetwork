using Godot;

using Common.Entities;

namespace LleuadNetworkSim.Config;

static public class DBConf
{
    static public string ConnectionString
        => ProjectSettings.GlobalizePath("user://Data.db");

    public const string LPR_COLL_NAME = LuaProcessRecord.COLL_NAME;

    public const string CHALLENGE_COLL_NAME = ChallengeRecord.COLL_NAME;

    public const string JOURNEY_COLL_NAME = MessageJourneyRecord.COLL_NAME;

    public const string FINAL_MESSAGE_COLL_NAME = FinalMessageRecord.COLL_NAME;

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
