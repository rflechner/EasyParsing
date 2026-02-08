using EasyParsing.Dsl;
using EasyParsing.Parsers.Maths;

namespace EasyParsing.Tests;

public class MathsExpressionsParserTests
{
    [Test]
    public void ParsingSimpleAdditionOfTwoSimpleIntegers_Should_Success()
    {
        var operandParser = Parse.SkipSpaces() << Parse.ManySatisfy(char.IsNumber) >> Parse.SkipSpaces();
        IParser<string> subOperationStart = Parse.SkipSpaces() << Parse.StringMatch("(") >> Parse.SkipSpaces();
        IParser<string> subOperationEnd = Parse.SkipSpaces() << Parse.StringMatch(")") >> Parse.SkipSpaces();

        var text = "1 + 25";

        var result = MathsParser.ParseAlgebraicExpression(
            subOperationStart, subOperationEnd,
            operandParser,
        [
            new Operator<string>(OperatorKind.Infix, "+", 1),
            new Operator<string>(OperatorKind.Infix, "-", 1)
        ]).Parse(text);
        
        Assert.That(result.Success, Is.True);
        var binaryOperation = (BinaryOperation<string>) result.Result!;
        
        Assert.That(result, Is.Not.Null);
        Assert.That(binaryOperation.Left, Is.EqualTo(new BinaryOperationOperandValue<string>("1")));
        Assert.That(binaryOperation.Right, Is.EqualTo(new BinaryOperationOperandValue<string>("25")));
        Assert.That(binaryOperation.Operator, Is.EqualTo(new Operator<string>(OperatorKind.Infix,"+", 1)));
    }
    
    [Test]
    public void ParsingSimpleAdditionOfThreeIntegers_Should_Success()
    {
        var operandParser = Parse.SkipSpaces() << Parse.ManySatisfy(char.IsNumber) >> Parse.SkipSpaces();
        var subOperationStart = Parse.SkipSpaces() << Parse.StringMatch("(") >> Parse.SkipSpaces();
        var subOperationEnd = Parse.SkipSpaces() << Parse.StringMatch(")") >> Parse.SkipSpaces();
        
        var text = "1 + 25 + 589";
        
        var result = MathsParser.ParseAlgebraicExpression(subOperationStart, subOperationEnd,
            operandParser,
        [
            new Operator<string>(OperatorKind.Infix, "+", 1),
            new Operator<string>(OperatorKind.Infix, "-", 1)
        ]).Parse(text);
        
        Assert.That(result.Success, Is.True);
        var binaryOperation = (BinaryOperation<string>) result.Result!;
        
        Assert.That(result.Success, Is.True);
        Assert.That(result.Result, Is.Not.Null);
        
        
        Assert.That(binaryOperation.Left, Is.InstanceOf<BinaryOperation<string>>());
        
        var leftOperation = (BinaryOperation<string>)binaryOperation.Left;
        Assert.That(leftOperation.Left, Is.EqualTo(new BinaryOperationOperandValue<string>("1")));
        Assert.That(leftOperation.Right, Is.EqualTo(new BinaryOperationOperandValue<string>("25")));
        Assert.That(leftOperation.Operator, Is.EqualTo(new Operator<string>(OperatorKind.Infix,"+", 1)));
        
        Assert.That(binaryOperation.Operator, Is.EqualTo(new Operator<string>(OperatorKind.Infix,"+", 1)));
        
        Assert.That(binaryOperation.Right, Is.EqualTo(new BinaryOperationOperandValue<string>("589")));
    }
    
