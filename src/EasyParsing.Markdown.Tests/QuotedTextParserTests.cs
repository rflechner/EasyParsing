using EasyParsing.Markdown.Ast;
using FluentAssertions;

namespace EasyParsing.Markdown.Tests;

public class QuotedTextParserTests
{
    [Test]
    public void QuotingTextParser_should_ParseCorrectly()
    {
        var parser = MarkdownParser.QuotingTextParser;
        var result = parser.Parse("> Text that is a quote\nnext line ...");
        
        result.Success.Should().BeTrue();
        result.Context.Remaining.ToString().Should().Be("\nnext line ...");

        result.Result.Should().BeEquivalentTo(new QuotingText("Text that is a quote"));
    }
    
    [Test]
    public void InlineQuotingCodeParser_should_ParseCorrectly()
    {
        var parser = MarkdownParser.InlineQuotingCodeParser;
        var result = parser.Parse("`git status`");
        
        result.Success.Should().BeTrue();
        result.Context.Remaining.ToString().Should().BeEmpty();

        result.Result.Should().BeEquivalentTo(new InlineQuotingCode("git status"));
    }
    
    [Test]
    public void QuotingCodeParserWithNoLang_Should_ParseCorrectly()
    {
        var parser = MarkdownParser.QuotingCodeParser;
        var result = parser.Parse("```\ngit status\ngit add\ngit commit\n```");
        
        result.Success.Should().BeTrue();
        result.Context.Remaining.ToString().Should().BeEmpty();

        result.Result.Should().BeEquivalentTo(new QuotingCode("git status\ngit add\ngit commit", string.Empty));
    }
    
    [Test]
    public void QuotingCodeParserWithLangCsharp_Should_ParseCorrectly()
    {
        var parser = MarkdownParser.QuotingCodeParser;
        var result = parser.Parse("```C#\npublic class Toto\n{\n}\n\n```");
        
        result.Success.Should().BeTrue();
        result.Context.Remaining.ToString().Should().BeEmpty();

        result.Result.Should().BeEquivalentTo(new QuotingCode("public class Toto\n{\n}", "C#"));
    }
    
}