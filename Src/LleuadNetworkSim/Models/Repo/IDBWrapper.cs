using System;

using Common.Entities;

using LiteDB;

namespace LleuadNetworkSim.Models.Repo;

public interface IDBWrapper : IDisposable
{
    public ILiteCollection<T> GetCollection<T>(string _Name);

    public ILiteCollection<LuaProcessRecord> LPRColl();
    public ILiteCollection<MessageJourneyRecord> JourneyColl();
    public ILiteCollection<FinalMessageRecord> FinalMsgColl();
    public ILiteCollection<ChallengeRecord> ChallengeColl();
    void Checkpoint();
}
