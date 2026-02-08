using EasyParsing.Dsl;
using EasyParsing.Parsers.Maths;

namespace EasyParsing.Tests;

public class MathsExpressionsParserTests
{
    [Test]
    public void ParsingSimpleAdditionOfTwoSimpleIntegers_Should_Success()
    {
        var operandParser = Parse.SkipSpaces() << Parse.ManySatisfy(char.IsNumber) >> Parse.SkipSpaces();
        var subOperationStart = Parse.SkipSpaces() << Parse.StringMatch("(") >> Parse.SkipSpaces();
        var subOperationEnd = Parse.SkipSpaces() << Parse.StringMatch(")") >> Parse.SkipSpaces();

        var text = "1 + 25";
        var result = MathsParser.ParseAlgebraicExpression(text, operandParser, 
            subOperationStart, subOperationEnd,
            new Operator<string>(OperatorKind.Infix,"+", 1),
            new Operator<string>(OperatorKind.Infix, "-", 1));
        
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result, Is.Not.Null);
        Assert.That(result.Result.Left, Is.EqualTo(new BinaryOperationOperandValue<string>("1")));
        Assert.That(result.Result.Right, Is.EqualTo(new BinaryOperationOperandValue<string>("25")));
        Assert.That(result.Result.Operator, Is.EqualTo(new Operator<string>(OperatorKind.Infix,"+", 1)));
    }
    
    [Test]
    public void ParsingSimpleAdditionOfThreeIntegers_Should_Success()
    {
        var operandParser = Parse.SkipSpaces() << Parse.ManySatisfy(char.IsNumber) >> Parse.SkipSpaces();
        var subOperationStart = Parse.SkipSpaces() << Parse.StringMatch("(") >> Parse.SkipSpaces();
        var subOperationEnd = Parse.SkipSpaces() << Parse.StringMatch(")") >> Parse.SkipSpaces();
        
        var text = "1 + 25 + 589";
        var result = MathsParser.ParseAlgebraicExpression(text, operandParser, 
            subOperationStart, subOperationEnd,
            new Operator<string>(OperatorKind.Infix,"+", 1),
            new Operator<string>(OperatorKind.Infix, "-", 1));
        
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result, Is.Not.Null);
        
        Assert.That(result.Result.Left, Is.EqualTo(new BinaryOperationOperandValue<string>("1")));
        Assert.That(result.Result.Operator, Is.EqualTo(new Operator<string>(OperatorKind.Infix,"+", 1)));
        
        Assert.That(result.Result.Right, Is.InstanceOf<BinaryOperation<string>>());
        
        var rightOperation = (BinaryOperation<string>)result.Result.Right;
        Assert.That(rightOperation.Left, Is.EqualTo(new BinaryOperationOperandValue<string>("25")));
        Assert.That(rightOperation.Right, Is.EqualTo(new BinaryOperationOperandValue<string>("589")));
        Assert.That(rightOperation.Operator, Is.EqualTo(new Operator<string>(OperatorKind.Infix,"+", 1)));
    }
    
    // [Test]
    // public void ParsingSimpleAdditionOfThreeIntegersWithParenthesis_Should_Success()
    // {
    //     var operandParser = Parse.SkipSpaces() << Parse.ManySatisfy(char.IsNumber) >> Parse.SkipSpaces();
    //     var addOperator = Parse.SkipSpaces() << Parse.StringMatch("+") >> Parse.SkipSpaces();
    //     var minusOperator = Parse.SkipSpaces() << Parse.StringMatch("-") >> Parse.SkipSpaces();
    //     
    //     var subOperationStart = Parse.SkipSpaces() << Parse.StringMatch("(") >> Parse.SkipSpaces();
    //     var subOperationEnd = Parse.SkipSpaces() << Parse.StringMatch(")") >> Parse.SkipSpaces();
    //     
    //     var text = "1 + (25 + 589)";
    //     var result = MathsParser.TokenizeAlgebraicExpression(text, operandParser, 
    //         subOperationStart, subOperationEnd,
    //         new Operator<string>(OperatorKind.Infix, addOperator, 1),
    //         new Operator<string>(OperatorKind.Infix, minusOperator, 1));
    //     
    //     Assert.That(result.Success, Is.True);
    //     Assert.That(result.Result, Is.EquivalentTo(new[] {"1", "+", "(", "25", "+", "589", ")"}));
    // }
    //
    
}