using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

using ChallengeGenerator.Exceptions;

using Common.Challenge;
using Common.Challenge.JSON;
using Common.Json;
using Common.Json.Exceptions;

using CSharpFunctionalExtensions;

using Spectre.Console;

using static Common.Conf;

namespace ChallengeGenerator;

static internal class Program
{
    static private ChallengeData Challenge = new();
    static private string? MapName = null;

    static private string MessageTypesFile = "./Conf/MessageTypes.json"; 
    
    static void Main(string[] _Args) {

        ArgHandler.HandleArgs(_Args);
        
        MapVO? Map = null;
        
        PrintTitle();
        
        string MapPath = GetMap();

        AnsiConsole.MarkupLine($"[cyan1]Using [[{MapPath}]][/]");

        Map = LoadMap(MapPath);
        
        MapName = Path.GetFileName(MapPath);
        
        Challenge.NodeCount = GetNodeCount(Map);

        Challenge.MessageCount = GetMessageCount();
        
        SelectMessageTypes();
    }

    static private void PrintTitle() {
        StringBuilder SB = new StringBuilder("[blue bold]LleuadNetwork Challenge Generator");

        if (MapName is not null)
        { SB.Append($" - {MapName}"); }

        if (Challenge.NodeCount > -1)
        { SB.Append($" - {Challenge.NodeCount} nodes"); }

        if (Challenge.MessageCount > -1)
        { SB.Append($" - {Challenge.MessageCount} messages"); }

        SB.Append("[/]");
        
        AnsiConsole.MarkupLine(SB.ToString());
    }

    static private MapVO? LoadMap(string _MapPath)
    {
        MapVO? Map = null;
        AnsiConsole.Status()
            .Start("[cyan1]Loading map...[/]",
            _CTX => {
                _CTX.Spinner(Spinner.Known.BouncingBar);
                
                AnsiConsole.MarkupLine("[italic]Validating json[/]");
                Thread.Sleep(350);
                
                Maybe<Exception> EXC = JsonValidator.ValidateJson<MapVO>(_MapPath, out JsonNode? JNode);

                if (EXC.HasValue)
                {
                    AnsiConsole.WriteException(EXC.Value);
                    return;
                }
                
                Thread.Sleep(300);

                AnsiConsole.MarkupLine("[italic]Deserialising file[/]");
                Thread.Sleep(300);
                
                Map = JNode.Deserialize<MapVO>()!;

                if (Map is null)
                { throw new JSONDeserialisationException(nameof(MapVO)); }
                
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
    
    static private int GetNodeCount(MapVO? _Map)
    {
        int ChosenNodeCount;

        do
        {
            AnsiConsole.Clear();
            PrintTitle();

            ChosenNodeCount = AnsiConsole.Prompt(
            new TextPrompt<int>($"[cyan1]There are [bold]{_Map!.NetworkNodes.Length}[/] node(s) in this map. " +
                                $"How many would you like to use?[/]")
                .DefaultValue<int>(_Map!.NetworkNodes.Length)
                .Validate((_Val) => (_Val <= _Map?.NetworkNodes.Length) && (_Val > 0))
            );
        } while (
            !AnsiConsole.Prompt(new ConfirmationPrompt($"[cyan1]You have chosen [bold]{ChosenNodeCount}[/] nodes to use. Is this amount correct?[/]"))
        );
        return ChosenNodeCount;
    }

    static private int GetMessageCount() {
        int ChosenMsgCount;

        do
        {
            AnsiConsole.Clear();
            PrintTitle();
            
            ChosenMsgCount = AnsiConsole.Prompt(
            new TextPrompt<int>($"[cyan1]How many messages would you like in the challenge? [bold][[10-{short.MaxValue}]][/][/]")
                .DefaultValue<int>(Challenge.NodeCount * 10)
                .Validate((_Val) => (_Val <= short.MaxValue) && (_Val > 10))
            );
        } while (
            !AnsiConsole.Prompt(new ConfirmationPrompt($"[cyan1]You have chosen [bold]{ChosenMsgCount}[/] messages to generate. Is this amount correct?[/]"))
        );

        return ChosenMsgCount;
    }

    static private void SelectMessageTypes() {

        if (!File.Exists(MessageTypesFile))
        {
            Exception EXC = new FileNotFoundException($"Cannot find {MessageTypesFile} File!");
            
            AnsiConsole.WriteException(EXC);
            throw EXC;
        }

        using StreamReader Reader = new(MessageTypesFile);
        List<MessageType>? AvailableTypes =
            JsonSerializer.Deserialize<List<MessageType>>(Reader.BaseStream,
            SourceGenerationContext.Default.ListMessageType);

        if (AvailableTypes is null)
        {
            Exception EXC = new JSONDeserialisationException(nameof(MapVO));
            
            AnsiConsole.WriteException(EXC);
            throw EXC;
        }

        if (AvailableTypes.Count < 1)
        {
            Exception EXC = new NoMessageTypesAvailableException(MessageTypesFile);
            
            AnsiConsole.WriteException(EXC);
            throw EXC;
        }

        List<MessageType> SelectedMessageTypes = [];

        do
        {
            SelectedMessageTypes = AnsiConsole.Prompt(new MultiSelectionPrompt<MessageType>()
                .Title("[cyan1]Please select what message types you'd like in this challenge.[/]")
                .Required(true)
                .AddChoices(AvailableTypes)
                .UseConverter(X => X.ToString())
                .InstructionsText("[grey](Press [blue]<space>[/] to toggle a message type, [green]<enter>[/] to accept)[/]")
            );

            if (SelectedMessageTypes.Count < 1)
            { AnsiConsole.MarkupLine("[red]You must select at least one message type[/]"); }
            
            
            
        } while (
            AnsiConsole.Prompt(new ConfirmationPrompt($"[cyan1] You've selected: [grey]{PrintSelectedOptions(SelectedMessageTypes)}[/]Are you happy with these choices?[/]"))            
            );
    }

    static private string PrintSelectedOptions(List<MessageType> _SelectedMessageTypes) {
        StringBuilder SB = new StringBuilder();
        
        _SelectedMessageTypes.ForEach(X => SB.Append($"\t{X.Name}\n"));

        return SB.ToString();
    }
}
