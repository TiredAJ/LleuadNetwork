using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

using MoreLinq;

using Msg = LleuadNetworkSim.Models.Messaging.Message;

namespace LleuadNetworkSim.Models;

public class MapChallenge
{
    readonly private Dictionary<string, List<Msg>> Challenge = [];
    private Dictionary<Msg, int> Distribution = [];
    private List<string> NodeAddresses = [];

    public string Name { get; set; }

    public MapChallenge(string _FilePath) {
        Name = Path.GetFileName(_FilePath);
        LoadFile(_FilePath);
    }

    public MapChallenge(List<string> _NodeAddresses, Dictionary<Msg, int> _Distribution) {
        Name = "N/A";
        NodeAddresses = _NodeAddresses;
        Distribution = _Distribution;
    }

    public Dictionary<string, List<Msg>> GenerateChallenge(List<string> _NodeAddresses) {
        NodeAddresses = _NodeAddresses;

        return GenerateChallenge();
    }

    public Dictionary<string, List<Msg>> GenerateChallenge() {
        Msg[] AllMessagesArr = DistributeMessages();

        (new Random((int)DateTime.Now.Ticks)).Shuffle(AllMessagesArr);

        List<Msg[]> AllMessages = AllMessagesArr.Batch(NodeAddresses.Count).ToList();

        for(int i = 0; i < NodeAddresses.Count; i++)
        {
            string Sender = NodeAddresses[i];
            
            List<string> AvailableAddr = NodeAddresses.Where(X => X != Sender).ToList();

            Msg[] Messages = AllMessages[i];

            foreach (Msg Msg in Messages)
            {
                Msg.SenderAddress = Sender;
                Msg.DestinationAddress = AvailableAddr.RandomSubset(1).First();
            }
            
            Challenge.Add(Sender, Messages.ToList());
        }

        return Challenge;
    }

    private Msg[] DistributeMessages() {
        List<Msg> AllMessages = [];

        foreach (KeyValuePair<Msg, int> KVP in Distribution)
        {
            for (int i = 0; i < KVP.Value; i++)
            { AllMessages.Add(KVP.Key.Clone()); }
        }

        return AllMessages.ToArray();
    }

    private void LoadFile(string _Path) {
        
        using Stream Reader = new FileStream(_Path, FileMode.Open);

        Dictionary<Msg, int>? Data = JsonSerializer.Deserialize<Dictionary<Msg, int>>(Reader);

        Distribution = Data ?? new Dictionary<Msg, int>();
    }
}
