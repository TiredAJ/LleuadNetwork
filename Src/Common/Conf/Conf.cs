namespace Common.Conf;

static public class Conf
{
    public const string MAP_EXTENSION = ".lnmap";
    public const string CHALLENGE_EXTENSION = ".lnchng";
    public const string ZIP_CHALLENGE_FILE = "Challenge.json";
    public const string ZIP_MAP_FILE = "Map.json";
    public const string ZIP_MESSAGES_FILE = "Messages.json";
    public const string MESSAGE_TYPES_FILE = "./Conf/MessageTypes.json";
    
    public const long MAX_FILE_SIZE = (25 * (8 * (1024 * 1024))); //25MiB
    public const int MAX_ITEMS = 50;
    public const int MAX_MESSAGE_SIZE = (1024 * 1024); //5MiB
}
