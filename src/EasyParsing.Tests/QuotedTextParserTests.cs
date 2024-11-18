using EasyParsing.Dsl;
using FluentAssertions;

namespace EasyParsing.Tests;

public class QuotedTextParserTests
{
    [Test]
    public void DoubleQuotedText_ShouldBe_Parsed()
    {
        Parse.QuotedTextParser.Parse("\"hello\"").Result.Should().Be("hello");
    }
    
    [Test]
    public void SingleQuotedText_ShouldBe_Parsed()
    {
        Parse.QuotedTextParser.Parse("'hello'").Result.Should().Be("hello");
    }
    
    [Test]
    public void DoubleQuotedTextWithInnerSingleQuotedText_ShouldBe_Parsed()
    {
        Parse.QuotedTextParser.Parse("\"hello 'world' !\"").Result.Should().Be("hello 'world' !");
    }
    
    [Test]
    public void SingleQuotedTextWithInnerDoubleQuotedText_ShouldBe_Parsed()
    {
        Parse.QuotedTextParser.Parse("'hello \"world\" !'").Result.Should().Be("hello \"world\" !");
    }
    
    [Test]
    public void DoubleQuotedTextWithInnerEscapedDoubleQuotedText_ShouldBe_Parsed()
    {
        Parse.QuotedTextParser.Parse("\"hello \\\"world\\\" !\"").Result.Should().Be("hello \"world\" !");
    }
    
    [Test]
    public void SingleQuotedTextWithInnerEscapedSingleQuotedText_ShouldBe_Parsed()
    {
        Parse.QuotedTextParser.Parse("\'hello \\\'world\\\' !\'").Result.Should().Be("hello \'world\' !");
    }
}