using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

using Godot;
using Godot.DependencyInjection.Attributes;
using Godot.Logging;

using LleuadNetworkSim.Models.Exceptions.Lua;
using LleuadNetworkSim.Models.Messaging;
using LleuadNetworkSim.Models.Repo;
using LleuadNetworkSim.Models.Repo.Entities;
using LleuadNetworkSim.Models.Validation;
using LleuadNetworkSim.Utils;

using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using MoonSharp.VsCodeDebugger;

using FileAccess = System.IO.FileAccess;
using Script = MoonSharp.Interpreter.Script;

namespace LleuadNetworkSim.Models.Lua;

public class LuaController
{
    readonly private Script Scrpt = new(/*CoreModules.Preset_SoftSandbox*/) {
        Options = {
            ScriptLoader = new FileSystemScriptLoader() {
                IgnoreLuaPathGlobal = false,
                ModulePaths = [
                    "/usr/lib/lua/5.4/",
                    "/home/aj/.luarocks/lib/luarocks/rocks-5.4"
                ]
            }
        }
    };
    
    private string NodeID = string.Empty;
    readonly private Dictionary<string, DynValue> DataRegister = [];
    readonly private Dictionary<string, string> ScriptFiles = [];
    readonly private Dictionary<string, Message> MessagesInProcess = [];
    
    private DynValue ProcessFunc = DynValue.Nil;
    
    //the number of available ports the parent network node has 
    public int PortCount { get; set; }
    public List<Message> Backlog = [];
    public Action<int, Message> ExtSendMessage { get; set; } = (_, _) => {};
    public Action PullBacklog { get; set; } = () => { };

    public IDBWrapper DB = null!;
    public ChallengeRecord? Challenge;

    public void LoadScript(LuaScript _LS, string _NodeID) {

        NodeID = _NodeID;
        
        LoadGlobals();

        if (_LS.PreLoaded)
        { Scrpt.DoString(_LS.FileData); }
        else
        { Scrpt.LoadFile(_LS.FileLoc); }

        DynValue TempProcess = Scrpt.Globals.Get("Process");
        
        if (TempProcess is not { Type: DataType.Function })
        { throw new ScriptMissingRequiredFuncException(Path.GetFileName(_LS.FileLoc), "ProcessMessage"); }
        
        this.ProcessFunc = TempProcess;
    }

    private void LoadGlobals() {
        Scrpt.Globals["Reg_Save"] = (Func<string, DynValue, bool>)Save;
        Scrpt.Globals["Reg_Load"] = (Func<string, DynValue>)(Load);
        Scrpt.Globals["Port_GetCount"] = (Func<int>)(() => PortCount);
        Scrpt.Globals["Backlog_Get"] = (Func<Message?>)BacklogGetMessage;
        Scrpt.Globals["Backlog_Return"] = (Func<Message, string>)ReturnToBacklog;
        Scrpt.Globals["Backlog_GetCount"] = (Func<int>)(() => Backlog.Count);
        Scrpt.Globals["Msg_GetNewMessage"] = (Func<Message>)GetNewMessage;
        Scrpt.Globals["Msg_DirectToPort"] = (Action<int, string>)SendMessage;
        Scrpt.Globals["Msg_Send"] = (Action<int, Message>)SendMessage;
        Scrpt.Globals["Node_ID"] = NodeID;
        Scrpt.Globals["print"] = (Action<string>)Log;
        Scrpt.Globals["Log"] = (Action<string, string>)DBLog;
    }

    private void RunTest() {
        try
        {
            Message TestMessage = MessageGenerator.DebugMessage;
            
            DynValue Res = ProcessFunc.Function.Call(TestMessage);

            if (Res.Type != DataType.Number || Res.CastToNumber().ToInt() == -1)
            { throw new InvalidScriptFuncResultException(Res.Type, Res); }
        }
        catch (Exception Exc)
        { throw new ScriptTestFuncFailureException(Exc); }
    }

    public async Task Start(CancellationToken _CT) {

        Closure Func = ProcessFunc.Function;

        Challenge = DB.ChallengeColl()
                      .FindById(G_ChallengeID);
        
        await Task.Run(() => {
                           Stopwatch SW = new();

                           bool FirstLoad = true;
                           
                           while (!_CT.IsCancellationRequested)
                           {
                               SW.Restart();

                               try
                               {
                                   Func.Call(DynValue.Nil, FirstLoad);
                               }
                               catch (ScriptRuntimeException EXC)
                               {
                                   GodotLogger.LogError($"{EXC.Message} - {EXC.DecoratedMessage} - {EXC.Data}");
                                   break;
                               }

                               FirstLoad = false;

                               Task.Delay(Math.Clamp((1000 - SW.Elapsed.Milliseconds), 0, 500), _CT);

                               if (Backlog.Count == 0)
                               { PullBacklog(); }
                           }
                           
                           GodotLogger.LogWarning("LuaController Finished!");
                       },
                       _CT);
    }

    //public void LoadDebugServer() {
    //    try
    //    { Server.Value.Start(); }
    //    catch (InvalidOperationException)
    //    { }
    //    catch (Exception Exc)
    //    {
    //        Debug.WriteLine($"Couldn't load debug server due to {Exc.Message}");
    //    }
    //}

