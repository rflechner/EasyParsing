using EasyParsing.Markdown.Html;

namespace EasyParsing.Markdown.Tests;

public class MarkdownToHtmlConverterTests
{
    [Test]
    public async Task ConvertAsync_Should_ParseOneLevelList()
    {
        var markdown = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, $"{nameof(MarkdownToHtmlConverterTests)}.case1.md"));

        MarkdownToHtmlConverter converter = new();
        var html = await converter.ConvertAsync(markdown);

        var outputFile = Path.Combine(AppContext.BaseDirectory, $"{nameof(MarkdownToHtmlConverterTests)}.case1.html");
        await File.WriteAllTextAsync(outputFile, html);
    }
}