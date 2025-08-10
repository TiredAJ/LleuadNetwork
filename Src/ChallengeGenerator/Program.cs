using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

using ChallengeGenerator.Exceptions;

using Common.Challenge;
using Common.Json;
using Common.Json.Exceptions;

using CSharpFunctionalExtensions;

using FileUtils;

using Spectre.Console;

using static Common.Conf.Conf;

namespace ChallengeGenerator;

static internal class Program
{
    static readonly private ChallengeVO Challenge = new();
    static private string? MapName;
    static private string MapPath = "";
    static private string TmpFolderPath = "";
    static private string ChallengeSaveFolder = "";
    static private string ChallengeSaveFile = "";

    static void Main() {
        PrintTitle();
        
        MapPath = GetMap();

        AnsiConsole.MarkupLine($"[cyan1]Using [[{MapPath}]][/]");

        Challenge.Map = LoadMap(MapPath);
        
        MapName = Path.GetFileName(MapPath);
        
        Challenge.NodeCount = GetNodeCount(Challenge.Map);

        Challenge.ItemCount = GetItemCount();
        
        SelectMessageTypes();

        SetMessageTypeDistribution();

        TmpFolderPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        Directory.CreateDirectory(TmpFolderPath);
        
        GenerateMessages();

        ConfirmChallenge();

        ZipChallenge();
    }

    #region Misc
    static private void PrintTitle() {
        AnsiConsole.Clear();
        
        StringBuilder SB = new("[blue bold]LleuadNetwork Challenge Generator");

        if (MapName is not null)
        { SB.Append($" - {MapName}"); }

        if (Challenge.NodeCount > -1)
        { SB.Append($" - {Challenge.NodeCount} nodes"); }

        if (Challenge.ItemCount > -1)
        { SB.Append($" - {Challenge.ItemCount} messages"); }

        if (Challenge.MessageDistribution.Count > 0)
        { SB.Append($" - [[{Challenge.MessageDistribution.Keys.ZipStr(X => X.Name,"|")}]]"); }

        SB.Append("[/]");
        
        AnsiConsole.MarkupLine(SB.ToString());
    }
    #endregion

    #region Step 1 - Map
    static private MapVO? LoadMap(string _MapPath)
    {
        MapVO? Map = null;
        AnsiConsole.Status()
            .Start("[cyan1]Loading map...[/]",
            _CTX => {
                _CTX.Spinner(Spinner.Known.BouncingBar);
                
                AnsiConsole.MarkupLine("[italic]Validating json[/]");
                Thread.Sleep(350);
                
                Maybe<Exception> EXC = JsonValidator.ValidateJson<MapVO>(_MapPath, out Stream? JStream);

                if (EXC.HasValue)
                {
                    AnsiConsole.WriteException(EXC.Value);
                    return;
                }

                if (JStream is null)
                {
                    AnsiConsole.WriteException(new Exception("JStream was null!"));
                    return;
                }
                
                Thread.Sleep(300);

                AnsiConsole.MarkupLine("[italic]Deserialising file[/]");
                Thread.Sleep(300);

                Result<MapVO> MapRes = JSONHelper.Deserialise<MapVO>(JStream);
                
                if (MapRes.IsFailure)
                { throw new JSONDeserialisationException(nameof(MapVO)); }

                Map = MapRes.Value;
                
                Thread.Sleep(300);
            });
        
        return Map;
    }

    static private string GetMap()
    {
        return AnsiConsole.Prompt(
        new TextPrompt<string>($"[cyan1]Please enter filepath of map to use ([bold]*{MAP_EXTENSION}[/])[/]")
            .Validate((_FPath) => {
                if (!File.Exists(_FPath) ||
                    Path.GetExtension(_FPath) != MAP_EXTENSION)
                {
                    return ValidationResult
                        .Error($"[red]Invalid file, must end with [bold]{MAP_EXTENSION}[/][/]");
                }

                return ValidationResult.Success();
            }
            )
        );
    }
    #endregion

