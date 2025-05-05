using LiteDB;

namespace LleuadNetworkSim.Scripts.Models.DBLogging;

public class Storer
{
    static private string DefaultLocation => "user://Data.db";

    static private LiteDatabase DBInstance = new LiteDatabase(DefaultLocation);

    static private ILiteCollection<MessageJourneyRecord>? JourneyColl = null;
    static private string JourneyColl_Name = "MessageJourneyCollection";
    
    static private ILiteCollection<FinalMessageRecord>? FinalMsgColl = null;
    static private string FinalMsgColl_Name = "FinalMessageCollection";
    
    static private ILiteCollection<ChallengeRecord>? ChallengeColl = null;
    static private string ChallengeColl_Name = "ChallengeRecordCollection";

    static private void CreateJourneyInstance()
        => JourneyColl = DBInstance.GetCollection<MessageJourneyRecord>(JourneyColl_Name);

    static private void CreateFinalMsgInstance()
        => FinalMsgColl = DBInstance.GetCollection<FinalMessageRecord>(FinalMsgColl_Name);

    static private void CreateChallengeInstance()
        => ChallengeColl = DBInstance.GetCollection<ChallengeRecord>(ChallengeColl_Name);

    static public void LogEvent() {
        
    }
}

