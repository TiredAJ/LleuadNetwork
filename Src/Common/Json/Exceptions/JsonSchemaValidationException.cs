using MoreLinq;

using NJsonSchema.Validation;

namespace Common.Json.Exceptions;

public class JsonSchemaValidationException(string _Path, ICollection<ValidationError> _Errors) : Exception(DebugMessage(_Path, _Errors))
{
    static private string DebugMessage(string _Path, ICollection<ValidationError> _Errors)
        => $"JSON schema validation failed for [{_Path}] with the following errors: [{_Errors.ToDelimitedString(",")}";
}
