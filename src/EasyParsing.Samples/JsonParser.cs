using System.Globalization;
using EasyParsing.Dsl;
using EasyParsing.Dsl.Linq;
using EasyParsing.Parsers;
using static EasyParsing.Dsl.Parse;

namespace EasyParsing.Samples;

public class JsonParser
{
    internal static readonly IParser<string> startObject = OneChar('{') >> SkipSpaces();
    internal static readonly IParser<string> endObject = OneChar('}') >> SkipSpaces();
    
    internal static readonly IParser<string> startArray = OneChar('[') >> SkipSpaces();
    internal static readonly IParser<string> endArray = OneChar(']') >> SkipSpaces();
    
    internal static readonly IParser<string> keyValueSeparator = OneChar(':') >> SkipSpaces();
    internal static readonly IParser<string> doubleQuote = OneChar('"');
    internal static readonly IParser<string> simpleQuote = OneChar('\'');
    
    internal static IParser<string> StringParser
    {
        get
        {
            IParser<string> ContentParser (char quoteChar) =>
                ConsumeWhile
                (
                    match => match.Span.EndsWith($"\\{quoteChar}") || !match.Span.EndsWith(quoteChar.ToString())
                );
            
            IParser<string> simpleQuotedStringParser = 
                from start in doubleQuote 
                from str in ContentParser('"') >> doubleQuote 
                select str;
            
            IParser<string> doubleQuotedStringParser = 
                from start in simpleQuote 
                from str in ContentParser('\'') >> simpleQuote 
                select str;
            
            return simpleQuotedStringParser | doubleQuotedStringParser;
        }
    }

    internal static IParser<JsonAst.JsonStringValue> JsonStringValueParser =>
        from str in StringParser 
        select new JsonAst.JsonStringValue(str);

    internal static IParser<JsonAst.JsonBoolValue> JsonBoolValueParser
    {
        get
        {
            var trueParser = from str in ManySatisfy(char.IsLetter)
                where str.Equals("true", StringComparison.InvariantCultureIgnoreCase)
                select new JsonAst.JsonBoolValue(true);

            var falseParser = from str in ManySatisfy(char.IsLetter)
                where str.Equals("false", StringComparison.InvariantCultureIgnoreCase)
                select new JsonAst.JsonBoolValue(false);

            return trueParser | falseParser;
        }
    }

    internal static IParser<JsonAst.JsonDecimalValue> JsonDecimalValueParser =>
        from abs in ManySatisfy(char.IsDigit)
        from point in OneChar('.')
        from rel in ManySatisfy(char.IsDigit)
        select new JsonAst.JsonDecimalValue(decimal.Parse($"{abs}.{rel}", NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture));

    internal static IParser<JsonAst.JsonLongValue> JsonLongValueParser =>
        from i in ManySatisfy(char.IsDigit)
        select new JsonAst.JsonLongValue(long.Parse(i, CultureInfo.InvariantCulture));

    internal static Lazy<IParser<JsonAst.JsonValue>> valueParser = new(() =>
    {
        return JsonStringValueParser.Cast<JsonAst.JsonStringValue, JsonAst.JsonValue>() 
               | JsonBoolValueParser.Cast<JsonAst.JsonBoolValue, JsonAst.JsonValue>()
               | JsonDecimalValueParser.Cast<JsonAst.JsonDecimalValue, JsonAst.JsonValue>()
               | JsonLongValueParser.Cast<JsonAst.JsonLongValue, JsonAst.JsonValue>()
               | jsonObjectParser.Value.Cast<JsonAst.JsonObject, JsonAst.JsonValue>()
               ;
    });

    internal static Lazy<IParser<JsonAst.JsonProperty>> propertyAssignParser = new(() => 
        from key in StringParser >> SkipSpaces()
        from dotDot in keyValueSeparator
        from value in valueParser.Value
        select new JsonAst.JsonProperty(key, value));

    internal static IParser<JsonAst.JsonProperty[]> propertiesListParser => 
        propertyAssignParser.Value.SeparatedBy(SkipSpaces() >> OneChar(',') >> SkipSpaces());
    
    internal static Lazy<IParser<JsonAst.JsonObject>> jsonObjectParser = new(() =>
    {
        return Between(startObject, propertiesListParser,  SkipSpaces() >> endObject)
            .Map(i => new JsonAst.JsonObject(i.Item));
    });
    
    internal static Lazy<IParser<JsonAst.JsonArray>> jsonArrayParser = new(() =>
    {
        IParser<JsonAst.JsonValue[]> itemsParser = valueParser.Value.SeparatedBy(SkipSpaces() >> OneChar(',') >> SkipSpaces());
        
        return Between(startArray, itemsParser,  SkipSpaces() >> endArray)
            .Map(i => new JsonAst.JsonArray(i.Item));
    });
    
    public static JsonAst.JsonValue ParseJson(string text)
    {

        throw new NotImplementedException();
    }
    
}