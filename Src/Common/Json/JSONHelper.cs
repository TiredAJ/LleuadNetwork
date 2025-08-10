using System.Collections;
using System.Text.Json;
using System.Text.Json.Serialization;

using CSharpFunctionalExtensions;

namespace Common.Json;

static public class JSONHelper
{
    static public Result<T> Deserialise<T>(Stream _JStream) {

        JsonSerializerContext JSCtx = VO_SrcGenCtx.Default;

        Type Info = typeof(T);

        if (Info.GetInterfaces().Contains(typeof(IEnumerable)))
        { JSCtx = Collection_SrcGenCtx.Default; }

        T? Res = (T?)JsonSerializer.Deserialize(_JStream, Info, JSCtx);

        _JStream.Dispose();
        
        return Res is null ?
                   Result.Failure<T>($"Couldn't deserialse {nameof(T)}") :
                   Result.Success<T>(Res);
    }
    
    static public Result<T> Deserialise<T>(StreamReader _JStream)
        => Deserialise<T>(_JStream.BaseStream);
    
    static public Result<T> DeserialiseFromFile<T>(string _Path) 
        => Deserialise<T>(new StreamReader(_Path));

    static public void Serialise<T>(Stream _JStream, T _Value) {
        JsonSerializerContext JSCtx = VO_SrcGenCtx.Default;

        Type Info = typeof(T);

        if (Info.GetInterfaces().Contains(typeof(IEnumerable)))
        { JSCtx = Collection_SrcGenCtx.Default; }
        
        JsonSerializer.Serialize(_JStream, _Value, Info, JSCtx);
        
        _JStream.Flush();
        _JStream.Dispose();
    }
    
    static public void Serialise<T>(StreamWriter _JStream, T _Value) {
        Serialise(_JStream.BaseStream, _Value);
    }
    
    static public void SerialiseToFile<T>(string _Path, T _Value) {
        Serialise(new StreamWriter(_Path), _Value);
    }
}
