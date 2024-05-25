using EasyParsing.Dsl;
using EasyParsing.Parsers;
using FluentAssertions;

namespace EasyParsing.Tests;

public class SeparatedByParserTests
{
    [Test]
    public async Task TwoItemsSepByComma_Should_Success()
    {
        var context = ParsingContext.FromString("abcde,12345");
        var parser = new SeparatedByParser<string, string>(Parse.ManyLettersOrDigits(), Parse.OneChar(','));
        
        var result = parser.Parse(context);
        
        result.Success.Should().BeTrue();
        result.Result.Should().BeEquivalentTo(["abcde", "12345"]);
    }
    
    [Test]
    public async Task FiveItemsSepByComma_Should_Success()
    {
        var context = ParsingContext.FromString("abcde,12345,popopo,lalala,98765");
        var parser = new SeparatedByParser<string, string>(Parse.ManyLettersOrDigits(), Parse.OneChar(','));
        
        var result = parser.Parse(context);
        
        result.Success.Should().BeTrue();
        result.Result.Should().BeEquivalentTo(["abcde", "12345", "popopo", "lalala", "98765"]);
    }
    
    [Test]
    public async Task FiveItemsSepByCommaEndingByComma_Should_SuccessAndLeftLastCommaInContext()
    {
        var context = ParsingContext.FromString("abcde,12345,popopo,lalala,98765,");
        var parser = new SeparatedByParser<string, string>(Parse.ManyLettersOrDigits(), Parse.OneChar(','));
        
        var result = parser.Parse(context);
        
        result.Success.Should().BeTrue();
        result.Result.Should().BeEquivalentTo(["abcde", "12345", "popopo", "lalala", "98765"]);
        result.Context.Remaining.ToString().Should().Be(",");
    }
    
    [Test]
    public async Task FiveItemsSepByCommaEndingByCommaWhenMatchTailingSeparator_Should_SuccessAndLeftLastCommaInContext()
    {
        var context = ParsingContext.FromString("abcde,12345,popopo,lalala,98765,");
        var parser = new SeparatedByParser<string, string>(Parse.ManyLettersOrDigits(), Parse.OneChar(','), matchTailingSeparator: true);
        
        var result = parser.Parse(context);
        
        result.Success.Should().BeTrue();
        result.Result.Should().BeEquivalentTo(["abcde", "12345", "popopo", "lalala", "98765"]);
        result.Context.Remaining.ToString().Should().BeEmpty();
    }
    
    
}