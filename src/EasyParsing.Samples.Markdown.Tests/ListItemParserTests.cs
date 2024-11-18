using EasyParsing.Samples.Markdown.Ast;
using FluentAssertions;

namespace EasyParsing.Samples.Markdown.Tests;

public class ListItemParserTests
{
    [TestCase("- list item 1", 0, "-", "list item 1")]
    [TestCase("+ list item 2", 0, "+", "list item 2")]
    [TestCase("* list item 3", 0, "*", "list item 3")]
    [TestCase("1. list item 4", 0, "1.", "list item 4")]
    [TestCase("23. list item 5", 0, "23.", "list item 5")]
    [TestCase("    + list item 6", 4, "+", "list item 6")]
    [TestCase("      230. list item 7", 6, "230.", "list item 7")]
    public void ListItemParser_Should_ParseCorrectly(string markdown, int expectedDepth, string expectedBullet, string expectedText)
    {
        var parser = MarkdownParser.ListItemParser;
        var result = parser.Parse(markdown);
        
        result.Success.Should().BeTrue();
        result.Context.Remaining.ToString().Should().BeEmpty();

        result.Result.Should()
            .BeEquivalentTo(new ListItem(expectedDepth, expectedBullet, [new RawText(expectedText)], []));
    }
    
    [Test]
    public void RichTextParser_Should_ParseOneLevelList()
    {
        var parser = MarkdownParser.MarkdownSyntaxParser;
        var markdown = 
                """
                - list item 1
                - list item 2
                - list item 3
                """.TrimStart();
        
        var result = parser.Parse(markdown);
        
        result.Success.Should().BeTrue();
        result.Context.Remaining.ToString().Should().BeEmpty();
        result.Result.Should().NotBeNull();

        result.Result!.Should().ContainSingle()
            .Subject.Should().BeOfType<ListItems>()
            .Which.Items.Should().HaveCount(3)
            .And.Subject.Should().BeEquivalentTo([
                    new ListItem(0, "-", [new RawText("list item 1")], []),
                    new ListItem(0, "-", [new RawText("list item 2")], []),
                    new ListItem(0, "-", [new RawText("list item 3")], []),
            ], options => options.WithStrictOrdering().IncludingProperties().IncludingAllRuntimeProperties().AllowingInfiniteRecursion());
    }
    
    [Test]
    public void ListParser_Should_ParseOneLevelList()
    {
        var parser = MarkdownParser.ListParser;
        var markdown = @"
- list item 1
- list item 2
- list item 3".TrimStart();
        
        var result = parser.Parse(markdown);
        
        result.Success.Should().BeTrue();
        result.Context.Remaining.ToString().Should().BeEmpty();
        result.Result.Should().NotBeNull();

        var listItems = result.Result!;
        listItems.Should()
            .BeEquivalentTo(new ListItems([
                new ListItem(0, "-", [new RawText("list item 1")], []),
                new ListItem(0, "-", [new RawText("list item 2")], []),
                new ListItem(0, "-", [new RawText("list item 3")], []),
            ]));
    }
    
