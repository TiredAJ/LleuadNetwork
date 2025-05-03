using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Nodes;

using Godot;

using LleuadNetworkSim.Scripts.Exceptions;

using NJsonSchema;
using NJsonSchema.Validation;

namespace LleuadNetworkSim.Utils.Validators.Json;

public class JsonValidator
{
    static public JsonNode ValidateJson<T>(string _JDataPath, Node _Caller) where T : class {

        using StreamReader Reader = new(_JDataPath);

        JsonSchema Schema = JsonSchema.FromType<T>();

        string JData = Reader.ReadToEnd();
        
        ICollection<ValidationError> Errors = [];

        try
        { Errors = Schema.Validate(JData); }
        catch (Exception Exc)
        { ExceptionPopupWrapper.Throw(_Caller, Exc); }
        
        if (Errors.Count != 0)
        { ExceptionPopupWrapper.Throw(_Caller, new JsonSchemaValidationException(_JDataPath, Errors)); }

        try
        { return JsonNode.Parse(JData, new JsonNodeOptions())!; }
        catch (Exception Exc)
        { ExceptionPopupWrapper.Throw(_Caller, Exc); }
        
        //shouldn't get to this point, but IntelliSense or whatever doesn't understand that
        // `ExceptionPopupWrapper.Throw` _throws_
        return null!;
    }
}
