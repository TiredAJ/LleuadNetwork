using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using CSharpFunctionalExtensions;

using Godot;

using LleuadNetworkSim.Models.Exceptions.Lua;

using MoonSharp.Interpreter;
using MoonSharp.VsCodeDebugger;

using FileAccess = System.IO.FileAccess;
using Script = MoonSharp.Interpreter.Script;

namespace LleuadNetworkSim.Models.Lua;

public class LuaController
{
    private Script Scrpt = new(CoreModules.Preset_SoftSandbox);
    private string NodeID = String.Empty;
    static private Maybe<MoonSharpVsCodeDebugServer> Server = Maybe<MoonSharpVsCodeDebugServer>.None;
    private Dictionary<string, DynValue> DataRegister = [];
    private Dictionary<string, string> ScriptFiles = [];
    
    public void LoadScript(LuaScript _LScript, string _NodeID) {

        //validate file
        
        Scrpt.DoFile(_LScript.FileLoc);
        LoadGlobals();
    }

    private void LoadGlobals() {
        Scrpt.Globals["Reg_Save"] = (Func<string, DynValue, bool>)Save;
        Scrpt.Globals["Reg_Load"] = (Func<string, DynValue>)Load;
    }

    public int Execute(Messaging.Message _Msg) {

        if (Server.HasValue)
        { Server.Value.AttachToScript(Scrpt, NodeID); }

        DynValue? ProcessFunc = Scrpt.Globals.Get("ProcessMessage");
        
        if (ProcessFunc is not { Type: DataType.Function })
        { throw new ScriptMissingRequiredFuncException("N/A", "ProcessMessage"); }

        try
        {
            DynValue Res = ProcessFunc.Function.Call();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        return -1;
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
    #endregion
}
