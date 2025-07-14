using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using CSharpFunctionalExtensions;

using Godot;
using Godot.DependencyInjection.Attributes;
using Godot.Logging;
using Godot.NativeInterop;

using LleuadNetworkSim.Config;
using LleuadNetworkSim.Models.Repo;
using LleuadNetworkSim.Models.Repo.Entities;
using LleuadNetworkSim.Scripts.Buttons;
using LleuadNetworkSim.Scripts.UI;
using LleuadNetworkSim.Utils;

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
    private BaseRecord? SelectedRecord;
    private BaseRecordDisplay? ChosenDisplay;

    [Export]
    private OptionButton NodeList = null!;

    [Export]
    private ItemList DetailsList = null!;

    [Export]
    // ReSharper disable once InconsistentNaming
    private Button dbg_btn_Clear = null!;

    [Export]
    private Control RecordDisplayParent = null!;

    [Export]
    private PackedScene FMRDisplayScene {
        get => throw new NotImplementedException("Please use the cached version instead."); 
        set => FMRDisp = new PackedSceneCache<FinalMessageDisplay>{Scene = value};
    }
    [Export]
    private PackedScene LPRDisplayScene {
        get => throw new NotImplementedException("Please use the cached version instead."); 
        set => LPRDisp = new PackedSceneCache<LPRDisplay> {Scene = value};
    }
    [Export]
    private PackedScene MJRDisplayScene {
        get => throw new NotImplementedException("Please use the cached version instead."); 
        set => MJRDisp = new PackedSceneCache<MessageJourneyDisplay> {Scene = value};
    }
    [Export]
    private PackedScene ChallengeDisplayScene {
        get => throw new NotImplementedException("Please use the cached version instead."); 
        set => ChallengeDisp = new PackedSceneCache<ChallengeDisplay> {Scene = value};
    }
    
    private PackedSceneCache<FinalMessageDisplay> FMRDisp = null!;
    private PackedSceneCache<LPRDisplay> LPRDisp = null!;
    private PackedSceneCache<MessageJourneyDisplay> MJRDisp = null!;
    private PackedSceneCache<ChallengeDisplay> ChallengeDisp = null!; 
    
    [Inject]
    private IDBWrapper DB = null!;
    
    public override void _Ready() {
        Console.WriteLine($"Loading from [{DBConf.ConnectionString}]");

        RefreshTimer = new Timer((_) => CallDeferredThreadGroup(nameof(_Refresh)));

#if DEBUG
        dbg_btn_Clear.Visible = true;
#endif
        
        ChosenDisplay = LPRDisp.GetInstance();
        
        SetRecordDisplay();
        
        base._Ready();
    }

    //public override bool _Set(StringName _Property, Variant _Value) {
    //    if (_Property != "FMRDisp")
    //    { return base._Set(_Property, _Value); }
    //
    //    FMRDisp = new PackedSceneCache<FinalMessageDisplay>() { Scene = (PackedScene)_Value };
    //    return true;
    //
    //}

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
        
        switch (_SelectedView)
        {
            case "Script_Output":
                ChosenDisplay = LPRDisp.GetInstance();
                Selectedview = Views.LUA_PROCESS;
                break;
            case "Challenge_Output":
                ChosenDisplay = ChallengeDisp.GetInstance();
                Selectedview = Views.CHALLENGE_RECORD;
                break;
            case "Message_Journey":
                ChosenDisplay = MJRDisp.GetInstance();
                Selectedview = Views.MSG_JOURNEY;
                break;
            case "Final_Message":
                ChosenDisplay = FMRDisp.GetInstance();
                Selectedview = Views.FINAL_MESSAGE;
                break;
            default:
                ChosenDisplay = null;
                Selectedview = Views.NONE;
                break;
            
        }
        
        SetRecordDisplay();
        _Refresh();
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
        CHALLENGE_RECORD,
        NONE
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
            case Views.NONE:
            default:
                return;
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
              .Find(X => (X.Sender == _SelectedNodeID.Value) || (X.Destination == _SelectedNodeID.Value)).ToList();

        return JourneyData.Select(X => X.ToString()).ToList();
    }

    private List<string> GetFinalMessageData() {
        FinalMessageData = DB.FinalMsgColl()
                             .FindAll()
                             .ToList();
        
        return FinalMessageData.Select(X => X.ToString()).ToList();
    }

    private List<string> GetChallengeData() {
        ChallengeData = DB.ChallengeColl()
                          .FindAll()
                          .ToList();
        
        return ChallengeData.Select(X => X.ToString()).ToList();
    }
    #endregion

    public void Clear() {
        int TotalDeletedRecords = Selectedview switch {
            Views.LUA_PROCESS => DB.LPRColl().DeleteAll(),
            Views.MSG_JOURNEY => DB.JourneyColl().DeleteAll(),
            Views.FINAL_MESSAGE => DB.FinalMsgColl().DeleteAll(),
            Views.CHALLENGE_RECORD => DB.ChallengeColl().DeleteAll(),
            _ => 0
        };

        GodotLogger.LogInfo($"Purged {TotalDeletedRecords} record(s)...");
        
        DetailsList.Clear();
    }

    public void SelectRecord(int _Index) {
        switch (Selectedview)
        {
            case Views.LUA_PROCESS:                
                SelectedRecord = LPRData[_Index];
                break;
            case Views.MSG_JOURNEY:                
                SelectedRecord = JourneyData[_Index];
                break;
            case Views.FINAL_MESSAGE:
                SelectedRecord = FinalMessageData[_Index];
                break;
            case Views.CHALLENGE_RECORD:
                SelectedRecord = ChallengeData[_Index];
                break;
            case Views.NONE:
            default:
                GodotLogger.LogInfo("No view selected.");
                return;
        }

        ChosenDisplay?.SetData(SelectedRecord);
    }

    private void SetRecordDisplay() {

        if (RecordDisplayParent.GetChildCount() > 0 || Selectedview == Views.NONE)
        { RecordDisplayParent.RemoveChild(RecordDisplayParent.GetChild(0)); }

        if (ChosenDisplay is null)
        { return; }

        if (SelectedRecord is not null)
        { ChosenDisplay.SetData(SelectedRecord); }


        RecordDisplayParent.AddChild(ChosenDisplay!);
    }
}
