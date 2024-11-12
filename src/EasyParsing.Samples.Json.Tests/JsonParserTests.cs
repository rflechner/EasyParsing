using EasyParsing.Dsl.Linq;
using EasyParsing.Samples.Json;
using FluentAssertions;
using NUnit.Framework;
using static EasyParsing.Dsl.Parse;

namespace EasyParsing.Samples.Tests;

public class JsonParserTests
{

    [Test]
    public void ParseStringFollowedByChar_Should_Success()
    {
        var strParser = JsonParser.StringParser;// >> SkipSpaces();
        var keyValueSeparator = OneChar(':') >> SkipSpaces();
        
        var parser = 
            from key in strParser
            from dotDot in keyValueSeparator
            select new
            {
                key,
                dotDot
            };

        var strResult = strParser.Parse("\"name\"");
        var combinedResult = parser.Parse("\"name\":");

        strResult.Success.Should().BeTrue();
        strResult.Result.Should().Be("name");

        combinedResult.Success.Should().BeTrue();
        combinedResult.Result.Should()
            .BeEquivalentTo(new
            {
                key = "name",
                dotDot = ":",
            });
    }
    
    [TestCase("\"hello world !\"", "", true)]
    [TestCase("\"name\":", ":", true)]
    [TestCase("'hello world !'", "", true)]
    [TestCase("\"hello world !'", "\"hello world !'", false)]
    [TestCase("\"hello world ", "\"hello world ", false)]
    public void QuotedString_ShouldBe_Expected(string text, string remaining, bool shouldSuccess)
    {
        var parser = JsonParser.StringParser >> SkipSpaces();
        
        var result = parser.Parse(text);

        result.Success.Should().Be(shouldSuccess);
        result.Context.Remaining.ToString().Should().Be(remaining);
    }

    [Test]
    public void JsonBoolValueParser_ShouldBe_True()
    {
        var result = JsonParser.JsonBoolValueParser.Parse("true");

        result.Success.Should().BeTrue();
        result.Result!.Value.Should().BeTrue();
    }

    [Test]
    public void JsonBoolValueParser_ShouldBe_False()
    {
        var result = JsonParser.JsonBoolValueParser.Parse("false");

        result.Success.Should().BeTrue();
        result.Result!.Value.Should().BeFalse();
    }
    
    [Test]
    public void JsonBoolValueParser_Should_Fail()
    {
        var result = JsonParser.JsonBoolValueParser.Parse("toto");

        result.Success.Should().BeFalse();
    }
    
    [Test]
    public void PropertyAssignParserWithString_Should_Success()
    {
        var result = JsonParser.PropertyAssignParser.Parse("\"popo\": 'hello, fine ?'");

        result.Success.Should().BeTrue();
        result.Result.Should()
            .BeEquivalentTo(new JsonProperty("popo", new JsonStringValue("hello, fine ?")));
    }
    
    [Test]
    public void PropertyAssignParserWithBool_Should_Success()
    {
        var result = JsonParser.PropertyAssignParser.Parse("\"activated\": true");

        result.Success.Should().BeTrue();
        result.Result.Should()
            .BeEquivalentTo(new JsonProperty("activated", new JsonBoolValue(true)));
    }
    
    [TestCase("\"age\": 32.1234", true,  32.1234d, typeof(JsonDecimalValue))]
    [TestCase("'age': 32,1234", true,  32L, typeof(JsonLongValue))]
    [TestCase("'age': dewdfew", false,  0, typeof(JsonDecimalValue))]
    public void PropertyAssignParserWithDecimal_ShouldBe_Expected(string text, bool shouldSuccess, object expectedValue, Type expectedType)
    {
        var result = JsonParser.PropertyAssignParser.Parse(text);

        if (shouldSuccess)
        {
            result.Success.Should().BeTrue();

            if (expectedType == typeof(JsonDecimalValue))
            {
                result.Result.Should()
                    .BeEquivalentTo(new JsonProperty("age", new JsonDecimalValue((decimal)(double)expectedValue)));
            }

            if (expectedType == typeof(JsonLongValue))
            {
                result.Result.Should()
                    .BeEquivalentTo(new JsonProperty("age", new JsonLongValue((long)expectedValue)));
            }

            return;
        }
        
        result.Success.Should().BeFalse();
    }
    
