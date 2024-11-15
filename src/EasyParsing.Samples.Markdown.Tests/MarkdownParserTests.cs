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

    [TestCase("[Duck Duck Go](https://duckduckgo.com)", "Duck Duck Go", "https://duckduckgo.com", "")]
    [TestCase("[Page 2](/article?page=2)", "Page 2", "/article?page=2", "")]
    [TestCase("[Page 2 - 50%](/article-x?page=2&var_2=%20lala)", "Page 2 - 50%", "/article-x?page=2&var_2=%20lala", "")]
    [TestCase("[Duck Duck Go](https://duckduckgo.com \"A search engine\")", "Duck Duck Go", "https://duckduckgo.com", "A search engine")]
    public void LinkParser_Should_MatchExpectedLinks(string input, string expectedLinkText, string expectedLinkUrl, string expectedTitleText)
    {
        var parser = MarkdownParser.LinkParser;
        var result = parser.Parse(input);

        result.Success.Should().Be(true);
        result.Context.Remaining.ToString().Should().BeEmpty();

        result.Result.Should().BeEquivalentTo(new Link(new RawText(expectedLinkText), expectedLinkUrl, expectedTitleText));
    }

    [TestCase("![The San Juan Mountains are beautiful!](/assets/images/san-juan-mountains.jpg)", "The San Juan Mountains are beautiful!", "/assets/images/san-juan-mountains.jpg")]
    public void ImageParser_Should_MatchExpected(string input, string expectedLinkText, string expectedLinkUrl)
    {
        var parser = MarkdownParser.ImageParser;
        var result = parser.Parse(input);

        result.Success.Should().Be(true);
        result.Context.Remaining.ToString().Should().BeEmpty();

        result.Result.Should().BeEquivalentTo(new Image(expectedLinkText, expectedLinkUrl));
    }
    
}