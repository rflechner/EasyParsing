using EasyParsing.Dsl;
using EasyParsing.Parsers;
using FluentAssertions;

namespace EasyParsing.Tests;

public class SeparatedByParserTests
{
    [Test]
    public Task TwoItemsSepByComma_Should_Success()
    {
        var context = ParsingContext.FromString("abcde,12345");
        var parser = new SeparatedByParser<string, string>(Parse.ManyLettersOrDigits(), Parse.OneChar(','));
        
        var result = parser.Parse(context);
        
        result.Success.Should().BeTrue();
        result.Result.Should().BeEquivalentTo(["abcde", "12345"]);
        return Task.CompletedTask;
    }
    
    [Test]
    public Task FiveItemsSepByComma_Should_Success()
    {
        var context = ParsingContext.FromString("abcde,12345,popopo,lalala,98765");
        var parser = new SeparatedByParser<string, string>(Parse.ManyLettersOrDigits(), Parse.OneChar(','));
        
        var result = parser.Parse(context);
        
        result.Success.Should().BeTrue();
        result.Result.Should().BeEquivalentTo(["abcde", "12345", "popopo", "lalala", "98765"]);
        return Task.CompletedTask;
    }
    
    [Test]
    public Task FiveItemsSepByCommaEndingByComma_Should_SuccessAndLeftLastCommaInContext()
    {
        var context = ParsingContext.FromString("abcde,12345,popopo,lalala,98765,");
        var parser = new SeparatedByParser<string, string>(Parse.ManyLettersOrDigits(), Parse.OneChar(','));
        
        var result = parser.Parse(context);
        
        result.Success.Should().BeTrue();
        result.Result.Should().BeEquivalentTo(["abcde", "12345", "popopo", "lalala", "98765"]);
        result.Context.Remaining.ToString().Should().Be(",");
        return Task.CompletedTask;
    }
    
    [Test]
    public Task FiveItemsSepByCommaEndingByCommaWhenMatchTailingSeparator_Should_SuccessAndLeftLastCommaInContext()
    {
        var context = ParsingContext.FromString("abcde,12345,popopo,lalala,98765,");
        var parser = new SeparatedByParser<string, string>(Parse.ManyLettersOrDigits(), Parse.OneChar(','), matchTailingSeparator: true);
        
        var result = parser.Parse(context);
        
        result.Success.Should().BeTrue();
        result.Result.Should().BeEquivalentTo(["abcde", "12345", "popopo", "lalala", "98765"]);
        result.Context.Remaining.ToString().Should().BeEmpty();
        return Task.CompletedTask;
    }
    
    
}