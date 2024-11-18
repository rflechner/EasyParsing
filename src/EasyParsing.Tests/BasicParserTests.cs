using EasyParsing.Dsl;
using EasyParsing.Dsl.Linq;
using EasyParsing.Parsers;
using FluentAssertions;

namespace EasyParsing.Tests;

public class BasicParserTests
{
    
    [Test]
    public Task OneCharParser_Should_Fail()
    {
        var context = ParsingContext.FromString("hello");
        var parser = new OneCharParser('a');
        
        IParsingResult<char> result = parser.Parse(context);

        result.Success.Should().BeFalse();
        result.Result.Should().Be(char.MinValue);
        result.Context.Position.Offset.Should().Be(0);
        result.Context.Position.Line.Should().Be(0);
        result.Context.Position.Column.Should().Be(0);
        return Task.CompletedTask;
    }
    
    [Test]
    public Task OneCharParser_Should_Success()
    {
        var context = ParsingContext.FromString("hello");
        var parser = new OneCharParser('h');
        
        IParsingResult<char> result = parser.Parse(context);

        result.Success.Should().BeTrue();
        result.Result.Should().Be('h');
        result.Context.Position.Offset.Should().Be(1);
        result.Context.Position.Line.Should().Be(0);
        result.Context.Position.Column.Should().Be(1);
        return Task.CompletedTask;
    }
    
    [Test]
    public Task CombineOneCharParsers_Should_Success()
    {
        var context = ParsingContext.FromString("hello");
        var hLetterParser = new OneCharParser('h');
        var eLetterParser = new OneCharParser('e');

        var combineParser = hLetterParser
            .Combine(eLetterParser)
            .Select(tuple => new string([tuple.Item1, tuple.Item2]));

        var result = combineParser.Parse(context);
        
        result.Success.Should().BeTrue();
        result.Result.Should().Be("he");
        result.Context.Position.Offset.Should().Be(2);
        result.Context.Position.Line.Should().Be(0);
        result.Context.Position.Column.Should().Be(2);
        return Task.CompletedTask;
    }
    
    [Test]
    public Task UntilTextParser_Should_Success()
    {
        var context = ParsingContext.FromString("my_json_prop :delimiter: 1234");
        var innerParser = Parse.ManySatisfy(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '_');
        var parser = new UntilTextParser<string>(innerParser, ":delimiter:", skipDelimiter: false);
        
        var result = parser.Parse(context);
        
        result.Success.Should().BeTrue();
        result.Result.Should().Be("my_json_prop ");

        result.Context.Remaining.ToString().Should().Be(":delimiter: 1234");
        
        result.Context.Position.Offset.Should().Be(13);
        result.Context.Position.Line.Should().Be(0);
        result.Context.Position.Column.Should().Be(13);
        return Task.CompletedTask;
    }
    
    [Test]
    public Task UntilTextParserTakingAfterMatch_Should_Success()
    {
        var context = ParsingContext.FromString("my_json_prop :delimiter: 1234");
        var innerParser = Parse.ManySatisfy(c => char.IsLetterOrDigit(c) || c == '_' || char.IsWhiteSpace(c));
        var parser = new UntilTextParser<string>(innerParser, ":delimiter:", skipDelimiter: true);
        
        var result = parser.Parse(context);
        
        result.Success.Should().BeTrue();
        result.Result.Should().Be("my_json_prop ");

        result.Context.Remaining.ToString().Should().Be(" 1234");
        
        result.Context.Position.Offset.Should().Be(24);
        result.Context.Position.Line.Should().Be(0);
        result.Context.Position.Column.Should().Be(24);
        return Task.CompletedTask;
    }
    
    
    
    
}