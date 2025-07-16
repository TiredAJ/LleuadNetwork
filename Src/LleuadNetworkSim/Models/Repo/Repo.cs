using System;

using LiteDB;

using LleuadNetworkSim.Config;
using LleuadNetworkSim.Models.Repo.Entities;

namespace LleuadNetworkSim.Models.Repo;

public sealed class Repo : IDBWrapper
{
    private LiteDatabase DB { get; }

    public Repo(string? _Location = null) {
        if (_Location is null)
        { _Location = DBConf.ConnectionString; }

#if DEBUG
        Console.WriteLine($"Opening DB at {_Location}");
#endif
        DB = new LiteDatabase(_Location);

        if (DB.UserVersion != DBConf.RecordVersion)
        {
            //TODO add actual record versioning pls
            DB.Rebuild();
            DB.UserVersion = DBConf.RecordVersion;
        }

        _LPRColl = DB.GetCollection<LuaProcessRecord>(DBConf.LPR_COLL_NAME);
        _JourneyColl = DB.GetCollection<MessageJourneyRecord>(DBConf.JOURNEY_COLL_NAME);
        _FinalMsgColl = DB.GetCollection<FinalMessageRecord>(DBConf.FINAL_MESSAGE_COLL_NAME);
        _ChallengeColl = DB.GetCollection<ChallengeRecord>(DBConf.CHALLENGE_COLL_NAME);

        SetupColls();
    }

    private void SetupColls() {
        _LPRColl.EnsureIndex(X => X.NodeID);
        _LPRColl.EnsureIndex(X => X.Action);
        _LPRColl.Include(X => X.Challenge);

        _JourneyColl.EnsureIndex(X => X.MessageID);
        _JourneyColl.EnsureIndex(X => X.Sender);
        _JourneyColl.EnsureIndex(X => X.Destination);
        _JourneyColl.Include(X => X.Challenge);

        _FinalMsgColl.EnsureIndex(X => X.MessageID);
        _FinalMsgColl.EnsureIndex(X => X.Sender);
        _FinalMsgColl.EnsureIndex(X => X.Destination);
        _FinalMsgColl.Include(X => X.Challenge);

        _ChallengeColl.EnsureIndex(X => X.ChallengeName);
    }

    public ILiteCollection<T> GetCollection<T>(string _Name) {
        return DB.GetCollection<T>(_Name);
    }

    #region Disposing
    private void Dispose(bool _Disposing) {
        if (_Disposing)
        {
            Console.WriteLine("Disposing DB!");
            DB.Checkpoint();
            DB.Dispose();
        }
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~Repo() => Dispose(false);
    #endregion

    #region Repos
    private ILiteCollection<LuaProcessRecord> _LPRColl { get; init; }
    public ILiteCollection<LuaProcessRecord> LPRColl() => _LPRColl;

    private ILiteCollection<MessageJourneyRecord> _JourneyColl { get; init; }
    public ILiteCollection<MessageJourneyRecord> JourneyColl() => _JourneyColl;

    private ILiteCollection<FinalMessageRecord> _FinalMsgColl { get; init; }
    public ILiteCollection<FinalMessageRecord> FinalMsgColl() => _FinalMsgColl;

    private ILiteCollection<ChallengeRecord> _ChallengeColl { get; init; }
    public ILiteCollection<ChallengeRecord> ChallengeColl() => _ChallengeColl;
    #endregion

    public void Checkpoint() {
        DB.Rebuild();
        DB.Checkpoint();
    }
}
