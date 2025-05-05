using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        string StrJData = Reader.ReadToEnd();
        
        ICollection<ValidationError> Errors = [];

        try
        { Errors = Schema.Validate(StrJData); }
        catch (Exception Exc)
        { ExceptionPopupWrapper.Throw(_Caller, Exc); }
        
        if (Errors.Count != 0)
        { ExceptionPopupWrapper.Throw(_Caller, new JsonSchemaValidationException(_JDataPath, Errors)); }

        JsonNode? JData = JsonNode.Parse(StrJData, new JsonNodeOptions());

        if (JData is null)
        { throw new NotImplementedException(); }

        //CheckSchemaVersion(JData, _Caller);        
        
        try
        { return JData; }
        catch (Exception Exc)
        { ExceptionPopupWrapper.Throw(_Caller, Exc); }
        
        //shouldn't get to this point, but IntelliSense or whatever doesn't understand that
        // `ExceptionPopupWrapper.Throw` _throws_
        return null!;
    }

    static private void CheckSchemaVersion(JsonNode _JData, Node _Caller) {

        Exception? Exc = null;
        
        if (_JData["_ObjVersion"]!.ToInt32() != CollectionNodeVO.SchemaVersion)
        { 
            Exc = new JsonSchemaVersionException("CollectionNodeVO", CollectionNodeVO.SchemaVersion, 
                                               _JData["_ObjVersion"]!.ToInt32()); 
        }
        else if (_JData["NetworkNodes"]!.AsArray().Any(X => X!["_ObjVersion"].ToInt32() != NetworkNodeVO.SchemaVersion))
        { Exc = new JsonSchemaVersionException("CollectionNodeVO", NetworkNodeVO.SchemaVersion); }

        if (Exc is not null)
        { ExceptionPopupWrapper.Throw(_Caller, Exc); }
        
    }
}