    [Test]
    public void ListParser_Should_Parse2LevelList()
    {
        var parser = MarkdownParser.ListParser;
        var markdown = @"
- list item 1
  * sub item 1
  * sub item 2
  * sub item 3
  * sub item 4
- list item 2
- list item 3".TrimStart();
        
        var result = parser.Parse(markdown);
        
        result.Success.Should().BeTrue();
        result.Context.Remaining.ToString().Should().BeEmpty();
        result.Result.Should().NotBeNull();

        var listItems = result.Result!;
        listItems.Should()
            .BeEquivalentTo(new ListItems([
                new ListItem(0, "-", [new RawText("list item 1")], 
                [
                    new ListItem(2, "*", [new RawText("sub item 1")], []),
                    new ListItem(2, "*", [new RawText("sub item 2")], []),
                    new ListItem(2, "*", [new RawText("sub item 3")], []),
                    new ListItem(2, "*", [new RawText("sub item 4")], []),
                ]
                ),
                new ListItem(0, "-", [new RawText("list item 2")], []),
                new ListItem(0, "-", [new RawText("list item 3")], []),
            ]));
    }

    
    [Test]
    public void ListParser_Should_Parse3LevelList()
    {
        var parser = MarkdownParser.ListParser;
        var markdown = @"
- list item 1
  * sub item 1
  * sub item 2
    + sub item 2.1
    + sub item 2.2
    + sub item 2.3
  * sub item 3
  * sub item 4
- list item 2
- list item 3".TrimStart();
        
        var result = parser.Parse(markdown);
        
        result.Success.Should().BeTrue();
        result.Context.Remaining.ToString().Should().BeEmpty();
        result.Result.Should().NotBeNull();

        var listItems = result.Result!;
        listItems.Should()
            .BeEquivalentTo(new ListItems([
                new ListItem(0, "-", [new RawText("list item 1")], 
                [
                    new ListItem(2, "*", [new RawText("sub item 1")], 
                        [
                            new ListItem(4, "+", [new RawText("sub item 2.1")], []),
                            new ListItem(4, "+", [new RawText("sub item 2.2")], []),
                            new ListItem(4, "+", [new RawText("sub item 2.3")], []),
                        ]
                    ),
                    new ListItem(2, "*", [new RawText("sub item 2")], []),
                    new ListItem(2, "*", [new RawText("sub item 3")], []),
                    new ListItem(2, "*", [new RawText("sub item 4")], []),
                ]
                ),
                new ListItem(0, "-", [new RawText("list item 2")], []),
                new ListItem(0, "-", [new RawText("list item 3")], []),
            ]));
    }
    
 
    [Test]
    public void ListParser_Should_Parse5LevelList()
    {
        var parser = MarkdownParser.ListParser;
        var markdown = @"
- list item 1
  * sub item 1
  * sub item 2
    + sub item 1.2.1
    + sub item 1.2.2
      - sub item 1.2.2.1
      - sub item 1.2.2.2
      - sub item 1.2.2.3
    + sub item 1.2.3
  * sub item 3
  * sub item 4
- list item 2
    + sub item 2.1
    + sub item 2.2
- list item 3".TrimStart();
        
        var result = parser.Parse(markdown);
        
        result.Success.Should().BeTrue();
        result.Context.Remaining.ToString().Should().BeEmpty();
        result.Result.Should().NotBeNull();

        var listItems = result.Result!;
        listItems.Should()
            .BeEquivalentTo(new ListItems([
                new ListItem(0, "-", [new RawText("list item 1")], 
                [
                    new ListItem(2, "*", [new RawText("sub item 1")], 
                        [
                            new ListItem(4, "+", [new RawText("sub item 1.2.1")], []),
                            new ListItem(4, "+", [new RawText("sub item 1.2.2")], 
                                [
                                    new ListItem(6, "-", [new RawText("sub item 1.2.2.1")], []),
                                    new ListItem(6, "-", [new RawText("sub item 1.2.2.2")], []),
                                    new ListItem(6, "-", [new RawText("sub item 1.2.2.3")], []),
                                ]
                            ),
                            new ListItem(4, "+", [new RawText("sub item 1.2.3")], []),
                        ]
                    ),
                    new ListItem(2, "*", [new RawText("sub item 2")], []),
                    new ListItem(2, "*", [new RawText("sub item 3")], []),
                    new ListItem(2, "*", [new RawText("sub item 4")], []),
                ]
                ),
                
                new ListItem(0, "-", [new RawText("list item 2")], 
                    [
                        new ListItem(4, "+", [new RawText("sub item 2.1")], []),
                        new ListItem(4, "+", [new RawText("sub item 2.2")], []),
                    ]),
                
                new ListItem(0, "-", [new RawText("list item 3")], []),
            ]));
    }
    
    
}