using EasyParsing.Dsl;
using EasyParsing.Dsl.Linq;
using EasyParsing.Parsers;
using FluentAssertions;
using static EasyParsing.Dsl.Parse;

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

        var betweenParser = new BetweenParser<char, string, char>(quoteParser, nameParser, quoteParser);

        var context = ParsingContext.FromString("'Toto'");
        var parsingResult = betweenParser.Parse(context);

        parsingResult.Success.Should().BeTrue();
        parsingResult.Result.Item1.Should().Be('\'');
        parsingResult.Result.Item2.Should().Be("Toto");
        parsingResult.Result.Item3.Should().Be('\'');
    }
    
    [Test]
    public void ParseJsonWithOnlyOnePropertyAsInt_Should_Success()
    {
        var startObject = OneChar('{') >> SkipSpaces();
        var endObject = OneChar('}') >> SkipSpaces();
        var keyName = ManyLettersOrDigits() >> SkipSpaces();
        var keyValueSeparator = OneChar(':') >> SkipSpaces();
        var valueParser = ManyLettersOrDigits() >> SkipSpaces();
        
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
 
    [Test]
    public void ParseJsonWithManyIntProperties_Should_Success()
    {
        var startObject = OneChar('{') >> SkipSpaces();
        var endObject = OneChar('}') >> SkipSpaces();
        var keyName = ManySatisfy(c => char.IsLetterOrDigit(c) || c == '_') >> SkipSpaces();
        var keyValueSeparator = OneChar(':') >> SkipSpaces();
        var valueParser = ManyLettersOrDigits() >> SkipSpaces();
        
        var propertyAssign = 
            from key in keyName
            from dotDot in keyValueSeparator
            from value in valueParser
            select (PropertyName: key, PropertyValue: value);

        IParser<(string PropertyName, string PropertyValue)[]> propertiesListParser = propertyAssign.SeparatedBy(OneChar(',') >> SkipSpaces());

        var jsonParser = new BetweenParser<string, (string PropertyName, string PropertyValue)[], string>(startObject, propertiesListParser, endObject);
        
        var result = jsonParser.Parse("{ age: 39, size_in_cm: 179}");

        result.Success.Should().BeTrue();
        result.Result.Before.Should().Be("{");
        result.Result.After.Should().Be("}");

        result.Result.Item.Should().BeEquivalentTo([
            (PropertyName: "age", PropertyValue: "39"),
            (PropertyName: "size_in_cm", PropertyValue: "179"),
        ]);
    }

    [TestCase("\"hello world !\"", "", true)]
    [TestCase("'hello world !'", "", true)]
    [TestCase("\"hello world !'", "", false)]
    [TestCase("\"hello world ", "\"hello world ", false)]
    public void QuotedString_ShouldBe_Expected(string text, string remaining, bool shouldSuccess)
    {
        var doubleQuote = OneChar('"');
        var simpleQuote = OneChar('\'');
        
        IParser<string> ContentParser (char quoteChar)
        {
            return new WhileTextParser(match => match.Span.EndsWith($"\\{quoteChar}") || !match.Span.EndsWith(quoteChar.ToString()));
        }

        var parser =
            (from start in doubleQuote from str in ContentParser('"') select str)
            .Or(from start in simpleQuote from str in ContentParser('\'') select str);

        var result = parser.Parse(text);

        result.Success.Should().Be(shouldSuccess);
    }
    
}