    [TestCase("'age': 32", true,  32)]
    [TestCase("\"age\": 656456456445", true,  656456456445)]
    [TestCase("\"age\": dewdfew", false,  0)]
    public void PropertyAssignParserWithLong_ShouldBe_Expected(string text, bool shouldSuccess, long expectedValue)
    {
        var result = JsonParser.PropertyAssignParser.Parse(text);

        if (shouldSuccess)
        {
            result.Success.Should().BeTrue();
            result.Result.Should()
                .BeEquivalentTo(new JsonProperty("age", new JsonLongValue(expectedValue)));
            
            return;
        }
        
        result.Success.Should().BeFalse();
    }
    
    [Test]
    public void PropertiesListParserWithMultipleProperties_Should_Success()
    {
        var result = JsonParser.PropertiesListParser.Parse("'age': 36, \"activated\": true");

        result.Success.Should().BeTrue();
        result.Result.Should()
            .BeEquivalentTo([
                new JsonProperty("age", new JsonLongValue(36)),
                new JsonProperty("activated", new JsonBoolValue(true)),
            ]);
    }
    
    [Test]
    public void JsonObjectParserWithMultiplePropertiesAtSameLevel_Should_Success()
    {
        var result = JsonParser.JsonObjectParser.Parse("{ \"popo\" : 'hello, fine ?', \"age\": 36, \"size_in_cm\": 1.78, \"activated\": true }");

        result.Success.Should().BeTrue();
        result.Result.Should()
            .BeEquivalentTo(new JsonObject(
            new Dictionary<string, JsonValue>
            {
                { "popo", new JsonStringValue("hello, fine ?") },
                {"age", new JsonLongValue(36) },
                {"size_in_cm", new JsonDecimalValue(1.78m) },
                {"activated", new JsonBoolValue(true) },   
            }));
    }
    
    [Test]
    public void JsonArrayOfPrimitiveValues_Should_Success()
    {
        var result = JsonParser.JsonArrayParser.Parse("[ 123, 2, 32324.234, 23,22  , \"hello, lol !\" ]");

        result.Success.Should().BeTrue();
        result.Result.Should()
            .BeEquivalentTo(new JsonArray([
                new JsonLongValue(123),
                new JsonLongValue(2),
                new JsonDecimalValue(32324.234m),
                new JsonLongValue(23),
                new JsonLongValue(22),
                new JsonStringValue("hello, lol !")
            ]));
    }
        
    [Test]
    public void JsonArrayOfPrimitiveValuesAndObject_Should_Success()
    {
        var json = """
                             [ 
                                 123, 2, 32324.234, 23,22  , 
                                 "hello, lol !", 
                                 {
                                     "name": "Joe John",
                                     "description": "Hi, I'm \"cool\" !!",
                                     "age": 39,
                                     'hobbies': [
                                        'lol',
                                        76,
                                        {
                                            'id': 1234,
                                            "score": 1223.234,
                                            'names': ['game 1', "game 2"]
                                        }
                                     ]
                                 }
                             ]
                             """;
        var result = JsonParser.JsonArrayParser.Parse(json);

        result.Success.Should().BeTrue();
        result.Result.Should()
            .BeEquivalentTo(new JsonArray([
                new JsonLongValue(123),
                new JsonLongValue(2),
                new JsonDecimalValue(32324.234m),
                new JsonLongValue(23),
                new JsonLongValue(22),
                new JsonStringValue("hello, lol !"),
                
                new JsonObject(new Dictionary<string, JsonValue>
                {
                    { "name", new JsonStringValue("Joe John") },
                    { "description", new JsonStringValue("Hi, I'm \"cool\" !!") },
                    { "age", new JsonLongValue(39) },
                    { "hobbies", 
                        new JsonArray([
                            new JsonStringValue("lol"),
                            new JsonLongValue(76),
                            
                            new JsonObject(new Dictionary<string, JsonValue>
                            {
                                {"id", new JsonStringValue("1234")},
                                {"score", new JsonDecimalValue(1223.234m)},
                                {
                                    "names", 
                                    new JsonArray([
                                        new JsonStringValue("game 1"),
                                        new JsonStringValue("game 2"),
                                    ])
                                },
                            })
                            
                        ]) }
                })
            ]));
    }
    
}
