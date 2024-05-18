using EasyParsing.Dsl.Linq;
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
        var result = JsonParser.propertyAssignParser.Value.Parse("\"popo\": 'hello, fine ?'");

        result.Success.Should().BeTrue();
        result.Result.Should()
            .BeEquivalentTo(new JsonAst.JsonProperty("popo", new JsonAst.JsonStringValue("hello, fine ?")));
    }
    
    [Test]
    public void PropertyAssignParserWithBool_Should_Success()
    {
        var result = JsonParser.propertyAssignParser.Value.Parse("\"activated\": true");

        result.Success.Should().BeTrue();
        result.Result.Should()
            .BeEquivalentTo(new JsonAst.JsonProperty("activated", new JsonAst.JsonBoolValue(true)));
    }
    
    [TestCase("\"age\": 32.1234", true,  32.1234d, typeof(JsonAst.JsonDecimalValue))]
    [TestCase("'age': 32,1234", true,  32L, typeof(JsonAst.JsonLongValue))]
    [TestCase("'age': dewdfew", false,  0, typeof(JsonAst.JsonDecimalValue))]
    public void PropertyAssignParserWithDecimal_ShouldBe_Expected(string text, bool shouldSuccess, object expectedValue, Type expectedType)
    {
        var result = JsonParser.propertyAssignParser.Value.Parse(text);

        if (shouldSuccess)
        {
            result.Success.Should().BeTrue();

            if (expectedType == typeof(JsonAst.JsonDecimalValue))
            {
                result.Result.Should()
                    .BeEquivalentTo(new JsonAst.JsonProperty("age", new JsonAst.JsonDecimalValue((decimal)(double)expectedValue)));
            }

            if (expectedType == typeof(JsonAst.JsonLongValue))
            {
                result.Result.Should()
                    .BeEquivalentTo(new JsonAst.JsonProperty("age", new JsonAst.JsonLongValue((long)expectedValue)));
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
        var result = JsonParser.propertyAssignParser.Value.Parse(text);

        if (shouldSuccess)
        {
            result.Success.Should().BeTrue();
            result.Result.Should()
                .BeEquivalentTo(new JsonAst.JsonProperty("age", new JsonAst.JsonLongValue(expectedValue)));
            
            return;
        }
        
        result.Success.Should().BeFalse();
    }
    
    [Test]
    public void PropertiesListParserWithMultipleProperties_Should_Success()
    {
        var result = JsonParser.propertiesListParser.Parse("'age': 36, \"activated\": true");

        result.Success.Should().BeTrue();
        result.Result.Should()
            .BeEquivalentTo([
                new JsonAst.JsonProperty("age", new JsonAst.JsonLongValue(36)),
                new JsonAst.JsonProperty("activated", new JsonAst.JsonBoolValue(true)),
            ]);
    }
    
    [Test]
    public void JsonObjectParserWithMultiplePropertiesAtSameLevel_Should_Success()
    {
        var result = JsonParser.jsonObjectParser.Value.Parse("{ \"popo\" : 'hello, fine ?', \"age\": 36, \"size_in_cm\": 1.78, \"activated\": true }");

        result.Success.Should().BeTrue();
        result.Result.Should()
            .BeEquivalentTo(new JsonAst.JsonObject([
                new JsonAst.JsonProperty("popo", new JsonAst.JsonStringValue("hello, fine ?")),
                new JsonAst.JsonProperty("age", new JsonAst.JsonLongValue(36)),
                new JsonAst.JsonProperty("size_in_cm", new JsonAst.JsonDecimalValue(1.78m)),
                new JsonAst.JsonProperty("activated", new JsonAst.JsonBoolValue(true)),
            ]));
    }
    
    [Test]
    public void JsonArrayOfPrimitiveValues_Should_Success()
    {
        var result = JsonParser.jsonArrayParser.Value.Parse("[ 123, 2, 32324.234, 23,22  , \"hello, lol !\" ]");

        result.Success.Should().BeTrue();
        result.Result.Should()
            .BeEquivalentTo(new JsonAst.JsonArray([
                new JsonAst.JsonLongValue(123),
                new JsonAst.JsonLongValue(2),
                new JsonAst.JsonDecimalValue(32324.234m),
                new JsonAst.JsonLongValue(23),
                new JsonAst.JsonLongValue(22),
                new JsonAst.JsonStringValue("hello, lol !")
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
        var result = JsonParser.jsonArrayParser.Value.Parse(json);

        result.Success.Should().BeTrue();
        result.Result.Should()
            .BeEquivalentTo(new JsonAst.JsonArray([
                new JsonAst.JsonLongValue(123),
                new JsonAst.JsonLongValue(2),
                new JsonAst.JsonDecimalValue(32324.234m),
                new JsonAst.JsonLongValue(23),
                new JsonAst.JsonLongValue(22),
                new JsonAst.JsonStringValue("hello, lol !"),
                new JsonAst.JsonObject([
                    new JsonAst.JsonProperty("name", new JsonAst.JsonStringValue("Joe John")),
                    new JsonAst.JsonProperty("description", new JsonAst.JsonStringValue("Hi, I'm \\\"cool\\\" !!")),
                    new JsonAst.JsonProperty("age", new JsonAst.JsonLongValue(39)),
                    new JsonAst.JsonProperty("hobbies", 
                        new JsonAst.JsonArray([
                            new JsonAst.JsonStringValue("lol"),
                            new JsonAst.JsonLongValue(76),
                            new JsonAst.JsonObject([
                                new JsonAst.JsonProperty("id", new JsonAst.JsonStringValue("1234")),
                                new JsonAst.JsonProperty("score", new JsonAst.JsonDecimalValue(1223.234m)),
                                new JsonAst.JsonProperty("names", 
                                    new JsonAst.JsonArray([
                                        new JsonAst.JsonStringValue("game 1"),
                                        new JsonAst.JsonStringValue("game 2"),
                                    ])),
                            ])
                        ])
                    ),
                ]),
            ]));
    }
    
}
