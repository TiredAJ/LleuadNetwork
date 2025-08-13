using System.Text.Json.Nodes;

using Common.Json.Exceptions;
using Common.Utils;

using CSharpFunctionalExtensions;

using NJsonSchema;
using NJsonSchema.Validation;

namespace Common.Json;

//TODO: TEST - all of these
static public class JsonValidator
{
    static public Maybe<Exception> ValidateJson<T>(string _JDataPath, out Stream? _JStream) where T : IBaseVO {
        _JStream = null;
        
        StreamReader Reader = new(_JDataPath);

        JsonSchema Schema = JsonSchema.FromType<T>();

        string StrJData = Reader.ReadToEnd();

        ICollection<ValidationError> Errors;
        
        try
        { Errors = Schema.Validate(StrJData); }
        catch (Exception Exc)
        { return Exc; }

        if (Errors.Count != 0)
        { return new JsonSchemaValidationException(_JDataPath, Errors); }

        JsonNode? JData = JsonNode.Parse(StrJData, new JsonNodeOptions());

        if (JData is null)
        { return new NotImplementedException(); }

        Maybe<Exception> EXC = CheckSchemaVersion<T>(JData);

        if (EXC.HasValue)
        { return EXC; }

        _JStream = Reader.BaseStream;
        _JStream.Position = 0;
        
        return Maybe<Exception>.None;
    }

    static private Maybe<Exception> CheckSchemaVersion<T>(JsonNode _JData) where T : IBaseVO {

        if (_JData["__Version"] is null)
        { return Maybe<Exception>.None; }
        
        if (_JData["__Version"]?.GetValue<int>() != T.GetVersion())
        {
            return new JsonSchemaVersionException(nameof(T), T.GetVersion(),
                                               _JData["__Version"]?.GetValue<int>());
        }
     
        return Maybe<Exception>.None;
    }
}
