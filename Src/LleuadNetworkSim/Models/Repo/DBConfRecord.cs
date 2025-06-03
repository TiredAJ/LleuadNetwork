using LiteDB;

using LleuadNetworkSim.Config;

using NJsonSchema;

namespace LleuadNetworkSim.Models.Repo;

public record DBConfRecord
{
    [BsonId]
    public int ID { get; } = -1;
    
    public int LPRecordVersion { get; set; }
    public int ChallengeRecordVersion { get; set; }
    public int JourneyRecordVersion { get; set; }
    public int FinalMessageRecordVersion { get; set; }
    
    public int RecordVersion
        => LPRecordVersion | ChallengeRecordVersion | JourneyRecordVersion | FinalMessageRecordVersion;

    public bool CheckVersions()
        => (LPRecordVersion == DBConf.LPRecordVersion) 
           && (ChallengeRecordVersion == DBConf.ChallengeRecordVersion) 
           && (JourneyRecordVersion == DBConf.JourneyRecordVersion) 
           && (FinalMessageRecordVersion == DBConf.FinalMessageRecordVersion);

    static public DBConfRecord CurrentConf()
        => new() {
            LPRecordVersion = DBConf.LPRecordVersion,
            ChallengeRecordVersion = DBConf.ChallengeRecordVersion,
            JourneyRecordVersion = DBConf.JourneyRecordVersion,
            FinalMessageRecordVersion = DBConf.FinalMessageRecordVersion
        };
};
