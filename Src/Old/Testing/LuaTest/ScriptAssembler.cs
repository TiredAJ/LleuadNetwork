using System.Text;

namespace LuaTest;

static public class ScriptAssembler
{
    static private SortedList<int, string> Scripts = [];

    static public void AddScript(int _Priority, string _FilePath) {

        try
        {
            string FullPath = Path.GetFullPath(_FilePath);
            Scripts.Add(_Priority, FullPath);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    static public Task AssembleScript(string? _Destination = null) {
        
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
                        });
    }
}
