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
    static public Maybe<Exception> ValidateJson<T>(string _JDataPath, out Stream? _JStream) where T : class {
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

        Maybe<Exception> EXC = CheckSchemaVersion(JData);

        if (EXC.HasValue)
        { return EXC; }

        _JStream = Reader.BaseStream;
        _JStream.Position = 0;
        
        return Maybe<Exception>.None;
    }

    static private Maybe<Exception> CheckSchemaVersion(JsonNode _JData) {

        if (_JData["__Version"]?.GetValue<int>() != MapVO.SchemaVersion)
        {
            return new JsonSchemaVersionException("MapVO", MapVO.SchemaVersion,
                                               _JData["__Version"]?.GetValue<int>());
        }
        
        return _JData["NetworkNodes"]!.AsArray().Any(X => X!["__Version"]?.GetValue<int>() != NetworkNodeVO.SchemaVersion) 
                   ? new JsonSchemaVersionException("MapVO", NetworkNodeVO.SchemaVersion) 
                   : Maybe<Exception>.None;
    }
}