    #region Step 2 - Node Count
    static private int GetNodeCount(MapVO? _Map)
    {
        int ChosenNodeCount;

        do
        {
            PrintTitle();

            ChosenNodeCount = AnsiConsole.Prompt(
            new TextPrompt<int>($"[cyan1]There are [bold]{_Map!.NetworkNodes.Length}[/] node(s) in this map. " +
                                $"How many would you like to use?[/]")
                .DefaultValue(_Map.NetworkNodes.Length)
                .Validate((_Val) => (_Val <= _Map.NetworkNodes.Length) && (_Val > 0))
            );
        } while (
            !AnsiConsole.Prompt(new ConfirmationPrompt($"[cyan1]You have chosen [bold]{ChosenNodeCount}[/] nodes to use. Is this amount correct?[/]"))
        );
        return ChosenNodeCount;
    }
    #endregion

    #region Step 3 = Items
    static private int GetItemCount() {
        int ChosenMsgCount;

        do
        {
            PrintTitle();
            string Prompt = $"[cyan1]How many Items would you like in the challenge? [bold][[{Challenge.NodeCount}-{MAX_ITEMS}]][/][/]" +
                            $"\n[grey82]An item is the object to \"send\". An amount of messages that can transport all the items will be " +
                            $"calculated[/]";
            
            ChosenMsgCount = AnsiConsole.Prompt(
            new TextPrompt<int>(Prompt)
                .DefaultValue(15)
                .Validate((_Val) => (_Val <= MAX_ITEMS) && (_Val >= Challenge.NodeCount))
            );
        } while (
            !AnsiConsole.Prompt(new ConfirmationPrompt($"[cyan1]You have chosen [bold]{ChosenMsgCount}[/] items to generate. " +
                                                       $"Is this amount correct?[/]"))
        );

        return ChosenMsgCount;
    }
    #endregion

    #region Step 4 - Message Types
    static private void SelectMessageTypes() {

        if (!File.Exists(MESSAGE_TYPES_FILE))
        {
            Exception EXC = new FileNotFoundException($"Cannot find {MESSAGE_TYPES_FILE} File!");
            
            AnsiConsole.WriteException(EXC);
            throw EXC;
        }

        Result<List<MessageType>> AvailableTypes = JSONHelper.DeserialiseFromFile<List<MessageType>>(MESSAGE_TYPES_FILE);

        if (AvailableTypes.IsFailure)
        {
            Exception EXC = new JSONDeserialisationException(nameof(MapVO));
            
            AnsiConsole.WriteException(EXC);
            throw EXC;
        }

        if (AvailableTypes.Value.Count < 1)
        {
            Exception EXC = new NoMessageTypesAvailableException(MESSAGE_TYPES_FILE);
            
            AnsiConsole.WriteException(EXC);
            throw EXC;
        }

        List<MessageType> SelectedMessageTypes;

        do
        {
            PrintTitle();
            
            SelectedMessageTypes = AnsiConsole.Prompt(new MultiSelectionPrompt<MessageType>()
                .Title("[cyan1]Please select what message types you'd like in this challenge.[/]")
                .Required(true)
                .AddChoices(AvailableTypes.Value)
                .UseConverter(X => X.ToString())
                .InstructionsText("[grey](Press [blue]<space>[/] to toggle a message type, [green]<enter>[/] to accept)[/]")
            );

            if (SelectedMessageTypes.Count < 1)
            { AnsiConsole.MarkupLine("[red]You must select at least one message type[/]"); }

            SelectedMessageTypes.ForEach(X => Challenge.MessageDistribution.Add(X, 0));
        } while (
            !AnsiConsole.Prompt(new ConfirmationPrompt($"[cyan1] You've selected: [grey82]\n{PrintSelectedOptions(SelectedMessageTypes)}[/]Are you " +
                                                       $"happy with these choices?[/]"))            
            );
    }

    static private string PrintSelectedOptions(List<MessageType> _SelectedMessageTypes) {
        StringBuilder SB = new();
        
        _SelectedMessageTypes.ForEach(X => SB.Append($"\t{X.Name}\n"));

        return SB.ToString();
    }
    