    #region Passthrough
    /// <summary>
    /// Attempts to save a value to the data register with a given key.
    /// </summary>
    /// <param name="_Key">The key to save the value to.</param>
    /// <param name="_Val">The value to save.</param>
    /// <returns><c>true</c> if saved successfully, <c>false</c> otherwise.</returns>
    private bool Save(string _Key, DynValue _Val) {
        if (!DataRegister.ContainsKey(_Key))
        { return DataRegister.TryAdd(_Key, _Val); }

        DataRegister[_Key] = _Val;
        return true;
    }

    /// <summary>
    /// Attempts to retrieve a value from the data register with a given key. Returns <see cref="DynValue.Nil"/>
    ///  if not found. 
    /// </summary>
    /// <param name="_Key">The key of the value to get.</param>
    /// <returns>The value if present, otherwise <see cref="DynValue.Nil"/>.</returns>
    private DynValue Load(string _Key) {
        DataRegister.TryGetValue(_Key, out DynValue? Val);

        return Val ?? DynValue.Nil;
    }

    /// <summary>
    /// Attempts to write a string to a file in the game's user's folder.
    /// </summary>
    /// <param name="_FileName">The name of the file (not path) to write to.</param>
    /// <param name="_Data">The data to write.</param>
    /// <returns>True if it successfully wrote, otherwise it returns a string containing an error message.</returns>
    private DynValue WriteToFile(string _FileName, string _Data) {

        string FileLoc;
        
        if (ScriptFiles.TryGetValue(_FileName, out string? Value))
        { FileLoc = Value; }
        else
        {
            FileLoc = ProjectSettings.GlobalizePath("user://ScriptFiles/");

            if (!Directory.Exists(FileLoc))
            { Directory.CreateDirectory(FileLoc); }

            FileLoc = $"{FileLoc}/{Path.GetFileName(_FileName)}.txt";
            
            if (!File.Exists(FileLoc))
            { File.Create(FileLoc); }
        }

        try
        {
            using StreamWriter Writer = new(FileLoc, Encoding.UTF8, new FileStreamOptions(){Access = FileAccess.ReadWrite});
        
            Writer.Write(_Data);
            
            return DynValue.True;
        }
        catch (Exception Exc)
        { return DynValue.NewString($"Failed to write to file due to [{Exc.Message}]"); }
    }

    /// <summary>
    /// Allows the script to obtain a blank <see cref="Message"/>
    /// </summary>
    /// <returns>A <see cref="Message"/> with default values</returns>
    static private Message GetNewMessage()
        => MessageGenerator.DefaultMessage();

    /// <summary>
    /// Allows the script to retrieve a <see cref="Message"/> from the backlog
    /// </summary>
    /// <returns>A <see cref="Message"/> if one is available, null otherwise</returns>
    private Message? BacklogGetMessage() {

        if (Backlog.Count == 0)
        { return null; }

        Message Msg = Backlog[0];
        Backlog.RemoveAt(0);
        
        MessagesInProcess.Add(Msg.ID, Msg);

        return Msg;
    }

    /// <summary>
    /// Allows the script to send the <see cref="Message"/> it's currently
    ///  processing to a specific port. 
    /// </summary>
    /// <param name="_Port">The port to send the <see cref="Message"/> through.</param>
    /// <param name="_ID">The ID of the message.</param>
    private void SendMessage(int _Port, string _ID) {
        if (!MessagesInProcess.Remove(_ID, out Message? Msg))
        { return; }

        ExtSendMessage(_Port - 1, Msg);
    }

    /// <summary>
    /// Allows the script to send it's own message down a specific port.
    /// </summary>
    /// <param name="_Port">The port to send the <see cref="Message"/> through.</param>
    /// <param name="_Msg">The <see cref="Message"/> to send.</param>
    private void SendMessage(int _Port, Message _Msg) {
        ExtSendMessage(_Port, _Msg);
    }

    /// <summary>
    /// Allows the script to log information.
    /// </summary>
    /// <param name="_Data">Loggable data.</param>
    private void Log(string _Data)
        => GodotLogger.LogInfo($"[{NodeID}]: {_Data}");

    /// <summary>
    /// If a packet can't be processed at the moment, it can be put to the back of the backlog for now
    /// </summary>
    /// <param name="_Msg"></param>
    private string ReturnToBacklog(Message _Msg) {
        if (!MessagesInProcess.TryGetValue(_Msg.ID, out Message? Msg))
        { return "Does not exist"; }

        if (MessageValidator.MessageValid(Msg))
        //if (MessageValidator.MessageValid(Msg, _Msg))
        {
            Backlog.Add(Msg);
            MessagesInProcess.Remove(_Msg.ID);
            return "Backlog'd";
        }

        return "Message failed Validation";
    }

    private void DBLog(string _Type, string _Message) {
        Console.WriteLine($"{NodeID} writing to db");

        try
        {
            DB.LPRColl()
              .Insert(new LuaProcessRecord { NodeID = NodeID, Action = _Type, Information = _Message, Challenge = Challenge});
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    #endregion
}
