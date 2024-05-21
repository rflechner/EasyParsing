namespace EasyParsing.Samples;

public abstract record JsonAst
{
    public record JsonProperty(string Name, JsonValue Value) : JsonAst;
    
    public abstract record JsonValue : JsonAst;

    public sealed record JsonStringValue(string Value) : JsonValue;
    public sealed record JsonLongValue(long Value) : JsonValue;
    public sealed record JsonDecimalValue(decimal Value) : JsonValue;
    public sealed record JsonBoolValue(bool Value) : JsonValue;
    
    public sealed record JsonArray(JsonValue[] Items) : JsonValue;
    
    public sealed record JsonObject(JsonProperty[] Properties) : JsonValue;
}