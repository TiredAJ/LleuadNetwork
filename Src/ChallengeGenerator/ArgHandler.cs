using ChallengeGenerator.Exceptions;

using Common.Challenge;

using MoreLinq;

namespace ChallengeGenerator;

static public class ArgHandler
{
    static public ArgSettings? HandleArgs(string[] _Args) {
        
        Dictionary<string, List<string>> Args = [];
        
        _Args.ForEach(X => {
            if (!X.Contains('-') || !X.Contains('='))
            { throw new MalformedArgumentException(X); }
            else
            {
                string[] ArgComponents = X.Trim('-')
                    .Split('=');

                if (ArgComponents.Length != 2)
                { throw new MalformedArgumentException(X, ArgComponents.Length); }
                
                Args.Add(ArgComponents[0], ArgComponents[1].Split(',').ToList());
            }
        });

        return null;
    }
}

public class ArgSettings
{
    public string ChallengeSavePath { get; set; }
    public string MapPath { get; set; }
    public int NodeCount { get; set; }
    public int MessageCount { get; set; }
    public List<MessageType> MessageTypes { get; set; }
}

internal enum Switches
{
    MAP,
    CHALLENGE_SAVE,
    NODE_COUNT,
    MESSAGE_COUNT,
    MESSAGE_TYPES,
}
