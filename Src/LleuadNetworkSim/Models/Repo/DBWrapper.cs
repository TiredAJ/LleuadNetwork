using System;

using LiteDB;

using LleuadNetworkSim.Config;

namespace LleuadNetworkSim.Models.Repo;

public class DBWrapper : IDBWrapper
{
    public LiteDatabase DB { get; }

    public DBWrapper(string? _Location = null) {
        if (_Location is null)
        { _Location = DBConf.ConnectionString; }

        DB = new LiteDatabase(_Location);

        if (DB.UserVersion != DBConf.RecordVersion)
        {
            DB.Rebuild();
            DB.UserVersion = DBConf.RecordVersion;
        }

        _LPRColl = DB.GetCollection<LuaProcessRecord>(DBConf.LPRCollName);
    }

    public ILiteCollection<T> GetCollection<T>(string _Name) {
        return DB.GetCollection<T>(_Name);
    }

    #region Disposing
    private void Dispose(bool _Disposing) {
        if (_Disposing)
        {
            DB.Checkpoint();
            DB.Dispose();
        }
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    ~DBWrapper() => Dispose(false);
    #endregion

    #region Repos
    private ILiteCollection<LuaProcessRecord> _LPRColl { get; init; }

    public ILiteCollection<LuaProcessRecord> LPRColl() => _LPRColl;
    #endregion
}
