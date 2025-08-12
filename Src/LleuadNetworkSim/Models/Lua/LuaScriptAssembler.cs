using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Common.Utils;

using Godot;
using Godot.Logging;

using LleuadNetworkSim.Models.Exceptions.Lua;

using MoreLinq;

namespace LleuadNetworkSim.Models.Lua;

public partial class LuaScriptAssembler
{
    static readonly private SortedList<int, string> Scripts = [];
    static private int Priority;

    #region Regex
    [GeneratedRegex("(?=.*?(require))", RegexOptions.IgnoreCase, "en-gb")]
    static private partial Regex MessageRequireRemover();
    #endregion

    static public void AddScript(int _Priority, string _FilePath) {

        if (_Priority == Priority)
        { _Priority++; }

        try
        {
            string FullPath = Path.GetFullPath(_FilePath);

            while (Scripts.ContainsKey(_Priority))
            { _Priority++; }

            Scripts.Add(_Priority, FullPath);

            if (Priority <= _Priority)
            { Priority = _Priority; }
        }
        catch (Exception EXC)
        {
            Console.WriteLine(EXC);
            throw;
        }
    }

    static public void AddScript(string _FilePath) {

        try
        {
            string FullPath = Path.GetFullPath(_FilePath);

            if (!File.Exists(_FilePath))
            { throw new ScriptDoesntExistException(_FilePath); }

            using StreamReader Reader = new(_FilePath);

            string Line = Reader.ReadLine() ?? "";

            string StrPriority = Line.ToLower().Trim(' ', '-', 'p').Or($"{Priority}");

            int TempPriority = Convert.ToInt32(StrPriority);

            AddScript(TempPriority, FullPath);
        }
        catch (Exception EXC)
        {
            Console.WriteLine(EXC);
            throw;
        }
    }

    //TODO: TEST - this
    static public Task<LuaScript> AssembleScript(string? _Destination = null, bool _PreLoad = false) {

        return Task.Run(async () => {
                            _Destination ??= ProjectSettings.GlobalizePath($"user://generated/{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}.lua");

                            if (File.Exists(_Destination))
                            { File.Delete(_Destination); }

                            if (!Directory.Exists(Path.GetDirectoryName(_Destination)))
                            { Directory.CreateDirectory(Path.GetDirectoryName(_Destination)!); }

                            File.Create(_Destination).Close();

                            await using StreamWriter Writer = new(_Destination);

                            LuaScript LS = new(){FileLoc = _Destination, PreLoaded = _PreLoad};

                            foreach (KeyValuePair<int, string> Scrpt in Scripts)
                            {
                                using StreamReader Reader = new(Scrpt.Value);

                                try
                                {
                                    StringBuilder Builder = new StringBuilder();
                                    Builder.Append(LS.FileData);
                                    while (!Reader.EndOfStream)
                                    {
                                        string? ReadData = await Reader.ReadLineAsync();

                                        if (ReadData is (null or "") || ReadData.Contains("require("))
                                        { continue; }

                                        if (_PreLoad)
                                        {
                                            Builder.Append(ReadData);
                                            Builder.Append('\n');
                                        }

                                        await Writer.WriteLineAsync(ReadData);
                                    }
                                    LS.FileData = Builder.ToString();
                                }
                                catch (Exception Exc)
                                {
                                    GodotLogger.LogError(Exc);
                                    throw;
                                }

                                Reader.Close();
                            }

                            Writer.Close();

                            Debug.WriteLine($"saved to [{_Destination}]");

                            return LS;
                        });
    }

    //TODO: TEST - this
    static private void CleanScript(string _ScriptLoc) {
        using StreamReader Reader = new(_ScriptLoc);

        string Data = Reader.ReadToEnd();

        Reader.Close();

        MessageRequireRemover()
            .Matches(Data)
            .ForEach(M => {
                                   int ReqIndex = M.Index;

                                   int FinalReq = Data.IndexOf(')', ReqIndex) + 2;

                                   Data = string.Concat(Data.AsSpan()[..ReqIndex], "", Data.AsSpan()[FinalReq..]);
                               });

        using StreamWriter Writer = new(_ScriptLoc);

        Writer.Write(Data);

        Writer.Close();
    }
}