    static private void SetMessageTypeDistribution() {
        int AvailablePercentage = 100;

        if (Challenge.MessageDistribution.Count == 1)
        {
            Challenge.MessageDistribution[Challenge.MessageDistribution.First().Key] = 100;
            return;
        }
        
        List<MessageType> AvailableTypes = new(Challenge.MessageDistribution.Keys);

        MessageType ExitMessageType = MessageType.Default(_Name: "Finished");

        SelectionPrompt<MessageType> DistributionPrompt = new SelectionPrompt<MessageType>()
            .UseConverter(X => (
                (X.Name == "Finished" || Challenge.MessageDistribution[X] == 0)
                    ? X.Name
                    : $"{X.Name} ({Challenge.MessageDistribution[X]}%)"
            ))
            .AddChoices(AvailableTypes);
        
        do
        {  
            PrintTitle();

            DistributionPrompt.Title($"[cyan1]Please select what message types you'd to set the distribution of. " +
                                     $"[grey]({AvailablePercentage}% remaining)[/][/]");
            
            MessageType SelectedOption = AnsiConsole.Prompt(DistributionPrompt);

            if (SelectedOption.Name == "Finished")
            {
                if (AvailablePercentage > 0)
                { continue; }
                break;
            }

            PrintTitle();

            int SpecificAvailablePercentage = Challenge.MessageDistribution[SelectedOption] + AvailablePercentage;
            
            int Percentage = AnsiConsole.Prompt(new TextPrompt<int>(
                $"[cyan1]Please enter the percentage distribution you'd like for [green]{SelectedOption.Name}[/][/]")
                .Validate(X => X <= SpecificAvailablePercentage && X >= 0)
                .DefaultValue(SpecificAvailablePercentage)
            );

            Challenge.MessageDistribution[SelectedOption] = Percentage;
            AvailablePercentage = 100 - Challenge.MessageDistribution.Values.Sum();

            Debug.WriteLine(AvailablePercentage);
            
            if (AvailablePercentage == 0)
            { DistributionPrompt.AddChoice(ExitMessageType); }

        } while (true);
    }
    #endregion

    #region Step 5 - Message Gen
    static private void GenerateMessages() {
        List<string> NodeIDs = Challenge.Map.NetworkNodes.Select(X => X.Name)
            .ToList();
        
        Challenge.GeneratedMessages = new MessageGenerator(Challenge.MessageDistribution, NodeIDs, Challenge.ItemCount, TmpFolderPath).GenerateMessages();
    }
    #endregion

    #region Step 6 - Challenge Summary 
    static private void DisplaySummary() {
        AnsiConsole.Clear();

        int NodeCount = Challenge.Map.NetworkNodes.Length;
        int ConnectionCount = Challenge.Map.NetworkNodes.Sum(X => X.Connections.Length);

        Tree SummaryTree = new("[cyan1 bold]Summary[/]");

        TreeNode MapNode = SummaryTree.AddNode(new Markup($"[cyan1 bold]Map:[/] {MapName}"));
        
        TreeNode NodesNode = MapNode.AddNode(new Markup($"[cyan1]Nodes[/] [grey82]({NodeCount})[/]"));
        NodesNode.AddNodes(Challenge.Map.NetworkNodes.Select(X => X.Name));

        IEnumerable<string> AllConnections = Challenge.Map.NetworkNodes
            .Select(X => X.Connections.Select(Y => $"[green]{X.Name}[/] [cyan2]->[/] [green]{Y}[/]"))
            .SelectMany(X => X);
        
        TreeNode ConnectionsNode = MapNode.AddNode(new Markup($"[cyan1]Connections[/] [grey82]({ConnectionCount})[/]"));
        ConnectionsNode.AddNodes(AllConnections);

        TreeNode MessageNode = SummaryTree.AddNode(new Markup("[cyan1 bold]Messages[/]"));

        MessageNode.AddNode($"[cyan1]Amount to Generate:[/] {Challenge.ItemCount}");
        
        IEnumerable<string> MsgDistribution = Challenge.MessageDistribution
            .OrderByDescending(X => X.Value)
            .Select(X => $"{X.Key.Name} [grey82]({X.Value}%)[/]");
        
        MessageNode.AddNode(new Markup($"[cyan1]Types[/] [grey82]({Challenge.MessageDistribution.Count})[/]")).AddNodes(MsgDistribution);
        
        AnsiConsole.Write(SummaryTree);
    }

