using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using LleuadNetworkSim.Models.Exceptions;
using LleuadNetworkSim.Models.Exceptions.Lua;

namespace LleuadNetworkSim.Models.Lua;

public class LuaScriptAssembler
{
    static private SortedList<int, string> Scripts = [];
    static private int Priority = 0;
    
    static public void AddScript(int _Priority, string _FilePath) {

        if (_Priority == Priority)
        { _Priority++; }
        
        try
        {
            string FullPath = Path.GetFullPath(_FilePath);

            while (Scripts.ContainsKey(_Priority))
            { _Priority++; }
            
            Scripts.Add(_Priority, FullPath);
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

            string StrPriority = Reader.ReadLine()?.Trim(' ', '-') ?? $"{LuaScriptAssembler.Priority}";
            
            int TempPriority = Convert.ToInt32(StrPriority);

            Scripts.Add(TempPriority, FullPath);
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
                                using StreamReader Reader = new(Scrpt.Value);
                                Memory<char> Mem = new(new char[1024], 0, 1024);

                                while (!Reader.EndOfStream)
                                {
                                    await Reader.ReadBlockAsync(Mem);

                                    Mem = Mem.TrimEnd('\0');
                                    
                                    await Writer.WriteLineAsync(Mem);
                                }
                                
                                Reader.Close();
                            }
                            
                            Writer.Close();
                            
                            return new LuaScript(){FileLoc = _Destination};
                        });
    }
}
