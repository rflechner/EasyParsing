using FluentAssertions;

namespace EasyParsing.Samples.Markdown.Tests;

public class MarkdownParserTests
{
    [TestCase("# Title", "Title", 1)]
    [TestCase("# Title 1", "Title 1", 1)]
    [TestCase("# Title 1 ok", "Title 1 ok", 1)]
    [TestCase("# Title 1 bla bla     ", "Title 1 bla bla", 1)]
    [TestCase("## Title 2 ok", "Title 2 ok", 2)]
    [TestCase("##### Title 5 ok", "Title 5 ok", 5)]
    public void TitleParser_Should_DetectTitleWithLevel1(string input, string expectedTitleText, int expectedLevel)
    {
        var parser = MarkdownParser.TitleParser;
        
        var result = parser.Parse(input);

        result.Success.Should().Be(true);
        result.Context.Remaining.ToString().Should().BeEmpty();

        result.Result.Should().BeEquivalentTo(new Title(expectedLevel, expectedTitleText));
    }
    
}