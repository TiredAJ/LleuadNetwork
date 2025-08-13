using System.Collections;
using System.Text.Json;
using System.Text.Json.Serialization;

using CSharpFunctionalExtensions;

namespace Common.Json;

static public class JSONHelper
{
    //static public Maybe<JsonSerializerContext> CustomContext = Maybe<JsonSerializerContext>.None;
    
    static public Result<T> Deserialise<T>(Stream _JStream, 
        Maybe<JsonSerializerContext> _CustomContext = default) {

        JsonSerializerContext JSCtx = VO_SrcGenCtx.Default;
        
        Type Info = typeof(T);

        if (Info.GetInterfaces().Contains(typeof(IEnumerable)))
        { JSCtx = Collection_SrcGenCtx.Default; }

        if (_CustomContext.HasValue)
        { JSCtx = _CustomContext.Value; }
        
        T? Res = (T?)JsonSerializer.Deserialize(_JStream, Info, JSCtx);

        _JStream.Dispose();
        
        return Res is null ?
                   Result.Failure<T>($"Couldn't deserialse {nameof(T)}") :
                   Result.Success<T>(Res);
    }
    
    static public Result<T> Deserialise<T>(StreamReader _JStream, 
        Maybe<JsonSerializerContext> _CustomContext = default)
        => Deserialise<T>(_JStream.BaseStream, _CustomContext);
    
    static public Result<T> DeserialiseFromFile<T>(string _Path, 
        Maybe<JsonSerializerContext> _CustomContext = default) 
        => Deserialise<T>(new StreamReader(_Path), _CustomContext);

    static public void Serialise<T>(Stream _JStream, T _Value, 
        Maybe<JsonSerializerContext> _CustomContext = default) {
        JsonSerializerContext JSCtx = VO_SrcGenCtx.Default;

        Type Info = typeof(T);

        if (Info.GetInterfaces().Contains(typeof(IEnumerable)))
        { JSCtx = Collection_SrcGenCtx.Default; }

        if (_CustomContext.HasValue)
        { JSCtx = _CustomContext.Value; }
        
        JsonSerializer.Serialize(_JStream, _Value, Info, JSCtx);
        
        _JStream.Flush();
        _JStream.Dispose();
    }
    
    static public void Serialise<T>(StreamWriter _JStream, T _Value, 
        Maybe<JsonSerializerContext> _CustomContext = default) {
        Serialise(_JStream.BaseStream, _Value, _CustomContext);
    }
    
    static public void SerialiseToFile<T>(string _Path, T _Value, 
        Maybe<JsonSerializerContext> _CustomContext = default) {
        Serialise(new StreamWriter(_Path), _Value, _CustomContext);
    }
}
