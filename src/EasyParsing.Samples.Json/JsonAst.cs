namespace EasyParsing.Samples.Json;

public record JsonProperty(string Name, JsonValue Value);
    
public abstract record JsonValue;

public sealed record JsonStringValue(string Value) : JsonValue;
public sealed record JsonLongValue(long Value) : JsonValue;
public sealed record JsonDecimalValue(decimal Value) : JsonValue;
public sealed record JsonBoolValue(bool Value) : JsonValue;
    
public sealed record JsonArray(JsonValue[] Items) : JsonValue;
    
public sealed record JsonObject(IDictionary<string, JsonValue> Properties) : JsonValue;
