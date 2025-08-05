using System.Diagnostics;
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
    private Dictionary<string, List<Message>> Messages = [];
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

            this.Messages.Add(Sender, Messages.ToList());
        }

        return Messages;
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

        DirectoryInfo TmpDir = FileUtils.FileUtils.CreateTempFolder();
        
        Result Res = Zipper.Extract(_FilePath, TmpDir.FullName);
        
        Debug.WriteLine($"Creating temp dir at {TmpDir.FullName}");

        if (Res.IsFailure)
        { return Res.ConvertFailure<MapChallenge>(); }

        MapChallenge NewMapChallenge = new();
        
        string ChallengeDataPath = Path.Combine(TmpDir.FullName, ZIP_CHALLENGE_FILE);
        string MapPath = Path.Combine(TmpDir.FullName, ZIP_MAP_FILE);
        string MessagesPath = Path.Combine(TmpDir.FullName, ZIP_MESSAGES_FILE);

        Result ChallengeRes = LoadChallengeData(ChallengeDataPath, NewMapChallenge);

        if (ChallengeRes.IsFailure)
        { return ChallengeRes.ConvertFailure<MapChallenge>(); }
        
        Result MapRes = LoadMap(MapPath, NewMapChallenge);
        
        if (MapRes.IsFailure)
        { return MapRes.ConvertFailure<MapChallenge>(); }
        
        Result MsgRes = LoadMessages(MessagesPath, NewMapChallenge);
        
        if (MsgRes.IsFailure)
        { return MsgRes.ConvertFailure<MapChallenge>(); }

        return Result.Success(NewMapChallenge); 
    }

    static private Result LoadChallengeData(string _ChallengeDataPath, MapChallenge _NewMapChallenge) {
        using StreamReader Reader = new(_ChallengeDataPath);

        Maybe<ChallengeVO> MbChallenge = Maybe<ChallengeVO>.None;
        
        try
        { MbChallenge = JsonSerializer.Deserialize(Reader.BaseStream, VO_SrcGenCtx.Default.ChallengeVO).AsMaybe(); }
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

        MapVO? Map;

        try
        { Map = JsonSerializer.Deserialize(Reader, VO_SrcGenCtx.Default.MapVO); }
        catch (Exception EXC)
        { return Result.Failure(EXC.Message); }

        if (Map is null)
        { return Result.Failure("Map was null"); }
        
        _NewMapChallenge.Map = Map;

        return Result.Success();
    }

    static private Result LoadMessages(string _MessagePath, MapChallenge _NewMapChallenge) {
        using Stream Reader = new StreamReader(_MessagePath).BaseStream;

        Dictionary<string, List<Message>>? GeneratedMessages;

        try
        {
            GeneratedMessages =
                JsonSerializer.Deserialize(Reader, MsgDistribution_SrcGenCtx.Default.DictionaryStringListMessage);
        }
        catch (Exception EXC)
        {
            Debug.WriteLine(EXC);
            return Result.Failure(EXC.Message);
        }

        if (GeneratedMessages is null)
        { return Result.Failure("Messages was null"); }
        
        _NewMapChallenge.Messages = GeneratedMessages;

        return Result.Success();
    }
}
