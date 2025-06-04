using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Godot.Logging;

using LiteDB;

using LleuadNetworkSim.Config;

namespace LleuadNetworkSim.Models.Repo;

//Temporary until I can be bothered to get DI working. 
static public class Repo
{
    static readonly private LiteDatabase DB_INSTANCE = new(DBConf.ConnectionString);
    
    static private ILiteCollection<MessageJourneyRecord>? JourneyColl;
    static private ILiteCollection<FinalMessageRecord>? FinalMsgColl;
    static private ILiteCollection<ChallengeRecord>? ChallengeColl;
    static private ILiteCollection<LuaProcessRecord>? LPRColl;

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

    static private void CreateLPRInstance() {
        LPRColl = DB_INSTANCE.GetCollection<LuaProcessRecord>(DBConf.LPRCollName);
        
        LPRColl.EnsureIndex(X => X.NodeID);
    }

    static public void Dispose() {
        DB_INSTANCE.Dispose();
    }

    static public void StartupCheck() {
        ILiteCollection<DBConfRecord>? DBConfColl = DB_INSTANCE.GetCollection<DBConfRecord>(nameof(DBConfRecord));

        DBConfRecord? Conf = DBConfColl.FindAll().LastOrDefault();

        if (Conf is null)
        {
            DBConfColl.Insert(DBConfRecord.CurrentConf());
            GodotLogger.LogWarning("No DBConf found, inserting...");
            return;
        }

        if (!Conf.CheckVersions())
        {
            DB_INSTANCE.Rebuild();
            GodotLogger.LogWarning("DBConf mismatch. Rebuilding...");
        }

        DBConfColl.Upsert(DBConfRecord.CurrentConf());
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

    #region Deletion
    static public int DeleteAllLPRecords() {
        if (LPRColl is null)
        { CreateLPRInstance(); }

        int DeleteCount = LPRColl!.DeleteAll(); 
        
        DB_INSTANCE.Commit();
        
        return DeleteCount;
    }

    static public int DeleteAllJourneyRecords() {
        if (JourneyColl is null)
        { CreateJourneyInstance(); }

        return JourneyColl!.DeleteAll();
    }

    static public int DeleteAllFinalMsgRecords() {
        if (FinalMsgColl is null)
        { CreateFinalMsgInstance(); }

        return FinalMsgColl!.DeleteAll();
    }

    static public int DeleteAllChallengeRecords() {
        if (ChallengeColl is null)
        { CreateChallengeInstance(); }

        return ChallengeColl!.DeleteAll();
    }
    #endregion

    #region Get
    static public IEnumerable<LuaProcessRecord> FindAll(int _Max = -1) {
        if (LPRColl is null)
        { CreateLPRInstance(); }
        
        return LPRColl!.FindAll()
                      .Take(_Max == -1 ? DBConf.MaxPageSize : _Max);
    }
    #endregion

    #region Count
    static public long LPRecordCount() {
        if (LPRColl is null)
        { CreateLPRInstance(); }
        
        return LPRColl!.LongCount();
    }
    #endregion

    #region Insert
    static public BsonValue Insert(LuaProcessRecord _LPR) {
        if (LPRColl is null)
        { CreateLPRInstance(); }

        return LPRColl!.Insert(_LPR);
    }
    #endregion

    #region Find
    static public IEnumerable<LuaProcessRecord> FindBy(Expression<Func<LuaProcessRecord, bool>> _Predicate) {
        if (LPRColl is null)
        { CreateLPRInstance(); }
        
        return LPRColl!.Find(_Predicate);
    }
    #endregion

    #region RawAccess
    static public ILiteCollection<LuaProcessRecord> GetLPRCollection() {
        if (LPRColl is null)
        { CreateLPRInstance(); }        
        
        return LPRColl;
    }
    #endregion
}

