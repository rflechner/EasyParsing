using EasyParsing.Markdown.Ast;
using FluentAssertions;

namespace EasyParsing.Markdown.Tests;

public class ParagraphParserTests
{
    [Test]
    public void ParagraphParser_Should_ParseCorrectly()
    {
        var parser = MarkdownParser.ParagraphStartParser;
        var result = parser.Parse("\n\n\n");
        
        result.Success.Should().BeTrue();
        result.Context.Remaining.ToString().Should().BeEmpty();

        result.Result.Should().BeOfType<ParagraphStart>();
    }
}