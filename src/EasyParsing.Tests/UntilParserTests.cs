using EasyParsing.Dsl;
using EasyParsing.Dsl.Linq;
using EasyParsing.Parsers;
using static EasyParsing.Dsl.Parse;
using FluentAssertions;

namespace EasyParsing.Tests;

public class UntilParserTests
{
    [Test]
    public void ResultAndRemaining_Should_BeAsExpected()
    {
        var context = ParsingContext.FromString("ab*cd**12345_end");
        var parser =
            ManySatisfy(c => char.IsLetterOrDigit(c) || c == '*')
                .Until(StringPrefix("**") | StringPrefix("__"));
        
        var result = parser.Parse(context);
        
        result.Success.Should().BeTrue();
        result.Context.Remaining.ToString().Should().Be("12345_end");
        result.Result.Should().BeEquivalentTo(("ab*cd", "**"));
    }
    
    [Test]
    public void SelectManyResultsAndRemaining_Should_BeAsExpected()
    {
        var context = ParsingContext.FromString("ab*cd**12345_end");
        var parser =
            from letters in ManySatisfy(c => char.IsLetterOrDigit(c) || c == '*')
                .Until(StringPrefix("**") | StringPrefix("__"))
            from digits in ManySatisfy(char.IsDigit)
            select new
            {
                letters = letters.Item1, 
                delimiter = letters.Item2, 
                digits
            };
        
        var result = parser.Parse(context);
        
        result.Success.Should().BeTrue();
        result.Context.Remaining.ToString().Should().Be("_end");
        result.Result.Should().BeEquivalentTo(new
        {
            letters = "ab*cd",
            delimiter = "**",
            digits = "12345"
        });
    }
    
    
}