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
    public void ParseJsonWithOnlyOnePropertyAsInt_Shoudl_Success()
    {
        var skipSpaces = new SkipSpacesParser();
        var startObject = new OneCharParser('{').AsString() >> skipSpaces;
        var endObject = new OneCharParser('}').AsString() >> skipSpaces;
        var letterOrDigit = new SatisfyParser(char.IsLetterOrDigit);
        var keyName = new ManyParser<char>(letterOrDigit).AsString() >> skipSpaces;
        var keyValueSeparator = new OneCharParser(':').AsString() >> skipSpaces;
        var valueParser = new ManyParser<char>(letterOrDigit).AsString() >> skipSpaces;

        
        var jsonParser = 
            from start in startObject
            from key in keyName
            from dotDot in keyValueSeparator
            from value in valueParser
            from end in endObject
            select new
            {
                Key = key,
                Value = value
            };
        
        var context = ParsingContext.FromString("{ age: 39}");
        var result = jsonParser.Parse(context);

        result.Success.Should().BeTrue();
        result.Result!.Key.Should().Be("age");
        result.Result!.Value.Should().Be("39");

    }
}
