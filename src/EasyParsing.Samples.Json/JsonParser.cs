using System.Globalization;
using EasyParsing.Dsl;
using EasyParsing.Dsl.Linq;
using EasyParsing.Parsers;
using static EasyParsing.Dsl.Parse;

namespace EasyParsing.Samples.Json;

public class JsonParser
{
    
    internal static readonly IParser<string> StartObject = OneCharText('{') >> SkipSpaces();
    internal static readonly IParser<string> EndObject = OneCharText('}') >> SkipSpaces();
    
    internal static readonly IParser<string> StartArray = OneCharText('[') >> SkipSpaces();
    internal static readonly IParser<string> EndArray = OneCharText(']') >> SkipSpaces();
    
    internal static readonly IParser<string> KeyValueSeparator = OneCharText(':') >> SkipSpaces();
    
    internal static readonly IParser<JsonStringValue> JsonStringValueParser =
        from str in QuotedTextParser 
        select new JsonStringValue(str);

    internal static readonly IParser<JsonBoolValue> TrueParser = from str in ManySatisfy(char.IsLetter)
        where str.Equals("true", StringComparison.InvariantCultureIgnoreCase)
        select new JsonBoolValue(true);

    internal static readonly IParser<JsonBoolValue> FalseParser = from str in ManySatisfy(char.IsLetter)
        where str.Equals("false", StringComparison.InvariantCultureIgnoreCase)
        select new JsonBoolValue(false);
    
    internal static readonly IParser<JsonBoolValue> JsonBoolValueParser = TrueParser | FalseParser;

    internal static readonly IParser<JsonDecimalValue> JsonDecimalValueParser =
        from abs in ManySatisfy(char.IsDigit)
        from point in OneCharText('.')
        from rel in ManySatisfy(char.IsDigit)
        select new JsonDecimalValue(decimal.Parse($"{abs}.{rel}", NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture));

    internal static readonly IParser<JsonLongValue> JsonLongValueParser =
        from i in ManySatisfy(char.IsDigit)
        select new JsonLongValue(long.Parse(i, CultureInfo.InvariantCulture));
    
    internal static readonly IParser<JsonProperty> PropertyAssignParser = 
        from key in QuotedTextParser >> SkipSpaces()
        from dotDot in KeyValueSeparator
        from value in ValueParser
        select new JsonProperty(key, value);

    internal static readonly IParser<JsonProperty[]> PropertiesListParser = 
        PropertyAssignParser.SeparatedBy(SkipSpaces() >> OneCharText(',') >> SkipSpaces());
    
    internal static readonly IParser<JsonObject> JsonObjectParser = 
        Between(StartObject, PropertiesListParser,  SkipSpaces() >> EndObject)
            .Select(i => new JsonObject(i.Item.ToDictionary(p => p.Name, p => p.Value)));
    
    internal static readonly IParser<JsonValue[]> ItemsParser = ValueParser.SeparatedBy(SkipSpaces() >> OneCharText(',') >> SkipSpaces());

    // When the parsers depend recursively on each other,
    // building them on the fly by exposing them with a function is a solution for building the global parser.
    internal static IParser<JsonArray> JsonArrayParser =>
            Between(StartArray, ItemsParser,  SkipSpaces() >> EndArray)
            .Select(i => new JsonArray(i.Item));

    internal static IParser<JsonValue> ValueParser =>
        JsonStringValueParser.Cast<JsonStringValue, JsonValue>()
        | JsonBoolValueParser
        | JsonDecimalValueParser
        | JsonLongValueParser
        | JsonObjectParser
        | JsonArrayParser;

    public static JsonValue ParseJson(string text)
    {
        var result = ValueParser.Parse(text);
        
        if (!result.Success)
            throw new JsonParsingException(result.FailureMessage);
        
        if (result.Result == null)
            throw new JsonParsingException("Could not parse JSON.");
        
        return result.Result;
    }
    
}