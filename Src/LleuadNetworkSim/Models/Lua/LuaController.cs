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
using MoonSharp.VsCodeDebugger;

using FileAccess = System.IO.FileAccess;
using Script = MoonSharp.Interpreter.Script;

namespace LleuadNetworkSim.Models.Lua;

public class LuaController
{
    static private Maybe<MoonSharpVsCodeDebugServer> Server { get; set; } = Maybe<MoonSharpVsCodeDebugServer>.None;
    
    private Script Scrpt = new(CoreModules.Preset_SoftSandbox);
    private string NodeID = String.Empty;
    private Dictionary<string, DynValue> DataRegister = [];
    private Dictionary<string, string> ScriptFiles = [];
    private Dictionary<string, Message> MessagesInProcess = [];
    
    private DynValue ProcessFunc = DynValue.Nil;
    
    //the number of available ports this node has 
    public int PortCount { get; set; }
    public Queue<Message> Backlog = [];
    required public Action<int, Message> ExtSendMessage { get; init; }

    public int AutoYieldCounter { get; set; } = 60_000;
    
    public void LoadScript(LuaScript _LScript, string _NodeID) {

        //validate file
        
        Scrpt.DoFile(_LScript.FileLoc);
        LoadGlobals();
        
        ProcessFunc = Scrpt.Globals.Get("Process");
    }

    private void LoadGlobals() {
        Scrpt.Globals["Reg_Save"] = (Func<string, DynValue, bool>)Save;
        Scrpt.Globals["Reg_Load"] = (Func<string, DynValue>)Load;
        Scrpt.Globals["Msg_GetNewMessage"] = (Func<Message>)GetDefaultMessage;
        Scrpt.Globals["Port_GetCount"] = (Func<int>)(() => PortCount);
        Scrpt.Globals["Backlog_Get"] = (Func<Message?>)BacklogGetMessage;
        Scrpt.Globals["Backlog_Count"] = (Func<int>)(() => Backlog.Count);
        Scrpt.Globals["Msg_DirectToPort"] = (Action<int, string>)SendMessage;
        Scrpt.Globals["Msg_Send"] = (Action<int, Message>)SendMessage;
    }

    private void RunTest() {
        
        if (ProcessFunc is not { Type: DataType.Function })
        { throw new ScriptMissingRequiredFuncException("N/A", "ProcessMessage"); }

        try
        {
            Message TestMessage = MessageGenerator.DebugMessage;
            
            DynValue Res = ProcessFunc.Function.Call(TestMessage);

            if (Res.Type != DataType.Number || Res.CastToNumber().ToInt() == -1)
            { throw new InvalidScriptFuncResultException(Res.Type, Res); }
        }
        catch (Exception exc)
        { throw new ScriptTestFuncFailureException(exc); }
    }

    public async Task Start(CancellationToken _CT) {

        if (Server.HasValue)
        { Server.Value.AttachToScript(Scrpt, NodeID); }

        await Task.Run(() => {
                           Stopwatch SW = new();
                           ProcessFunc.Coroutine.AutoYieldCounter = AutoYieldCounter;
                           
                           while (!_CT.IsCancellationRequested)
                           {
                               SW.Restart();

                               ProcessFunc.Coroutine.Resume();
                               
                               Thread.Sleep(Math.Clamp((500 - SW.Elapsed.Milliseconds), 0, 500));
                           }
                       },
                       _CT);
    }

    public void LoadDebugServer() {
        Server = new MoonSharpVsCodeDebugServer();
        Server.Value.Start();
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
        
        if (ScriptFiles.TryGetValue(_FileName, out string? value))
        { FileLoc = value; }
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
        catch (Exception e)
        { return DynValue.NewString($"Failed to write to file due to [{e.Message}]"); }
    }

    private Message GetDefaultMessage()
        => MessageGenerator.DefaultMessage();

    private Message? BacklogGetMessage() {

        if (Backlog.Count == 0)
        { return null; }

        Message Msg = Backlog.Dequeue();
        
        MessagesInProcess.Add(Msg.ID, Msg);

        return Msg;
    }

    private void SendMessage(int _Port, string _ID) {
        if (!MessagesInProcess.Remove(_ID, out Message? Msg))
        { return; }

        ExtSendMessage(_Port, Msg);
    }

    private void SendMessage(int _Port, Message _Msg)
        => ExtSendMessage(_Port, _Msg);
    #endregion
}
