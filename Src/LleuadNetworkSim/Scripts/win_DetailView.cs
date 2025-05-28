using System;
using System.Collections.Generic;

using Godot;

using LiteDB;

using LleuadNetworkSim.Config;
using LleuadNetworkSim.Models.Repo;

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

    public override void _Process(double delta) {
        
        
        
        base._Process(delta);
    }

    public void UpdateNodes(List<string> _NodeIDs) {
        NodeIDs = _NodeIDs;

        SetupNodeList();
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
}