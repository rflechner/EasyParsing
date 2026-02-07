using EasyParsing.Markdown.Ast;
using FluentAssertions;

namespace EasyParsing.Markdown.Tests;

public class MarkdownParserTests
{
    [Test]
    public void TryParseMarkdown_Should_success()
    {
        var markdown = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, $"{nameof(MarkdownParserTests)}.asset1.md"));
        
        MarkdownParser.TryParseMarkdown(markdown, out var results).Should().BeTrue();

        results[0].Should().Be(new Crlf());
        results[1].Should().BeEquivalentTo(new Title(1, [new RawText("Big title 1")]));
        results[2].Should().Be(new ParagraphStart());
        
        results[3].Should().BeEquivalentTo(new RawText("Hello, does it work ?"));
        
        results[4].Should().Be(new ParagraphStart());

        results[5].Should().BeEquivalentTo(new Title(2, [
            new RawText("Second title"),
            new Bold([
                new InlineQuotingCode("with bold code")
            ])
        ]));
        
        results[6].Should().Be(new ParagraphStart());
        
        results[7].Should()
            .BeEquivalentTo(new ListItems([
                new ListItem(0, "-", [new RawText("list item 1")], 
                [
                    new ListItem(2, "+", [new RawText("lol")], []),
                    new ListItem(2, "+", [new RawText("cool !")], [])
                ]),
                
                new ListItem(0, "-", [new RawText("list item 2")], [])
            ]));
        
    }
}