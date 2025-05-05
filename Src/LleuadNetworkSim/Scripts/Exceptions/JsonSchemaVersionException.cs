using System;

namespace LleuadNetworkSim.Scripts.Exceptions;

public class JsonSchemaVersionException : Exception
{
    public JsonSchemaVersionException(string _SchemaName, int _ExpectedVer) 
        : base(GetDebugMessage(_SchemaName, _ExpectedVer)) { }

    public JsonSchemaVersionException(string _SchemaName, int _ExpectedVer, int _CurrentVer) 
        : base(GetDebugMessage(_SchemaName, _ExpectedVer, _CurrentVer)) { }

    static private string GetDebugMessage(string _SchemaName, int _ExpectedVer, int _FileVer)
        => $"{_SchemaName} schema failed version validation. " +
           $"Expected version: [{_ExpectedVer}], File version: [{_FileVer}].";
    
    static private string GetDebugMessage(string _SchemaName, int _ExpectedVer)
        => $"{_SchemaName} schema failed version validation. " +
           $"Expected version: [{_ExpectedVer}].";
}
