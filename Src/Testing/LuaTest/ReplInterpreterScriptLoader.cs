using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;

namespace LuaTest;

//stolen from https://github.com/moonsharp-devs/moonsharp/blob/master/src/MoonSharp.Interpreter/REPL/ReplInterpreterScriptLoader.cs

/// <summary>
/// A script loader loading scripts directly from the file system (does not go through platform object)
/// AND starts with module paths taken from environment variables (again, not going through the platform object).
/// 
/// The paths are preconstructed using :
///		* The MOONSHARP_PATH environment variable if it exists
///		* The LUA_PATH_5_2 environment variable if MOONSHARP_PATH does not exists
///		* The LUA_PATH environment variable if LUA_PATH_5_2 and MOONSHARP_PATH do not exists
///		* The "?;?.lua" path if all the above fail
///		
/// Also, everytime a module is require(d), the "LUA_PATH" global variable is checked. If it exists, those paths
/// will be used to load the module instead of the global ones.
/// </summary>
public class ReplInterpreterScriptLoader : FileSystemScriptLoader
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ReplInterpreterScriptLoader"/> class.
	/// </summary>
	public ReplInterpreterScriptLoader()
	{
		string? env = Environment.GetEnvironmentVariable("MOONSHARP_PATH");
		if (!string.IsNullOrEmpty(env)) ModulePaths = UnpackStringPaths(env);

		if (ModulePaths == null)
		{
			env = Environment.GetEnvironmentVariable("LUA_PATH_5_2");
			if (!string.IsNullOrEmpty(env)) ModulePaths = UnpackStringPaths(env);
		}

		if (ModulePaths == null)
		{
			env = Environment.GetEnvironmentVariable("LUA_PATH");
			if (!string.IsNullOrEmpty(env)) ModulePaths = UnpackStringPaths(env);
		}

        ModulePaths ??= [
            "/usr/lib/lua/5.4/",
            "/home/aj/.luarocks/lib/luarocks/rocks-5.4",
            "/home/aj/.luarocks/share/lua/5.4/cjson/util.lua",
            "/home/aj/.luarocks/share/lua/5.4/json2lua.lua",
            "/home/aj/.luarocks/share/lua/5.4/lua2json.lua"
        ];
		ModulePaths ??= UnpackStringPaths("?;?.lua");
	}

	/// <summary>
	/// Resolves the name of a module to a filename (which will later be passed to OpenScriptFile).
	/// The resolution happens first on paths included in the LUA_PATH global variable, and -
	/// if the variable does not exist - by consulting the
	/// ScriptOptions.ModulesPaths array. Override to provide a different behaviour.
	/// </summary>
	/// <param name="_ModuleName">The _ModuleName.</param>
	/// <param name="_GlobalContext">The global context.</param>
	public override string ResolveModuleName(string _ModuleName, Table _GlobalContext)
	{
		DynValue s = _GlobalContext.RawGet("LUA_PATH");
        
        return s is {Type: DataType.String} 
                   ? ResolveModuleName(_ModuleName, UnpackStringPaths(s.String)) 
                   : base.ResolveModuleName(_ModuleName, _GlobalContext);
    }
}
