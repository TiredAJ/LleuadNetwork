using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Godot;
using Godot.Logging;

using LiteDB;

using LleuadNetworkSim.Config;
using LleuadNetworkSim.Models.Repo;
using LleuadNetworkSim.Scripts.Buttons;

using Timer = System.Threading.Timer;

namespace LleuadNetworkSim.Scripts;

public partial class win_DetailView : Window
{
    private LiteDatabase LDB = null!;
    private ILiteCollection<LuaProcessRecord> LPRCollection = null!;
    private ILiteCollection<MessageJourneyRecord> MsgJourneyCollection = null!;
    private ILiteCollection<FinalMessageRecord> FinalMsgCollection = null!;
    private ILiteCollection<ChallengeRecord> ChallengeRecordCollection = null!;
    private List<string> NodeIDs = [];
    private Views Selectedview = Views.LuaProcess;
    private bool IsAutoRefreshing = false;
    private Timer RefreshTimer = null!;

    [Export]
    private OptionButton NodeList = null!;

    [Export]
    private ItemList DetailsList = null!;

    [Export]
    // ReSharper disable once InconsistentNaming
    private Button dbg_btn_Clear = null!;

    public override void _Ready() {

        LDB = new LiteDatabase($"Filename={DBConf.ConnectionString};ReadOnly=true");

        Console.WriteLine($"Loading from [{DBConf.ConnectionString}]");
        
        LPRCollection = LDB.GetCollection<LuaProcessRecord>(DBConf.LPRCollName);
        MsgJourneyCollection = LDB.GetCollection<MessageJourneyRecord>(DBConf.JourneyCollName);
        FinalMsgCollection = LDB.GetCollection<FinalMessageRecord>(DBConf.FinalMessageCollName);
        ChallengeRecordCollection = LDB.GetCollection<ChallengeRecord>(DBConf.ChallengeCollName);

        RefreshTimer = new Timer(_Refresh);

#if DEBUG
        dbg_btn_Clear.Visible = true;
#endif
        
        base._Ready();
    }

    public void UpdateNodes(List<string> _NodeIDs) {
        NodeIDs = _NodeIDs;

        SetupNodeList();
    }

    public void UpdateRunning(bool _IsRunning) {
        IsAutoRefreshing = _IsRunning;
    }

    private void SetupNodeList() {
        foreach (string NodeID in NodeIDs)
        { NodeList.AddItem(NodeID); }

        NodeList.Selected = -1;
        
        Console.WriteLine("Populated NodeIDs");
    }

    public void ChangeView(string _SelectedView) {
        Selectedview = _SelectedView switch {
            "Script_Output" => Views.LuaProcess,
            "Challenge_Output" => Views.ChallengeRecord,
            "Message_Journey" => Views.MsgJourney,
            _ => Selectedview
        };
    }
    
    public override void _Notification(int _Notification)
    {
        if (_Notification == NotificationWMCloseRequest)
        { LDB.Dispose(); }
        
        base._Notification(_Notification);
    }
    
    private enum Views
    {
        LuaProcess,
        MsgJourney,
        FinalMessage,
        ChallengeRecord
    }

    public void Refresh() {

        if (IsAutoRefreshing)
        { IsAutoRefreshing = false; }
        
        _Refresh(null);
    }

    public void SetAutoRefresh(DetailViewRefreshMode _Mode) {

        Console.WriteLine($"Autorefresh set to {_Mode}");
        
        if (_Mode == DetailViewRefreshMode.Manual)
        {
            RefreshTimer.Change(Timeout.Infinite, Timeout.Infinite);
            IsAutoRefreshing = false;
            return;
        }

        long Interval = _Mode switch {
            DetailViewRefreshMode.OneSec => 1000,
            DetailViewRefreshMode.FiveSec => 5000,
            DetailViewRefreshMode.TenSec => 10000,
            _ => 1000L
        };

        RefreshTimer.Change(500, Interval);
    }

    private void _Refresh(object? _) {

        Console.WriteLine("refreshing");

        if (LPRCollection.Count() <= 0)
        {
            Console.WriteLine("no records to load");
            return;
        }

        List<LuaProcessRecord> LPRecords = LPRCollection.FindAll().Take(DBConf.MaxPageSize).ToList();

        DetailsList.Clear();
        
        LPRecords?.ForEach(X => DetailsList.AddItem(X.ToString()));

        Console.WriteLine($"Loaded {DetailsList.ItemCount} process messages.");
    }

    public void Clear() {

        int TotalDeletedRecords = 0;
        
        switch (Selectedview)
        {
            case Views.LuaProcess:
                TotalDeletedRecords = LPRCollection.DeleteAll();
                break;
            case Views.MsgJourney:
                TotalDeletedRecords = MsgJourneyCollection.DeleteAll();
                break;
            case Views.FinalMessage:
                TotalDeletedRecords = FinalMsgCollection.DeleteAll();
                break;
            case Views.ChallengeRecord:
                TotalDeletedRecords = ChallengeRecordCollection.DeleteAll();
                break;
        }
        
        GodotLogger.LogInfo($"Purged {TotalDeletedRecords} record(s)...");
    }
}