using EasyParsing.Samples.Markdown.Ast;
using FluentAssertions;

namespace EasyParsing.Samples.Markdown.Tests;

public class MarkdownParserTextsElementsTests
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

        result.Success.Should().BeTrue();
        result.Context.Remaining.ToString().Should().BeEmpty();

        result.Result.Should().BeEquivalentTo(new Title(expectedLevel, [new RawText(expectedTitleText)]));
    }

    [TestCase("[Duck Duck Go](https://duckduckgo.com)", "Duck Duck Go", "https://duckduckgo.com", "")]
    [TestCase("[Page 2](/article?page=2)", "Page 2", "/article?page=2", "")]
    [TestCase("[Page 2 - 50%](/article-x?page=2&var_2=%20lala)", "Page 2 - 50%", "/article-x?page=2&var_2=%20lala", "")]
    [TestCase("[Duck Duck Go](https://duckduckgo.com \"A search engine\")", "Duck Duck Go", "https://duckduckgo.com", "A search engine")]
    public void LinkParser_Should_MatchExpectedLinks(string input, string expectedLinkText, string expectedLinkUrl, string expectedTitleText)
    {
        var parser = MarkdownParser.LinkParser;
        var result = parser.Parse(input);

        result.Success.Should().BeTrue();
        result.Context.Remaining.ToString().Should().BeEmpty();

        result.Result.Should().BeEquivalentTo(new Link(new RawText(expectedLinkText), expectedLinkUrl, expectedTitleText));
    }

    [TestCase("![The San Juan Mountains are beautiful!](/assets/images/san-juan-mountains.jpg)", "The San Juan Mountains are beautiful!", "/assets/images/san-juan-mountains.jpg")]
    public void ImageParser_Should_MatchExpected(string input, string expectedLinkText, string expectedLinkUrl)
    {
        var parser = MarkdownParser.ImageParser;
        var result = parser.Parse(input);

        result.Success.Should().BeTrue();
        result.Context.Remaining.ToString().Should().BeEmpty();

        result.Result.Should().BeEquivalentTo(new Image(expectedLinkText, expectedLinkUrl));
    }

    [TestCase("__This is bold text__", "This is bold text")]
    [TestCase("**This is bold text**", "This is bold text")]
    public void BoldParser_should_MatchExpected(string input, string expectedText)
    {
        var parser = MarkdownParser.MarkdownSyntaxParser;
        var result = parser.Parse(input);

        result.Success.Should().BeTrue();
        result.Context.Remaining.ToString().Should().BeEmpty();

        result.Result.Should().BeEquivalentTo([new Bold([new RawText(expectedText)])]);
    }

    [TestCase("_This is italic text_", "This is italic text")]
    [TestCase("*This is italic text*", "This is italic text")]
    public void ItalicParser_should_MatchExpected(string input, string expectedText)
    {
        var parser = MarkdownParser.MarkdownSyntaxParser;
        var result = parser.Parse(input);

        result.Success.Should().BeTrue();
        result.Context.Remaining.ToString().Should().BeEmpty();

        result.Result.Should().BeEquivalentTo([new Italic([new RawText(expectedText)])]);
    }

    [Test]
    public void RichTextParser_ShouldMatch_BoldFollowedByItalic()
    {
        var parser = MarkdownParser.MarkdownSyntaxParser;
        var result = parser.Parse("**This text is bold** and _this text is italic_");
    
        result.Success.Should().BeTrue();
        result.Context.Remaining.ToString().Should().BeEmpty();

        result.Result.Should().NotBeNull();
        
        MarkdownAst[] parsedText = result.Result!;
        parsedText.Should().HaveCount(3);
        
        parsedText[0].Should().BeOfType<Bold>().Which.Content[0].Should().BeOfType<RawText>().Which.Content.Should().Be("This text is bold");
        parsedText[1].Should().BeOfType<RawText>().Which.Content.Should().Be(" and ");
        parsedText[2].Should().BeOfType<Italic>().Which.Content[0].Should().BeOfType<RawText>().Which.Content.Should().Be("this text is italic");
    }

    [Test]
    public void RichTextParser_ShouldMatch_BoldNestedItalic()
    {
        var parser = MarkdownParser.MarkdownSyntaxParser;
        var result = parser.Parse("**This text is _extremely_ important**");
    
        result.Success.Should().BeTrue();
        result.Context.Remaining.ToString().Should().BeEmpty();
    
        result.Result.Should()
            .BeEquivalentTo([
                new Bold(
                [
                    new RawText("This text is "),
                    new Italic([new RawText("extremely")]),
                    new RawText(" important")
                ])
            ]);
    }
    
    [Test]
    public void RichTextParser_ShouldMatch_BoldNestedItalicAndStrikethrough()
    {
        var parser = MarkdownParser.MarkdownSyntaxParser;
        var result = parser.Parse("**This text is _extremely_ important ~~and false~~ **");
    
        result.Success.Should().BeTrue();
        result.Context.Remaining.ToString().Should().BeEmpty();
    
        result.Result.Should()
            .BeEquivalentTo([
                new Bold(
                [
                    new RawText("This text is "),
                    new Italic([new RawText("extremely")]),
                    new RawText(" important"),
                    new Strikethrough([new RawText("and false")]),
                    new RawText(" ")
                ])
            ]);
    }
    
}