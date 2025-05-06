using Godot;

using LiteDB;

namespace LleuadNetworkSim.Models.Repo;

public class Repo
{
    static private string DefaultLocation => ProjectSettings.GlobalizePath("user://Data.db");

    static readonly private LiteDatabase DB_INSTANCE = new(DefaultLocation);

    static private ILiteCollection<MessageJourneyRecord>? JourneyColl;
    private const string JOURNEY_COLL_NAME = "MessageJourneyCollection";

    static private ILiteCollection<FinalMessageRecord>? FinalMsgColl;
    private const string FINAL_MSG_COLL_NAME = "FinalMessageCollection";

    static private ILiteCollection<ChallengeRecord>? ChallengeColl;
    private const string CHALLENGE_COLL_NAME = "ChallengeRecordCollection";

    static private void CreateJourneyInstance() {
        JourneyColl = DB_INSTANCE.GetCollection<MessageJourneyRecord>(JOURNEY_COLL_NAME);

        JourneyColl.EnsureIndex(X => X.MessageID);
        JourneyColl.EnsureIndex(X => X.Sender);
        JourneyColl.EnsureIndex(X => X.Destination);
        JourneyColl.EnsureIndex(X => X.ChallengeID);
    }

    static private void CreateFinalMsgInstance() {
        FinalMsgColl = DB_INSTANCE.GetCollection<FinalMessageRecord>(FINAL_MSG_COLL_NAME);
        
        FinalMsgColl.EnsureIndex(X => X.MessageID);
        FinalMsgColl.EnsureIndex(X => X.Sender);
        FinalMsgColl.EnsureIndex(X => X.Destination);
        FinalMsgColl.EnsureIndex(X => X.ChallengeID);
    }

    static private void CreateChallengeInstance() {
        ChallengeColl = DB_INSTANCE.GetCollection<ChallengeRecord>(CHALLENGE_COLL_NAME);

        ChallengeColl.EnsureIndex(X => X.ChallengeName);
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

