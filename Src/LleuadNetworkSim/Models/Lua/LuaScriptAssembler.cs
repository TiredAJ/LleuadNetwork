using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Godot;

using LleuadNetworkSim.Models.Exceptions;
using LleuadNetworkSim.Models.Exceptions.Lua;

using static System.Text.RegularExpressions.Regex;

namespace LleuadNetworkSim.Models.Lua;

public partial class LuaScriptAssembler
{
    static private SortedList<int, string> Scripts = [];
    static private int Priority = 0;

    #region Regex
    [GeneratedRegex("(?=.*?(require))(?=.*?(Message))", RegexOptions.IgnoreCase, "en-gb")]
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
        catch (Exception e)
        {
            Console.WriteLine(e);
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
            
            string StrPriority = Line.Trim(' ', '-') ?? $"{LuaScriptAssembler.Priority}";
            
            int TempPriority = Convert.ToInt32(StrPriority);

            AddScript(TempPriority, FullPath);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    static public Task<LuaScript> AssembleScript(string? _Destination = null) {
        
        return Task.Run(async () => {
                            _Destination ??= Path.GetRandomFileName();

                            if (File.Exists(_Destination))
                            { File.Delete(_Destination); }
                            
                            await using StreamWriter Writer = new(File.Create(_Destination));
                            
                            foreach (KeyValuePair<int, string> Scrpt in Scripts)
                            {
                                CleanScript(Scrpt.Value);
                                
                                using StreamReader Reader = new(Scrpt.Value);
                                Span<char> Spn = new(new char[1024], 0, 1024);

                                while (!Reader.EndOfStream)
                                {
                                    Reader.ReadBlock(Spn);

                                    Spn = Spn.TrimEnd('\0');
                                    
                                    Writer.WriteLine(Spn);
                                }
                                
                                Reader.Close();
                            }
                            
                            Writer.Close();
                            
                            return new LuaScript(){FileLoc = _Destination};
                        });
    }

    static private void CleanScript(string _ScriptLoc) {
        using StreamReader Reader = new(_ScriptLoc);
        using StreamWriter Writer = new(_ScriptLoc);

        string Data = Reader.ReadToEnd();

        Data = MessageRequireRemover().Replace(Data, "");
        
        Writer.Write(Data);
    }
}
