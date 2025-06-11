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
    private Maybe<string> _SearchCriteria = Maybe<string>.None; 

    [Export]
    private OptionButton NodeList = null!;

    [Export]
    private ItemList DetailsList = null!;

    [Export]
    // ReSharper disable once InconsistentNaming
    private Button dbg_btn_Clear = null!;

    [Inject]
    public IDBWrapper DB;
    
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

    public void ChangeSearchCriteria(string? _SearchQ) {
        _SearchCriteria = _SearchQ;
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

        List<string> RecordData = [];
        
        IEnumerable<LuaProcessRecord> LPRecords = _SelectedNodeID == Maybe<string>.None 
           ? DB.LPRColl().FindAll() 
           : DB.LPRColl().Find(X => X.NodeID == _SelectedNodeID.Value);

        if (_SearchCriteria.HasValue)
        {
            //RecordData = LPRecords.Where(X => X.Action)
            
        }
        
        RecordData?.ForEach(X => DetailsList.AddItem(X.ToString()));
        
        Console.WriteLine($"Loaded {DetailsList.ItemCount} process messages.");
    }

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
