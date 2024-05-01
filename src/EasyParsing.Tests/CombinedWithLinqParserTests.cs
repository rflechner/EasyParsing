using EasyParsing.Linq;
using EasyParsing.Parsers;
using FluentAssertions;

namespace EasyParsing.Tests;

public class CombinedWithLinqParserTests
{
    [Test]
    public void CombineOneCharParsers_Should_Success()
    {
        var context = ParsingContext.FromString("hello");
        var hLetterParser = new OneCharParser('h');
        var eLetterParser = new OneCharParser('e');

        var parser = 
            from h in hLetterParser
            from e in eLetterParser
            let a = $"{e}aaa"
            select new
            {
                Premier = h, 
                Second = e,
                Concat = a
            };

        var parsingResult = parser.Parse(context);

        parsingResult.Success.Should().BeTrue();
        parsingResult.Result!.Premier.Should().Be('h');
        parsingResult.Result.Second.Should().Be('e');
        parsingResult.Result.Concat.Should().Be("eaaa");
    }

    [TestCase("\n", true, '\n')]
    [TestCase("e", true, 'e')]
    [TestCase("as", false,  char.MinValue)]
    public void OrElseParsers_Should_HaveExpected(string input, bool success, char expectedResult)
    {
        var context = ParsingContext.FromString(input);
        IParser<char> newLine = new OneCharParser('\n');
        IParser<char> eLetterParser = new OneCharParser('e');

        var parser = newLine | eLetterParser;
        var result = parser.Parse(context);

        if (success)
        {
            result.Success.Should().BeTrue();
            result.Result.Should().Be(expectedResult);
        }
        else
        {
            result.Success.Should().BeFalse();
        }
    }
    
    [Test]
    public void ManyParser_Should_Success()
    {
        var letterOrDigitParser = new SatisfyParser(char.IsLetterOrDigit);
        var nameParser = new ManyParser<char>(letterOrDigitParser).Map(chars => new string(chars));

        var parsingResult = nameParser.Parse(ParsingContext.FromString("abcde!!"));

        parsingResult.Success.Should().BeTrue();
        parsingResult.Result.Should().Be("abcde");
    }
    
    [Test]
    public void LookupStringParser_Should_Success()
    {
        var parser = new LookupStringParser("name: ");

        var parsingResult = parser.Parse(ParsingContext.FromString("name: lalala"));
        
        parsingResult.Success.Should().BeTrue();
        parsingResult.Result.Should().Be("name: ");
    }
    
    [Test]
    public void BetweenParser_Should_Success()
    {
        var quoteParser = new OneCharParser('\'');
        var letterOrDigitParser = new SatisfyParser(char.IsLetterOrDigit);
        var nameParser = new ManyParser<char>(letterOrDigitParser).Map(chars => new string(chars));

        var betweenParser = new BetweenParser<char, char, string>(quoteParser, nameParser, quoteParser);

        var context = ParsingContext.FromString("'Toto'");
        var parsingResult = betweenParser.Parse(context);

        parsingResult.Success.Should().BeTrue();
        parsingResult.Result.Item1.Should().Be('\'');
        parsingResult.Result.Item2.Should().Be('\'');
        parsingResult.Result.Item3.Should().Be("Toto");
    }
    
    [Test]
    public void JsonParserTest()
    {
        var context = ParsingContext.FromString("{name: 'Toto', age: 39}");
        
        
    }
}
