using System;

using LiteDB;

namespace LleuadNetworkSim.Models.Repo;

public interface IDBWrapper : IDisposable
{
    public ILiteCollection<T> GetCollection<T>(string _Name);

    public ILiteCollection<LuaProcessRecord> LPRColl();
}
