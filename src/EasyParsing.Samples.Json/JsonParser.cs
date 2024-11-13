using System.Globalization;
using EasyParsing.Dsl;
using EasyParsing.Dsl.Linq;
using EasyParsing.Parsers;
using static EasyParsing.Dsl.Parse;

namespace EasyParsing.Samples.Json;

public class JsonParser
{
    
    internal static readonly IParser<string> StartObject = OneChar('{') >> SkipSpaces();
    internal static readonly IParser<string> EndObject = OneChar('}') >> SkipSpaces();
    
    internal static readonly IParser<string> StartArray = OneChar('[') >> SkipSpaces();
    internal static readonly IParser<string> EndArray = OneChar(']') >> SkipSpaces();
    
    internal static readonly IParser<string> KeyValueSeparator = OneChar(':') >> SkipSpaces();
    
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
        from point in OneChar('.')
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
        PropertyAssignParser.SeparatedBy(SkipSpaces() >> OneChar(',') >> SkipSpaces());
    
    internal static readonly IParser<JsonObject> JsonObjectParser = 
        Between(StartObject, PropertiesListParser,  SkipSpaces() >> EndObject)
            .Map(i => new JsonObject(i.Item.ToDictionary(p => p.Name, p => p.Value)));
    
    internal static readonly IParser<JsonValue[]> ItemsParser = ValueParser.SeparatedBy(SkipSpaces() >> OneChar(',') >> SkipSpaces());

    // When the parsers depend recursively on each other,
    // building them on the fly by exposing them with a function is a solution for building the global parser.
    internal static IParser<JsonArray> JsonArrayParser =>
            Between(StartArray, ItemsParser,  SkipSpaces() >> EndArray)
            .Map(i => new JsonArray(i.Item));

    internal static IParser<JsonValue> ValueParser =>
            JsonStringValueParser.Cast<JsonStringValue, JsonValue>()
            | JsonBoolValueParser.Cast<JsonBoolValue, JsonValue>()
            | JsonDecimalValueParser.Cast<JsonDecimalValue, JsonValue>()
            | JsonLongValueParser.Cast<JsonLongValue, JsonValue>()
            | JsonObjectParser.Cast<JsonObject, JsonValue>()
            | JsonArrayParser.Cast<JsonArray, JsonValue>();

    public static JsonValue ParseJson(string text)
    {
        ParsingResult<JsonValue> result = ValueParser.Parse(text);
        
        if (!result.Success)
            throw new JsonParsingException(result.FailureMessage);
        
        if (result.Result == null)
            throw new JsonParsingException("Could not parse JSON.");
        
        return result.Result;
    }
    
}