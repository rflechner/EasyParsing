using System.Globalization;

namespace EasyParsing.Samples;

public static class JsonValueExtensions
{
    public static int ReadAsInt(this JsonAst.JsonValue jsonValue) =>
        jsonValue switch
        {
            JsonAst.JsonDecimalValue jsonDecimalValue => (int)jsonDecimalValue.Value,
            JsonAst.JsonLongValue jsonLongValue => (int)jsonLongValue.Value,
            JsonAst.JsonStringValue jsonStringValue => int.Parse(jsonStringValue.Value),
            _ => throw new ArgumentOutOfRangeException(nameof(jsonValue))
        };
    
    public static decimal ReadAsDecimal(this JsonAst.JsonValue jsonValue) =>
        jsonValue switch
        {
            JsonAst.JsonDecimalValue jsonDecimalValue => jsonDecimalValue.Value,
            JsonAst.JsonLongValue jsonLongValue => jsonLongValue.Value,
            JsonAst.JsonStringValue jsonStringValue => Decimal.Parse(jsonStringValue.Value),
            _ => throw new ArgumentOutOfRangeException(nameof(jsonValue))
        };
    
    public static bool ReadAsBool(this JsonAst.JsonValue jsonValue) =>
        jsonValue switch
        {
            JsonAst.JsonBoolValue jsonBoolValue => jsonBoolValue.Value,
            JsonAst.JsonStringValue jsonStringValue => bool.Parse(jsonStringValue.Value),
            _ => throw new ArgumentOutOfRangeException(nameof(jsonValue))
        };

    public static string ReadAsString(this JsonAst.JsonValue jsonValue) =>
        jsonValue switch
        {
            JsonAst.JsonBoolValue jsonBoolValue => jsonBoolValue.Value.ToString(),
            JsonAst.JsonStringValue jsonStringValue => jsonStringValue.Value,
            JsonAst.JsonDecimalValue jsonDecimalValue => jsonDecimalValue.Value.ToString(CultureInfo.InvariantCulture),
            JsonAst.JsonLongValue jsonLongValue => jsonLongValue.Value.ToString(),
            _ => throw new ArgumentOutOfRangeException(nameof(jsonValue))
        };

    public static JsonAst.JsonValue? GetProperty(this JsonAst.JsonValue? jsonValue, string name)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

        if (jsonValue is null) return null;
        
        if (jsonValue is not JsonAst.JsonObject jsonObject) throw new InvalidOperationException($"{jsonValue.GetType()} has not '{name}' property.");

        if (!jsonObject.Properties.TryGetValue(name, out var propertyValue))
            return null;

        return propertyValue;
    }
    
    public static JsonAst.JsonValue? Select(this JsonAst.JsonValue? jsonValue, string path)
    {
        if (jsonValue is null) return null;
        
        var parts = new Queue<string>(path.Split('.', StringSplitOptions.RemoveEmptyEntries));

        var current = jsonValue;

        while (parts.TryDequeue(out var name))
        {
            if (current == null) return null;
            
            current = current.GetProperty(name);
        }
        
        return current;
    }
    
}