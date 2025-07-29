using Common.Challenge;
using Common.Messaging;

using MoreLinq;

using static Common.Conf.Conf;

namespace ChallengeGenerator;

public class MessageGenerator
{
    readonly private Dictionary<MessageType, int> Distribution = [];
    readonly private List<string> Nodes = [];
    private string TempFolderPath = "";
    
    public MessageGenerator(Dictionary<MessageType, int> _Distribution, List<string> _Nodes, int _NoItems, string _TempFolder) {
        Nodes = _Nodes;
        
        foreach (KeyValuePair<MessageType, int> KVP in _Distribution)
        { Distribution.Add(KVP.Key, (int)((_NoItems / 100f) * KVP.Value)); }

        TempFolderPath = _TempFolder;
    }

    /// <summary>
    /// Generates the messages and saves them to a temporary file.
    /// </summary>
    /// <returns>The path to the temporary file.</returns>
    public Dictionary<string, List<Message>> GenerateMessages() {
        Dictionary<string, List<Message>> TmpMsgs = [];

        foreach (KeyValuePair<string, List<Message>> KVP2 in Distribution.SelectMany(KVP => GenerateMessages(KVP.Key, KVP.Value)))
        {
            if (TmpMsgs.TryGetValue(KVP2.Key, out List<Message>? Msgs))
            { Msgs.AddRange(KVP2.Value); }
            else
            { TmpMsgs.Add(KVP2.Key, KVP2.Value); } 
        }

        return TmpMsgs;
    }
    
    private Dictionary<string, List<Message>> GenerateMessages(MessageType _MT, int _Count) {
        Dictionary<string, List<Message>> TmpMsgs = [];

        for (int i = 0; i < _Count; i++)
        {
            List<string> ChosenNodes = Nodes.RandomSubset(2).ToList();

            List<Message> Msgs = GenerateMessagesForItem(_MT, ChosenNodes[0], ChosenNodes[1]);
            
            if (TmpMsgs.TryGetValue(ChosenNodes[0], out List<Message>? MsgsVal))
            { MsgsVal.AddRange(MsgsVal); }
            else
            { TmpMsgs.Add(ChosenNodes[0], Msgs); }
        }

        return TmpMsgs;
    }

    static private List<Message> GenerateMessagesForItem(MessageType _MT, string _DestNode, string _SenderNode) {
        List<Message> TmpMessages = [];
        
        long ItemSize = Random.Shared.NextInt64(_MT.SizeRange.Min, _MT.SizeRange.Max);

        long NoMessagesNeeded = (long)Math.Ceiling(ItemSize / (double)MAX_MESSAGE_SIZE);
        long SizeRemaining = ItemSize % MAX_MESSAGE_SIZE;

        for (int i = 0; i < NoMessagesNeeded; i++)
        {
            MessageBuilder? Msg = MessageBuilder.Default()
                .WithIndex(i)
                .WithMessageType(_MT.Name)
                .WithMessageSize(MAX_MESSAGE_SIZE)
                .WithTotalSize(ItemSize)
                .WithDestinationAddress(_DestNode)
                .WithSenderAddress(_SenderNode);

            if (i == --NoMessagesNeeded)
            { Msg.WithMessageSize((int)SizeRemaining); }
            
            TmpMessages.Add(Msg.Build());
        }

        return TmpMessages;
    }
}
