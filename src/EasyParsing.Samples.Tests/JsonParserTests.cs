using FluentAssertions;
using NUnit.Framework;

namespace EasyParsing.Samples.Tests;

public class JsonParserTests
{
    [TestCase("\"hello world !\"", "", true)]
    [TestCase("'hello world !'", "", true)]
    [TestCase("\"hello world !'", "\"hello world !'", false)]
    [TestCase("\"hello world ", "\"hello world ", false)]
    public void QuotedString_ShouldBe_Expected(string text, string remaining, bool shouldSuccess)
    {
        var result = JsonParser.StringParser.Parse(text);

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
        var result = JsonParser.propertyAssignParser.Value.Parse("popo: 'hello, fine ?'");

        result.Success.Should().BeTrue();
        result.Result.Should()
            .BeEquivalentTo(new JsonAst.JsonProperty("popo", new JsonAst.JsonStringValue("hello, fine ?")));
    }
    
    [Test]
    public void PropertyAssignParserWithBool_Should_Success()
    {
        var result = JsonParser.propertyAssignParser.Value.Parse("activated: true");

        result.Success.Should().BeTrue();
        result.Result.Should()
            .BeEquivalentTo(new JsonAst.JsonProperty("activated", new JsonAst.JsonBoolValue(true)));
    }
    
    [TestCase("age: 32.1234", true,  32.1234)]
    [TestCase("age: 32,1234", true,  32.1234)]
    [TestCase("age: dewdfew", false,  0)]
    public void PropertyAssignParserWithDecimal_ShouldBe_Expected(string text, bool shouldSuccess, decimal expectedValue)
    {
        var result = JsonParser.propertyAssignParser.Value.Parse(text);

        if (shouldSuccess)
        {
            result.Success.Should().BeTrue();
            result.Result.Should()
                .BeEquivalentTo(new JsonAst.JsonProperty("age", new JsonAst.JsonDecimalValue(expectedValue)));
            
            return;
        }
        
        result.Success.Should().BeFalse();
    }
    
    [TestCase("age: 32", true,  32)]
    [TestCase("age: 656456456445", true,  656456456445)]
    [TestCase("age: dewdfew", false,  0)]
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
        var result = JsonParser.propertiesListParser.Parse("age: 36, activated: true");

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
        var result = JsonParser.jsonObjectParser.Value.Parse("{ popo: 'hello, fine ?', age: 36, size_in_cm: 1.78, activated: true }");

        result.Success.Should().BeTrue();
        result.Result.Should()
            .BeEquivalentTo(new JsonAst.JsonObject([
                new JsonAst.JsonProperty("popo", new JsonAst.JsonStringValue("hello, fine ?")),
                new JsonAst.JsonProperty("age", new JsonAst.JsonLongValue(36)),
                new JsonAst.JsonProperty("size_in_cm", new JsonAst.JsonDecimalValue(1.78m)),
                new JsonAst.JsonProperty("activated", new JsonAst.JsonBoolValue(true)),
            ]));
    }
    
}
