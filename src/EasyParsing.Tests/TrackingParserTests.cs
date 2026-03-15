using EasyParsing.Dsl.Linq;
using EasyParsing.Parsers;
using FluentAssertions;

namespace EasyParsing.Tests;

public class TrackingParserTests
{
    [Test]
    public void CombineOneCharParsersWithTracking_Should_Success()
    {
        var context = ParsingContext.FromString("hello");
        var hLetterParser = new OneCharParser('h').Track();
        var eLetterParser = new OneCharParser('e');

        var parser = 
            from h in hLetterParser
            from e in eLetterParser.Track()
            let a = $"{e.Value}aaa"
            select new
            {
                Premier = h, 
                Second = e,
                Concat = a
            };

        var parsingResult = parser.Parse(context);

        parsingResult.Success.Should().BeTrue();
        
        parsingResult.Result!.Premier.Range.Start.Should().BeEquivalentTo(new TextPosition(0, 0, 0));
        parsingResult.Result!.Premier.Value.Should().Be('h');
        parsingResult.Result!.Premier.Range.End.Should().Be(new TextPosition(1, 1, 0));
        
        parsingResult.Result!.Second.Range.Start.Should().BeEquivalentTo(new TextPosition(1, 1, 0));
        parsingResult.Result.Second.Value.Should().Be('e');
        parsingResult.Result!.Second.Range.End.Should().BeEquivalentTo(new TextPosition(2, 2, 0));
        
        parsingResult.Result.Concat.Should().Be("eaaa");
    }
}