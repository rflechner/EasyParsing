using System.Globalization;

namespace EasyParsing.Samples;

public static class JsonValueExtensions
{
    public static int ReadAsInt(this JsonValue jsonValue) =>
        jsonValue switch
        {
            JsonDecimalValue jsonDecimalValue => (int)jsonDecimalValue.Value,
            JsonLongValue jsonLongValue => (int)jsonLongValue.Value,
            JsonStringValue jsonStringValue => int.Parse(jsonStringValue.Value),
            _ => throw new ArgumentOutOfRangeException(nameof(jsonValue))
        };
    
    public static decimal ReadAsDecimal(this JsonValue jsonValue) =>
        jsonValue switch
        {
            JsonDecimalValue jsonDecimalValue => jsonDecimalValue.Value,
            JsonLongValue jsonLongValue => jsonLongValue.Value,
            JsonStringValue jsonStringValue => decimal.Parse(jsonStringValue.Value),
            _ => throw new ArgumentOutOfRangeException(nameof(jsonValue))
        };
    
    public static bool ReadAsBool(this JsonValue jsonValue) =>
        jsonValue switch
        {
            JsonBoolValue jsonBoolValue => jsonBoolValue.Value,
            JsonStringValue jsonStringValue => bool.Parse(jsonStringValue.Value),
            _ => throw new ArgumentOutOfRangeException(nameof(jsonValue))
        };

    public static string ReadAsString(this JsonValue jsonValue) =>
        jsonValue switch
        {
            JsonBoolValue jsonBoolValue => jsonBoolValue.Value.ToString(),
            JsonStringValue jsonStringValue => jsonStringValue.Value,
            JsonDecimalValue jsonDecimalValue => jsonDecimalValue.Value.ToString(CultureInfo.InvariantCulture),
            JsonLongValue jsonLongValue => jsonLongValue.Value.ToString(),
            _ => throw new ArgumentOutOfRangeException(nameof(jsonValue))
        };

    public static JsonValue? GetProperty(this JsonValue? jsonValue, string name)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

        if (jsonValue is null) return null;
        
        if (jsonValue is not JsonObject jsonObject) throw new InvalidOperationException($"{jsonValue.GetType()} has not '{name}' property.");

        if (!jsonObject.Properties.TryGetValue(name, out var propertyValue))
            return null;

        return propertyValue;
    }
    
    public static JsonValue? Select(this JsonValue? jsonValue, string path)
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