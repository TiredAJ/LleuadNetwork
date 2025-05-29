using System;
using System.Collections.Generic;
using System.Linq;

using Godot;

using LiteDB;

using LleuadNetworkSim.Config;
using LleuadNetworkSim.Models.Repo;

using MoreLinq;

namespace LleuadNetworkSim.Scripts;

public partial class win_DetailView : Window
{
    private LiteDatabase LDB;
    private ILiteCollection<LuaProcessRecord> LPRCollection;
    private ILiteCollection<MessageJourneyRecord> MsgJourneyCollection;
    private ILiteCollection<FinalMessageRecord> FinalMsgCollection;
    private ILiteCollection<ChallengeRecord> ChallengeRecord;
    private List<string> NodeIDs = [];
    private Views Selectedview = Views.LuaProcess;
    private bool IsAutoRefreshing = false;

    [Export]
    private OptionButton NodeList = null!;

    [Export]
    private ItemList DetailsList = null!;

    public override void _Ready() {

        LDB = new LiteDatabase($"Filename={DBConf.ConnectionString};ReadOnly=true");

        LPRCollection = LDB.GetCollection<LuaProcessRecord>(DBConf.LPRCollName);
        MsgJourneyCollection = LDB.GetCollection<MessageJourneyRecord>(DBConf.JourneyCollName);
        FinalMsgCollection = LDB.GetCollection<FinalMessageRecord>(DBConf.FinalMessageCollName);
        ChallengeRecord = LDB.GetCollection<ChallengeRecord>(DBConf.ChallengeCollName);
        
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
        
        _Refresh();
    }

    private void _Refresh() {
        if (LPRCollection.Count() <= 0)
        { return; }

        List<LuaProcessRecord> LPRecords = LPRCollection.FindAll().Take(DBConf.MaxPageSize).ToList();

        DetailsList.Clear();
                
        LPRecords?.ForEach(X => DetailsList.AddItem(X.ToString()));

        Console.WriteLine($"Loaded {DetailsList.ItemCount} process messages.");
    }
}