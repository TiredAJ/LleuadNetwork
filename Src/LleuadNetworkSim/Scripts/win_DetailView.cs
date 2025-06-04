using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using CSharpFunctionalExtensions;

using Godot;
using Godot.Logging;

using LleuadNetworkSim.Config;
using LleuadNetworkSim.Models.Repo;
using LleuadNetworkSim.Scripts.Buttons;

using Timer = System.Threading.Timer;

namespace LleuadNetworkSim.Scripts;

public partial class win_DetailView : Window
{
    private List<string> NodeIDs = [];
    private Views Selectedview = Views.LuaProcess;
    private bool IsAutoRefreshing = false;
    private Timer RefreshTimer = null!;
    private Maybe<string> _SelectedNodeID = Maybe<string>.None;

    [Export]
    private OptionButton NodeList = null!;

    [Export]
    private ItemList DetailsList = null!;

    [Export]
    // ReSharper disable once InconsistentNaming
    private Button dbg_btn_Clear = null!;

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
            "Script_Output" => Views.LuaProcess,
            "Challenge_Output" => Views.ChallengeRecord,
            "Message_Journey" => Views.MsgJourney,
            _ => Selectedview
        };
    }

    public void ChangeSelectedNode(string? _Selection) {
        _SelectedNodeID = _Selection;
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
        
        _Refresh();
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

    private void _Refresh() {

        Console.WriteLine("refreshing");

        DetailsList.Clear();

        if (Repo.LPRecordCount() <= 0)
        {
            Console.WriteLine("no records to load");
            return;
        }

        var RawLinq = Repo.FindAll()
                          .Where(X => X.NodeID == _SelectedNodeID.Value)
                          .ToList();

        var LDB = Repo.GetLPRCollection()
                      .Find(X => X.NodeID == _SelectedNodeID.Value)
                      .ToList();

        var LDB2 = Repo.FindBy(X => X.NodeID == _SelectedNodeID.Value)
                       .ToList();
        
        List<LuaProcessRecord> LPRecords = _SelectedNodeID == Maybe<string>.None 
                                               ? Repo.FindAll().ToList() 
                                               : Repo.GetLPRCollection().Find(X => X.NodeID == _SelectedNodeID.Value).ToList();
        
        LPRecords?.ForEach(X => DetailsList.AddItem(X.ToString()));

        Console.WriteLine($"Loaded {DetailsList.ItemCount} process messages.");
    }

    public void Clear() {

        int TotalDeletedRecords = 0;
        
        switch (Selectedview)
        {
            case Views.LuaProcess:
                TotalDeletedRecords = Repo.DeleteAllLPRecords();
                break;
            case Views.MsgJourney:
                TotalDeletedRecords = Repo.DeleteAllJourneyRecords();
                break;
            case Views.FinalMessage:
                TotalDeletedRecords = Repo.DeleteAllFinalMsgRecords();
                break;
            case Views.ChallengeRecord:
                TotalDeletedRecords = Repo.DeleteAllChallengeRecords();
                break;
        }
        
        GodotLogger.LogInfo($"Purged {TotalDeletedRecords} record(s)...");
        
        _Refresh();
    }
}