    static private void ConfirmChallenge() {
        DisplaySummary();
        
        AnsiConsole.WriteLine();

        bool Confirmation = AnsiConsole.Prompt(new ConfirmationPrompt($"[cyan1]Would you like to use this challenge?[/]"));

        if (!Confirmation)
        { return; }

        bool IsNameValid = true;
        
        do
        {
            if (!IsNameValid)
            { AnsiConsole.MarkupLine("[red]File name already exists at that location, please choose a new name and path[/]"); }
            
            Challenge.Name =
                AnsiConsole.Prompt(new TextPrompt<string>("[cyan1]Please enter a name for the challenge[/]")).Trim();

            ChallengeSaveFolder =
                AnsiConsole.Prompt(new TextPrompt<string>("[cyan1]Please enter a folder to save the challenge to[/]")
                    .Validate(_S => Directory.Exists(_S.Trim())));
            
            ChallengeSaveFile = Path.Combine(ChallengeSaveFolder, Challenge.Name + CHALLENGE_EXTENSION);

            if (File.Exists(ChallengeSaveFile))
            { IsNameValid = false; }
            else
            { IsNameValid = true; }
            
        } while (!IsNameValid);
    }
    #endregion
    
    #region Step 7 - Challenge Zipping
    static private void ZipChallenge() {
        PrintTitle();

        Random RndOffset = new Random((int)DateTime.Now.Ticks);
        
        AnsiConsole.Status()
            .Start("[cyan1]Exporting challenge...[/]",
            _CTX => {
                CopyMap(_CTX);
                
                Thread.Sleep(RndOffset.Next(800, 1500));

                ExportMessages(_CTX);
                
                Thread.Sleep(RndOffset.Next(800, 1500));
                
                ExportChallengeDataFile(_CTX);
                
                Thread.Sleep(RndOffset.Next(800, 1500));
                
                ExportChallengeZip(_CTX);
                
                Thread.Sleep(RndOffset.Next(800, 1500));
            });
        
        AnsiConsole.Clear();
        
        AnsiConsole.Markup($"[cyan1 bold]Export has completed. " +
                           $"You can find it at [[[italic]{Path.Combine(ChallengeSaveFolder, Challenge.Name)}[/]]]. " +
                           $"please restart application to make a new challenge.[/]");
    }

    static private void ExportMessages(StatusContext _CTX) {
        AnsiConsole.MarkupLine("[italic]Exporting messages...[/]");
        _CTX.Spinner(Spinner.Known.BouncingBar);
        
        string TmpMsgFile = Path.Combine(TmpFolderPath, ZIP_MESSAGES_FILE);
        
        JSONHelper.SerialiseToFile(TmpMsgFile, Challenge.GeneratedMessages);
    }

    static private void CopyMap(StatusContext _CTX) {
        AnsiConsole.MarkupLine("[italic]Copying map...[/]");
        _CTX.Spinner(Spinner.Known.BouncingBar);
                
        string TmpMapFile = Path.Combine(TmpFolderPath, ZIP_MAP_FILE);

        File.Copy(MapPath, TmpMapFile);
    }

    static private void ExportChallengeDataFile(StatusContext _CTX)
    {
        AnsiConsole.MarkupLine("[italic]Exporting challenge data...[/]");
        _CTX.Spinner(Spinner.Known.BouncingBar);
        
        string TmpChallengeFile = Path.Combine(TmpFolderPath, ZIP_CHALLENGE_FILE);

        JSONHelper.SerialiseToFile(TmpChallengeFile, Challenge);
    }

    static private void ExportChallengeZip(StatusContext _CTX) {
        AnsiConsole.MarkupLine("[italic]Exporting challenge...[/]");
        _CTX.Spinner(Spinner.Known.BouncingBar);
        
        Zipper.Compress(ChallengeSaveFile, TmpFolderPath);
    }
    #endregion
}
