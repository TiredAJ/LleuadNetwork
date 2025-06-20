using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using CSharpFunctionalExtensions;

using Godot;
using Godot.DependencyInjection.Attributes;
using Godot.Logging;

using LleuadNetworkSim.Config;
using LleuadNetworkSim.Models.Repo;
using LleuadNetworkSim.Models.Repo.Entities;
using LleuadNetworkSim.Scripts.Buttons;

using Timer = System.Threading.Timer;

namespace LleuadNetworkSim.Scripts;

// ReSharper disable once InconsistentNaming
public partial class win_DetailView : Window
{
    private List<string> NodeIDs = [];
    private Views Selectedview = Views.LUA_PROCESS;
    private bool IsAutoRefreshing = false;
    private Timer RefreshTimer = null!;
    private Maybe<string> _SelectedNodeID = Maybe<string>.None;
    private Maybe<string> _FilterCriteria = Maybe<string>.None;
    private List<LuaProcessRecord> LPRData = [];
    private List<MessageJourneyRecord> JourneyData = [];
    private List<FinalMessageRecord> FinalMessageData = [];
    private List<ChallengeRecord> ChallengeData = [];

    [Export]
    private OptionButton NodeList = null!;

    [Export]
    private ItemList DetailsList = null!;

    [Export]
    // ReSharper disable once InconsistentNaming
    private Button dbg_btn_Clear = null!;

    [Inject]
    public IDBWrapper DB = null!;
    
    public override void _Ready() {
        Console.WriteLine($"Loading from [{DBConf.ConnectionString}]");

        RefreshTimer = new Timer((_) => CallDeferredThreadGroup(nameof(_Refresh)));

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
            "Script_Output" => Views.LUA_PROCESS,
            "Challenge_Output" => Views.CHALLENGE_RECORD,
            "Message_Journey" => Views.MSG_JOURNEY,
            _ => Selectedview
        };
    }

    public void ChangeSelectedNode(string? _Selection) {
        _SelectedNodeID = _Selection;
    }

    public void ChangeFilterCriteria(string? _FilterQ) {
        _FilterCriteria = _FilterQ;
    }
    
    private enum Views
    {
        LUA_PROCESS,
        MSG_JOURNEY,
        FINAL_MESSAGE,
        CHALLENGE_RECORD
    }

    public void Refresh() {

        if (IsAutoRefreshing)
        { IsAutoRefreshing = false; }
        
        _Refresh();
    }

    public void SetAutoRefresh(DetailViewRefreshMode _Mode) {

        Console.WriteLine($"Autorefresh set to {_Mode}");
        
        if (_Mode == DetailViewRefreshMode.MANUAL)
        {
            RefreshTimer.Change(Timeout.Infinite, Timeout.Infinite);
            IsAutoRefreshing = false;
            return;
        }

        long Interval = _Mode switch {
            DetailViewRefreshMode.ONE_SEC => 1000,
            DetailViewRefreshMode.FIVE_SEC => 5000,
            DetailViewRefreshMode.TEN_SEC => 10000,
            _ => 1000L
        };

        RefreshTimer.Change(500, Interval);
    }

    private void _Refresh() {

        Console.WriteLine("refreshing");

        if (DB.LPRColl().Count() <= 0)
        {
            Console.WriteLine("no records to load");
            return;
        }
        
        DetailsList.Clear();

        List<string> RecordData;

        switch (Selectedview)
        {
            default:
            case Views.LUA_PROCESS:
                RecordData = GetLPRData();
                break;
            case Views.MSG_JOURNEY:
                RecordData = GetJourneyData();
                break;
            case Views.FINAL_MESSAGE:
                RecordData = GetFinalMessageData();
                break;
            case Views.CHALLENGE_RECORD:
                RecordData = GetChallengeData();
                break;
        }
        
        RecordData.ForEach(X => DetailsList.AddItem(X));
        
        Console.WriteLine($"Loaded {DetailsList.ItemCount} messages.");
    }

    #region DataSources

    private List<string> GetLPRData() {
        LPRData = _SelectedNodeID == Maybe<string>.None 
           ? DB.LPRColl().FindAll().ToList()
           : DB.LPRColl().Find(X => X.NodeID == _SelectedNodeID.Value).ToList();
        
        return _FilterCriteria.HasValue
            ? LPRData.Where(X => X.Action == _FilterCriteria.Value)
                       .Select(X => X.ToString())
                       .ToList()
            : LPRData.Select(X => X.ToString())
                       .ToList();
    }

    private List<string> GetJourneyData() {
        JourneyData = _SelectedNodeID == Maybe<string>.None
          ? DB.JourneyColl().FindAll().ToList()
          : DB.JourneyColl()
              .Find(X => X.Sender == _SelectedNodeID.Value || X.Destination == _SelectedNodeID.Value).ToList();

        return JourneyData.Select(X => X.ToString()).ToList();
    }

    private List<string> GetFinalMessageData() {
        return [];
    }

    private List<string> GetChallengeData() {
        return [];
    }
    #endregion
    
    
    public void Clear() {

        int TotalDeletedRecords = 0;
        
        switch (Selectedview)
        {
            case Views.LUA_PROCESS:
                TotalDeletedRecords = DB.LPRColl().DeleteAll();
                break;
            case Views.MSG_JOURNEY:
                TotalDeletedRecords = DB.JourneyColl().DeleteAll();
                break;
            case Views.FINAL_MESSAGE:
                TotalDeletedRecords = DB.FinalMsgColl().DeleteAll();
                break;
            case Views.CHALLENGE_RECORD:
                TotalDeletedRecords = DB.ChallengeColl().DeleteAll();
                break;
        }
        
        GodotLogger.LogInfo($"Purged {TotalDeletedRecords} record(s)...");
        
        _Refresh();
    }
}
