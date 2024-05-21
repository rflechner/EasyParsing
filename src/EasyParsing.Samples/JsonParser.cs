using System.Globalization;
using EasyParsing.Dsl;
using EasyParsing.Dsl.Linq;
using EasyParsing.Parsers;
using static EasyParsing.Dsl.Parse;

namespace EasyParsing.Samples;

public class JsonParser
{
    internal static IParser<string> CreateStringParser(char quoteChar)
    {
        var contentParser = ConsumeWhile
        (
            match => match.Span.EndsWith($"\\{quoteChar}") || !match.Span.EndsWith(quoteChar.ToString())
        );
        var quote = OneChar(quoteChar);
        
        return 
            from start in quote 
            from str in contentParser >> quote 
            select str;
    }
    
    internal static readonly IParser<string> StartObject = OneChar('{') >> SkipSpaces();
    internal static readonly IParser<string> EndObject = OneChar('}') >> SkipSpaces();
    
    internal static readonly IParser<string> StartArray = OneChar('[') >> SkipSpaces();
    internal static readonly IParser<string> EndArray = OneChar(']') >> SkipSpaces();
    
    internal static readonly IParser<string> KeyValueSeparator = OneChar(':') >> SkipSpaces();

    internal static readonly IParser<string> StringParser = CreateStringParser('\'') | CreateStringParser('"');
    
    internal static readonly IParser<JsonAst.JsonStringValue> JsonStringValueParser =
        from str in StringParser 
        select new JsonAst.JsonStringValue(str);

    internal static readonly IParser<JsonAst.JsonBoolValue> TrueParser = from str in ManySatisfy(char.IsLetter)
        where str.Equals("true", StringComparison.InvariantCultureIgnoreCase)
        select new JsonAst.JsonBoolValue(true);

    internal static readonly IParser<JsonAst.JsonBoolValue> FalseParser = from str in ManySatisfy(char.IsLetter)
        where str.Equals("false", StringComparison.InvariantCultureIgnoreCase)
        select new JsonAst.JsonBoolValue(false);
    
    internal static readonly IParser<JsonAst.JsonBoolValue> JsonBoolValueParser = TrueParser | FalseParser;

    internal static readonly IParser<JsonAst.JsonDecimalValue> JsonDecimalValueParser =
        from abs in ManySatisfy(char.IsDigit)
        from point in OneChar('.')
        from rel in ManySatisfy(char.IsDigit)
        select new JsonAst.JsonDecimalValue(decimal.Parse($"{abs}.{rel}", NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture));

    internal static readonly IParser<JsonAst.JsonLongValue> JsonLongValueParser =
        from i in ManySatisfy(char.IsDigit)
        select new JsonAst.JsonLongValue(long.Parse(i, CultureInfo.InvariantCulture));
    
    internal static readonly IParser<JsonAst.JsonProperty> PropertyAssignParser = 
        from key in StringParser >> SkipSpaces()
        from dotDot in KeyValueSeparator
        from value in ValueParser
        select new JsonAst.JsonProperty(key, value);

    internal static readonly IParser<JsonAst.JsonProperty[]> PropertiesListParser = 
        PropertyAssignParser.SeparatedBy(SkipSpaces() >> OneChar(',') >> SkipSpaces());
    
    internal static readonly IParser<JsonAst.JsonObject> JsonObjectParser = 
        Between(StartObject, PropertiesListParser,  SkipSpaces() >> EndObject)
            .Map(i => new JsonAst.JsonObject(i.Item));
    
    internal static readonly IParser<JsonAst.JsonValue[]> ItemsParser = ValueParser.SeparatedBy(SkipSpaces() >> OneChar(',') >> SkipSpaces());

    // When the parsers depend recursively on each other,
    // building them on the fly by exposing them with a function is a solution for building the global parser.
    internal static IParser<JsonAst.JsonArray> JsonArrayParser =>
            Between(StartArray, ItemsParser,  SkipSpaces() >> EndArray)
            .Map(i => new JsonAst.JsonArray(i.Item));

    internal static IParser<JsonAst.JsonValue> ValueParser =>
            JsonStringValueParser.Cast<JsonAst.JsonStringValue, JsonAst.JsonValue>()
            | JsonBoolValueParser.Cast<JsonAst.JsonBoolValue, JsonAst.JsonValue>()
            | JsonDecimalValueParser.Cast<JsonAst.JsonDecimalValue, JsonAst.JsonValue>()
            | JsonLongValueParser.Cast<JsonAst.JsonLongValue, JsonAst.JsonValue>()
            | JsonObjectParser.Cast<JsonAst.JsonObject, JsonAst.JsonValue>()
            | JsonArrayParser.Cast<JsonAst.JsonArray, JsonAst.JsonValue>();

    public static JsonAst.JsonValue ParseJson(string text)
    {
        ParsingResult<JsonAst.JsonValue> result = ValueParser.Parse(text);
        
        if (!result.Success)
            throw new JsonParsingException(result.FailureMessage);
        
        if (result.Result == null)
            throw new JsonParsingException("Could not parse JSON.");
        
        return result.Result;
    }
    
}