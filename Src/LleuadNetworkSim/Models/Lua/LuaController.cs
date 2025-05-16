using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

using Godot;

using LleuadNetworkSim.Models.Exceptions.Lua;
using LleuadNetworkSim.Models.Messaging;
using LleuadNetworkSim.Utils;

using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using MoonSharp.VsCodeDebugger;

using FileAccess = System.IO.FileAccess;
using Script = MoonSharp.Interpreter.Script;

namespace LleuadNetworkSim.Models.Lua;

public class LuaController
{
    static private Maybe<MoonSharpVsCodeDebugServer> Server { get; set; } = new MoonSharpVsCodeDebugServer();

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
    readonly private Dictionary<string, (Message Msg, int Port)> MessagesInProcess = [];
    
    private DynValue ProcessCoroutine = DynValue.Nil;
    
    //the number of available ports this node has 
    public int PortCount { get; set; }
    public Queue<(Message Msg, int Port)> Backlog = [];
    public Action<int, Message> ExtSendMessage { get; set; }
    public Action PullBacklog { get; set; }

    public int AutoYieldCounter { get; set; } = 60_000;
    
    public void LoadScript(LuaScript _LS, string _NodeID) {

        NodeID = _NodeID;
        
        LoadGlobals();

        DynValue? Loaded = _LS.PreLoaded 
                               ? Scrpt.DoString(_LS.FileData) 
                               : Scrpt.LoadFile(_LS.FileLoc);
        
        DynValue ProcessFunc = Scrpt.Globals.Get("Process");

        this.ProcessCoroutine = Scrpt.CreateCoroutine(ProcessFunc);
    }

    private void LoadGlobals() {
        Scrpt.Globals["Reg_Save"] = (Func<string, DynValue, bool>)Save;
        Scrpt.Globals["Reg_Load"] = (Func<string, DynValue>)(Load);
        Scrpt.Globals["Port_GetCount"] = (Func<int>)(() => PortCount);
        Scrpt.Globals["Backlog_Get"] = (Func<(Message Msg, int Port)?>)BacklogGetMessage;
        Scrpt.Globals["Backlog_GetCount"] = (Func<int>)(() => Backlog.Count);
        Scrpt.Globals["Msg_GetNewMessage"] = (Func<Message>)GetNewMessage;
        Scrpt.Globals["Msg_DirectToPort"] = (Action<int, string>)SendMessage;
        Scrpt.Globals["Msg_Send"] = (Action<int, Message>)SendMessage;
        Scrpt.Globals["Node_ID"] = NodeID;
    }

    private void RunTest() {
        
        if (ProcessCoroutine is not { Type: DataType.Function })
        { throw new ScriptMissingRequiredFuncException("N/A", "ProcessMessage"); }

        try
        {
            Message TestMessage = MessageGenerator.DebugMessage;
            
            DynValue Res = ProcessCoroutine.Function.Call(TestMessage);

            if (Res.Type != DataType.Number || Res.CastToNumber().ToInt() == -1)
            { throw new InvalidScriptFuncResultException(Res.Type, Res); }
        }
        catch (Exception Exc)
        { throw new ScriptTestFuncFailureException(Exc); }
    }

    public async Task Start(CancellationToken _CT) {

        if (Server.HasValue)
        { Server.Value.AttachToScript(Scrpt, NodeID); }

        await Task.Run(() => {
                           Stopwatch SW = new();
                           ProcessCoroutine.Coroutine.AutoYieldCounter = AutoYieldCounter;
                           
                           while (!_CT.IsCancellationRequested)
                           {
                               SW.Restart();

                               ProcessCoroutine.Coroutine.Resume();
                               
                               Thread.Sleep(Math.Clamp((1000 - SW.Elapsed.Milliseconds), 0, 500));

                               if (Backlog.Count == 0)
                               { PullBacklog(); }
                           }
                       },
                       _CT);
    }

    public void LoadDebugServer() {
        try
        { Server.Value.Start(); }
        catch (InvalidOperationException)
        { }
        catch (Exception Exc)
        {
            Debug.WriteLine($"Couldn't load debug server due to {Exc.Message}");
            throw;
        }
    }

    #region Passthrough
    /// <summary>
    /// Attempts to save a value to the data register with a given key.
    /// </summary>
    /// <param name="_Key">The key to save the value to.</param>
    /// <param name="_Val">The value to save.</param>
    /// <returns><c>true</c> if saved successfully, <c>false</c> otherwise.</returns>
    private bool Save(string _Key, DynValue _Val) 
        => DataRegister.TryAdd(_Key, _Val);

    /// <summary>
    /// Attempts to retrieve a value from the data register with a given key. Returns <see cref="DynValue.Nil"/>
    ///  if not found. 
    /// </summary>
    /// <param name="_Key">The key of the value to get.</param>
    /// <returns>The value if present, otherwise <see cref="DynValue.Nil"/>.</returns>
    private DynValue Load(string _Key)
        => DataRegister.TryGetValue(_Key, out DynValue? Val) ? Val : DynValue.Nil;

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
    private (Message Msg, int Port)? BacklogGetMessage() {

        if (Backlog.Count == 0)
        { return null; }

        (Message Msg, int Port) Msg = Backlog.Dequeue();
        
        MessagesInProcess.Add(Msg.Msg.ID, Msg);

        return Msg;
    }

    /// <summary>
    /// Allows the script to send the <see cref="Message"/> it's currently
    ///  processing to a specific port. 
    /// </summary>
    /// <param name="_Port">The port to send the <see cref="Message"/> through.</param>
    /// <param name="_ID">The ID of the message.</param>
    private void SendMessage(int _Port, string _ID) {
        if (!MessagesInProcess.Remove(_ID, out (Message Msg, int _) Msg))
        { return; }

        ExtSendMessage(_Port, Msg.Msg);
    }

    /// <summary>
    /// Allows the script to send it's own message down a specific port.
    /// </summary>
    /// <param name="_Port">The port to send the <see cref="Message"/> through.</param>
    /// <param name="_Msg">The <see cref="Message"/> to send.</param>
    private void SendMessage(int _Port, Message _Msg) {
        
        ExtSendMessage(_Port, _Msg);
    }
    #endregion
}
