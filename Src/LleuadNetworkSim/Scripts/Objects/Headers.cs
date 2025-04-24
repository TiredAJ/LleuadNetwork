using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using CSharpFunctionalExtensions;

namespace LleuadNetworkSim.Scripts.Objects;

public struct Headers
{
    public const string DEFAULT_VAL = "DEFAULT";

    static readonly private ImmutableDictionary<string, string> DEFAULT_HEADERS = ImmutableDictionary.CreateRange([
        new KeyValuePair<string, string>("SENDER_ADDRESS", DEFAULT_VAL),
        new KeyValuePair<string, string>("RECEIVE_ADDRESS", DEFAULT_VAL),
        new KeyValuePair<string, string>("TYPE", DEFAULT_VAL),
        new KeyValuePair<string, string>("INDEX", "-1"),
        new KeyValuePair<string, string>("PRIORITY", "20"),
        new KeyValuePair<string, string>("CREATION_TIME", DEFAULT_VAL),
        new KeyValuePair<string, string>("LIFESPAN", "100"),
        new KeyValuePair<string, string>("HOPS", "0"),
        new KeyValuePair<string, string>("RECEIVE_RESPONSE_REQUIRED", "NO"),
        new KeyValuePair<string, string>("MESSAGE_SIZE", "0"),
        new KeyValuePair<string, string>("MAX_SIZE", "1000"),
        new KeyValuePair<string, string>("TOTAL_SIZE", "-1")
    ]);

    private Dictionary<string, string> IntHeaders = DEFAULT_HEADERS.ToDictionary();

    public Headers(Dictionary<string, string> Metadata) {
        foreach (KeyValuePair<string, string> KVP in Metadata)
        { IntHeaders.TryAdd(KVP.Key, KVP.Value); }
    }

    private void ResetHeaders() {
        IntHeaders = DEFAULT_HEADERS.ToDictionary();
    }

    public string GetHeader(Header _H) {
        return IntHeaders[_H.ToStr()];
    }

    public void SetValue(Header _H, string _Value) {
        IntHeaders[_H.ToStr()] = _Value;
    }

    public void SetMetadata(string _Header, string _Value) {
        if (!IntHeaders.TryAdd(_Header, _Value))
        { IntHeaders.Add(_Header, _Value); }
    }

    public Maybe<string> GetMetadata(string _Header) {
        return IntHeaders.TryGetValue(_Header, out string Value) ? Value : Maybe.None;
    }
}
