using System.Text.Json;

using Common.Json;
using Common.Messaging;

using CSharpFunctionalExtensions;

using FileUtils;

using MoreLinq;

using static Common.Conf.Conf;

namespace Common.Challenge;

public class MapChallenge
{
    readonly private Dictionary<string, List<Message>> Challenge = [];
    private Dictionary<MessageType, int> Distribution = [];
    private List<string> NodeAddresses = [];

    public string Name { get; set; } = "DEFAULT";
    public MapVO Map { get; private set; }

    private MapChallenge() {}
    
    public MapChallenge(List<string> _NodeAddresses, Dictionary<MessageType, int> _Distribution) {
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

        return [];
        
        /*List<Message> AllMessages = [];

        foreach (KeyValuePair<MessageType, int> KVP in Distribution)
        {
            for (int i = 0; i < KVP.Value; i++)
            { AllMessages.Add(KVP.Key); }
        }

        return AllMessages.ToArray();*/
    }

    static public Result<MapChallenge> LoadChallenge(string _FilePath) {
        string TempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        Directory.CreateDirectory(TempPath);
        
        Result Res = Zipper.Extract(_FilePath, TempPath);

        if (Res.IsFailure)
        { return Res.ConvertFailure<MapChallenge>(); }

        MapChallenge NewMapChallenge = new MapChallenge();
        
        string ChallengeDataPath = Path.Combine(TempPath, ZIP_CHALLENGE_FILE);
        string MapPath = Path.Combine(TempPath, ZIP_MAP_FILE);
        string MessagesPath = Path.Combine(TempPath, ZIP_MESSAGES_FILE);

        LoadChallengeData(ChallengeDataPath, NewMapChallenge);
        LoadMap(MapPath, NewMapChallenge);
        LoadMessages(MessagesPath, NewMapChallenge);
    }

    static private Result LoadChallengeData(string _ChallengeDataPath, MapChallenge _NewMapChallenge) {
        using StreamReader Reader = new(_ChallengeDataPath);

        Maybe<ChallengeVO> MbChallenge = Maybe<ChallengeVO>.None;
        
        try
        { MbChallenge = JsonSerializer.Deserialize(Reader.BaseStream, SourceGenerationContext.Default.ChallengeVO).AsMaybe(); }
        catch (Exception EXC)
        { Result.Failure(EXC.Message); }
        
        if (MbChallenge.HasNoValue)
        { Result.Failure($"Failed to deserialise {_ChallengeDataPath}"); }

        _NewMapChallenge.Name = MbChallenge.Value.Name;
        _NewMapChallenge.Distribution = MbChallenge.Value.MessageDistribution;

        return Result.Success();
    }

    static private Result LoadMap(string _MapPath, MapChallenge _NewMapChallenge) {
        using Stream Reader = new StreamReader(_MapPath).BaseStream;

        MapVO? Map = JsonSerializer.Deserialize(Reader, SourceGenerationContext.Default.MapVO);
    }

    static private void LoadMessages(string _MessagePath, MapChallenge _Challenge) {
        
    }
}
