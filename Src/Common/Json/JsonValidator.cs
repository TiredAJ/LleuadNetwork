using System.Text.Json.Nodes;

using Common.Json.Exceptions;
using Common.Utils;

using CSharpFunctionalExtensions;

using NJsonSchema;
using NJsonSchema.Validation;

namespace Common.Json;

static public class JsonValidator
{
    static public Maybe<Exception> ValidateJson<T>(string _JDataPath, out JsonNode? _JNode) where T : class {

        _JNode = false;
        
        using StreamReader Reader = new(_JDataPath);

        JsonSchema Schema = JsonSchema.FromType<T>();

        string StrJData = Reader.ReadToEnd();

        ICollection<ValidationError> Errors = [];

        try
        { Errors = Schema.Validate(StrJData); }
        catch (Exception Exc)
        { return Exc; }

        if (Errors.Count != 0)
        { return new JsonSchemaValidationException(_JDataPath, Errors); }

        JsonNode? JData = JsonNode.Parse(StrJData, new JsonNodeOptions());

        if (JData is null)
        { return new NotImplementedException(); }

        //CheckSchemaVersion(JData, _Caller);
        
        _JNode = JData;
        
        return Maybe<Exception>.None;
    }

    static private Maybe<Exception> CheckSchemaVersion(JsonNode _JData) {

        if (_JData["_ObjVersion"]!.ToInt32() != MapVO.SchemaVersion)
        {
            return new JsonSchemaVersionException("MapVO", MapVO.SchemaVersion,
                                               _JData["_ObjVersion"]!.ToInt32());
        }
        
        return _JData["NetworkNodes"]!.AsArray().Any(X => X!["_ObjVersion"]!.ToInt32() != NetworkNodeVO.SchemaVersion) 
                   ? new JsonSchemaVersionException("MapVO", NetworkNodeVO.SchemaVersion) 
                   : Maybe<Exception>.None;
    }
}
