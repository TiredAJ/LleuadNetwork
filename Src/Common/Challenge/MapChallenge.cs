using System.Text.Json;

using Common.Messaging;

using MoreLinq;

namespace Common.Challenge;

public class MapChallenge
{
    readonly private Dictionary<string, List<Message>> Challenge = [];
    private Dictionary<Message, int> Distribution = [];
    private List<string> NodeAddresses = [];

    public string Name { get; set; }

    public MapChallenge(string _FilePath) {
        Name = Path.GetFileName(_FilePath);
        LoadFile(_FilePath);
    }

    public MapChallenge(List<string> _NodeAddresses, Dictionary<Message, int> _Distribution) {
        Name = "N/A";
        NodeAddresses = _NodeAddresses;
        Distribution = _Distribution;
    }

    public Dictionary<string, List<Message>> GenerateChallenge(List<string> _NodeAddresses) {
        NodeAddresses = _NodeAddresses;

        return GenerateChallenge();
    }

    public Dictionary<string, List<Message>> GenerateChallenge() {
        Message[] AllMessagesArr = DistributeMessages();

        (new Random((int)DateTime.Now.Ticks)).Shuffle(AllMessagesArr);

        List<Message[]> AllMessages = AllMessagesArr.Batch(NodeAddresses.Count).ToList();

        for(int i = 0; i < NodeAddresses.Count; i++)
        {
            string Sender = NodeAddresses[i];

            List<string> AvailableAddr = NodeAddresses.Where(X => X != Sender).ToList();

            Message[] Messages = AllMessages[i];

            foreach (Message Msg in Messages)
            {
                Msg.SenderAddress = Sender;
                Msg.DestinationAddress = AvailableAddr.RandomSubset(1).First();
            }

            Challenge.Add(Sender, Messages.ToList());
        }

        return Challenge;
    }

    private Message[] DistributeMessages() {
        List<Message> AllMessages = [];

        foreach (KeyValuePair<Message, int> KVP in Distribution)
        {
            for (int i = 0; i < KVP.Value; i++)
            { AllMessages.Add(KVP.Key.Clone()); }
        }

        return AllMessages.ToArray();
    }

    private void LoadFile(string _Path) {

        using Stream Reader = new FileStream(_Path, FileMode.Open);

        Dictionary<Message, int>? Data = JsonSerializer.Deserialize<Dictionary<Message, int>>(Reader);

        Distribution = Data ?? new Dictionary<Message, int>();
    }
}