    [Test]
    public void ParsingMultiplicationWithAdditionOfThreeIntegers_Should_Success()
    {
        var operandParser = Parse.SkipSpaces() << Parse.ManySatisfy(char.IsNumber) >> Parse.SkipSpaces();
        var subOperationStart = Parse.SkipSpaces() << Parse.StringMatch("(") >> Parse.SkipSpaces();
        var subOperationEnd = Parse.SkipSpaces() << Parse.StringMatch(")") >> Parse.SkipSpaces();
        
        var text = "1 * 25 + 589";
        
        var result = MathsParser.ParseAlgebraicExpression(subOperationStart, subOperationEnd,
            operandParser,
        [
            new Operator<string>(OperatorKind.Infix, "+", 10),
            new Operator<string>(OperatorKind.Infix, "-", 10),
            new Operator<string>(OperatorKind.Infix, "*", 20),
            new Operator<string>(OperatorKind.Infix, "/", 20),
        ]).Parse(text);
        
        Assert.That(result.Success, Is.True);
        var binaryOperation = (BinaryOperation<string>) result.Result!;
        Assert.That(result.Result, Is.Not.Null);
        Assert.That(binaryOperation.Left, Is.InstanceOf<BinaryOperation<string>>());
        
        var leftOperation = (BinaryOperation<string>)binaryOperation.Left;
        Assert.That(leftOperation.Left, Is.EqualTo(new BinaryOperationOperandValue<string>("1")));
        Assert.That(leftOperation.Right, Is.EqualTo(new BinaryOperationOperandValue<string>("25")));
        Assert.That(leftOperation.Operator, Is.EqualTo(new Operator<string>(OperatorKind.Infix,"*", 20)));
        
        Assert.That(binaryOperation.Operator, Is.EqualTo(new Operator<string>(OperatorKind.Infix,"+", 10)));
        
        Assert.That(binaryOperation.Right, Is.EqualTo(new BinaryOperationOperandValue<string>("589")));
    }
    
    [Test]
    public void ParsingAdditionWithMultiplicationOfThreeIntegers_Should_Success()
    {
        var operandParser = Parse.SkipSpaces() << Parse.ManySatisfy(char.IsNumber) >> Parse.SkipSpaces();
        var subOperationStart = Parse.SkipSpaces() << Parse.StringMatch("(") >> Parse.SkipSpaces();
        var subOperationEnd = Parse.SkipSpaces() << Parse.StringMatch(")") >> Parse.SkipSpaces();
        
        var text = "1 + 25 * 589";
        
        var result = MathsParser.ParseAlgebraicExpression(subOperationStart, subOperationEnd,
            operandParser,
        [
            new Operator<string>(OperatorKind.Infix, "+", 10),
            new Operator<string>(OperatorKind.Infix, "-", 10),
            new Operator<string>(OperatorKind.Infix, "*", 20),
            new Operator<string>(OperatorKind.Infix, "/", 20),
        ]).Parse(text);
        
        Assert.That(result.Success, Is.True);
        var binaryOperation = (BinaryOperation<string>) result.Result!;
        Assert.That(result.Result, Is.Not.Null);
        
        Assert.That(binaryOperation.Left, Is.InstanceOf<BinaryOperationOperandValue<string>>());
        var leftOperation = (BinaryOperationOperandValue<string>)binaryOperation.Left;
        Assert.That(leftOperation.Value, Is.EqualTo("1"));
        Assert.That(binaryOperation.Operator, Is.EqualTo(new Operator<string>(OperatorKind.Infix,"+", 10)));
        
        Assert.That(binaryOperation.Right, Is.InstanceOf<BinaryOperation<string>>());
        
        var rightOperation = (BinaryOperation<string>)binaryOperation.Right;
        Assert.That(rightOperation.Left, Is.EqualTo(new BinaryOperationOperandValue<string>("25")));
        Assert.That(rightOperation.Right, Is.EqualTo(new BinaryOperationOperandValue<string>("589")));
        Assert.That(rightOperation.Operator, Is.EqualTo(new Operator<string>(OperatorKind.Infix,"*", 20)));
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
    //     var result = MathsParser.ParseAlgebraicExpression(text, operandParser, 
    //         subOperationStart, subOperationEnd,
    //         new Operator<string>(OperatorKind.Infix, addOperator, 1),
    //         new Operator<string>(OperatorKind.Infix, minusOperator, 1));
    //     
    //     Assert.That(result.Success, Is.True);
    //     Assert.That(result.Result, Is.EquivalentTo(new[] {"1", "+", "(", "25", "+", "589", ")"}));
    // }
    
    
}