using System.Linq;

using Godot;

using LiteDB;

using LleuadNetworkSim.Config;

namespace LleuadNetworkSim.Models.Repo;

public class Repo
{
    static private string DefaultLocation => ProjectSettings.GlobalizePath("user://Data.db");

    static readonly private LiteDatabase DB_INSTANCE = new(DefaultLocation);
    
    static private ILiteCollection<MessageJourneyRecord>? JourneyColl;
    static private ILiteCollection<FinalMessageRecord>? FinalMsgColl;
    static private ILiteCollection<ChallengeRecord>? ChallengeColl;
    static private ILiteCollection<DBConfRecord>? DBConfColl;

    static private void CreateJourneyInstance() {
        JourneyColl = DB_INSTANCE.GetCollection<MessageJourneyRecord>(DBConf.JourneyCollName);

        JourneyColl.EnsureIndex(X => X.MessageID);
        JourneyColl.EnsureIndex(X => X.Sender);
        JourneyColl.EnsureIndex(X => X.Destination);
        JourneyColl.EnsureIndex(X => X.ChallengeID);
    }

    static private void CreateFinalMsgInstance() {
        FinalMsgColl = DB_INSTANCE.GetCollection<FinalMessageRecord>(DBConf.FinalMessageCollName);
        
        FinalMsgColl.EnsureIndex(X => X.MessageID);
        FinalMsgColl.EnsureIndex(X => X.Sender);
        FinalMsgColl.EnsureIndex(X => X.Destination);
        FinalMsgColl.EnsureIndex(X => X.ChallengeID);
    }

    static private void CreateChallengeInstance() {
        ChallengeColl = DB_INSTANCE.GetCollection<ChallengeRecord>(DBConf.ChallengeCollName);

        ChallengeColl.EnsureIndex(X => X.ChallengeName);
    }

    static private void StartupCheck() {
        DBConfColl = DB_INSTANCE.GetCollection<DBConfRecord>(nameof(DBConfRecord));

        DBConfRecord? Conf = DBConfColl.FindAll().Last();

        if (Conf is null)
        { DBConfColl.Insert(DBConfRecord.CurrentConf()); }

        if (!Conf.CheckVersions())
        { DB_INSTANCE.Rebuild(); }
    }

    static public void LogEvent(MessageJourneyRecord _MJR) {
        if (JourneyColl is null)
        { CreateJourneyInstance(); }

        JourneyColl!.Insert(_MJR);
    }

    static public void LogEvent(FinalMessageRecord _FMR) {
        if (FinalMsgColl is null)
        { CreateFinalMsgInstance(); }

        FinalMsgColl!.Insert(_FMR);
    }

    static public ObjectId LogEvent(ChallengeRecord _CR) {
        if (ChallengeColl is null)
        { CreateChallengeInstance(); }

        return ChallengeColl!.Insert(_CR)["_id"].AsObjectId;
    }
}

