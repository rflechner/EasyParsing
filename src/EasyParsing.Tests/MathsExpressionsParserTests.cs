using EasyParsing.Dsl;
using EasyParsing.Parsers.Maths;

namespace EasyParsing.Tests;

public class MathsExpressionsParserTests
{
    [Test]
    public void ParsingSimpleAdditionOfTwoSimpleIntegers_Should_Success()
    {
        var operandParser = Parse.SkipSpaces() << Parse.ManySatisfy(char.IsNumber) >> Parse.SkipSpaces();
        var addOperator = Parse.SkipSpaces() << Parse.StringMatch("+") >> Parse.SkipSpaces();
        var minusOperator = Parse.SkipSpaces() << Parse.StringMatch("-") >> Parse.SkipSpaces();
        
        var text = "1 + 25";
        var result = MathsParser.TokenizeAlgebraicExpression(text, operandParser, 
            new Operator<string>(OperatorKind.Infix, addOperator, 1),
            new Operator<string>(OperatorKind.Infix, minusOperator, 1));
        
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result, Is.EquivalentTo(new[] {"1", "+", "25"}));
    }
    
    [Test]
    public void ParsingSimpleAdditionOfThreeIntegers_Should_Success()
    {
        var operandParser = Parse.SkipSpaces() << Parse.ManySatisfy(char.IsNumber) >> Parse.SkipSpaces();
        var addOperator = Parse.SkipSpaces() << Parse.StringMatch("+") >> Parse.SkipSpaces();
        var minusOperator = Parse.SkipSpaces() << Parse.StringMatch("-") >> Parse.SkipSpaces();
        
        var text = "1 + 25 + 589";
        var result = MathsParser.TokenizeAlgebraicExpression(text, operandParser, 
            new Operator<string>(OperatorKind.Infix, addOperator, 1),
            new Operator<string>(OperatorKind.Infix, minusOperator, 1));
        
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result, Is.EquivalentTo(new[] {"1", "+", "25", "+", "589"}));
    }
    
    [Test]
    public void ParsingSimpleAdditionOfThreeIntegersWithParenthesis_Should_Success()
    {
        var operandParser = Parse.SkipSpaces() << Parse.ManySatisfy(char.IsNumber) >> Parse.SkipSpaces();
        var addOperator = Parse.SkipSpaces() << Parse.StringMatch("+") >> Parse.SkipSpaces();
        var minusOperator = Parse.SkipSpaces() << Parse.StringMatch("-") >> Parse.SkipSpaces();
        
        var text = "1 + (25 + 589)";
        var result = MathsParser.TokenizeAlgebraicExpression(text, operandParser, 
            new Operator<string>(OperatorKind.Infix, addOperator, 1),
            new Operator<string>(OperatorKind.Infix, minusOperator, 1));
        
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result, Is.EquivalentTo(new[] {"1", "+", "25", "+", "589"}));
    }
    
    
}