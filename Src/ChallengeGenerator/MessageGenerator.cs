using System.Collections;
using System.Text.Json;

using Common.Challenge;
using Common.Json;
using Common.Messaging;

using static Common.Conf.Conf;

namespace ChallengeGenerator;

public class MessageGenerator
{
    readonly private Dictionary<MessageType, int> Distribution = [];
    private string TempFolderPath = "";
    
    public MessageGenerator(Dictionary<MessageType, int> _Distribution, int _NoItems, string _TempFolder) {
        foreach (KeyValuePair<MessageType, int> KVP in _Distribution)
        { Distribution.Add(KVP.Key, (_NoItems / 100) * KVP.Value); }

        TempFolderPath = _TempFolder;
    }

    /// <summary>
    /// Generates the messages and saves them to a temporary file.
    /// </summary>
    /// <returns>The path to the temporary file.</returns>
    public string GenerateMessages() {
        List<Message> Messages = [];
        
        foreach (KeyValuePair<MessageType, int> KVP in Distribution)
        { Messages.AddRange(GenerateMessages(KVP.Key, KVP.Value)); }

        return SaveToFile(Messages);
    }
    
    private List<Message> GenerateMessages(MessageType _MT, int _Count) {
        List<Message> TmpMsgs = [];
        
        for (int i = 0; i < _Count; i++)
        { TmpMsgs.AddRange(GenerateMessagesForItem(_MT)); }

        return TmpMsgs;
    }

    private List<Message> GenerateMessagesForItem(MessageType _MT) {
        List<Message> TmpMessages = [];
        
        long ItemSize = Random.Shared.NextInt64(_MT.SizeRange.Min, _MT.SizeRange.Max);

        long NoMessagesNeeded = ItemSize / MAX_MESSAGE_SIZE;
        long SizeRemaining = ItemSize % MAX_MESSAGE_SIZE;

        for (int i = 0; i < NoMessagesNeeded; i++)
        {
            var Msg = Message.Builder
                .WithIndex(i)
                .WithMessageType(_MT.Name)
                .WithMessageSize(MAX_MESSAGE_SIZE)
                .WithTotalSize(ItemSize);

            if (i == NoMessagesNeeded--)
            { Msg.WithMessageSize((int)SizeRemaining); }
            
            TmpMessages.Add(Msg.Build());
        }

        return TmpMessages;
    }

    private string SaveToFile(List<Message> _Msgs) {
        
        string FilePath = Path.Combine(TempFolderPath, "Messages.json");
        
        string TempFile = Path.Combine(FilePath, Path.GetRandomFileName());
        
        using StreamWriter Writer = new(TempFile);

        JsonSerializer.Serialize(Writer.BaseStream, _Msgs, SourceGenerationContext.Default.ListMessage);

        return TempFile;
    }
}
