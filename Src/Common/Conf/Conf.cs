namespace Common.Conf;

static public class Conf
{
    public const string MAP_EXTENSION = ".lnmap";
    public const string CHALLENGE_EXTENSION = ".lnchng";
    public const long MAX_FILE_SIZE = (25 * (8 * (1024 ^ 2))); //25MiB
    public const int MAX_ITEMS = 50;
    public const int MAX_MESSAGE_SIZE = 5120 * 1024; //5MiB
}
