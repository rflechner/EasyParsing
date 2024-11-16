using EasyParsing.Samples.Markdown.Ast;
using FluentAssertions;

namespace EasyParsing.Samples.Markdown.Tests;

public class TaskListItemParserTests
{
    [TestCase("- [x] #739", true, "#739")]
    [TestCase("- [ ] #739", false, "#739")]
    [TestCase("- [] #739", false, "#739")]
    [TestCase("- [x] task 1", true, "task 1")]
    public void TaskListItemParser_Should_ParseCorrectly(string markdown, bool expectedChecked, string expectedText)
    {
        var parser = MarkdownParser.TaskListItemParser;
        var result = parser.Parse(markdown);
        
        result.Success.Should().BeTrue();
        result.Context.Remaining.ToString().Should().BeEmpty();

        result.Result.Should()
            .BeEquivalentTo(new TaskListItem(expectedChecked, [new RawText(expectedText)]));
    }
}