using EasyParsing.Dsl;
using EasyParsing.Dsl.Linq;
using EasyParsing.Parsers;
using EasyParsing.Parsers.Maths;

namespace EasyParsing.Tests;

public class MathsExpressionsParserTests
{
    [Test]
    public void ParsingSimpleAdditionOfTwoSimpleIntegers_Should_Success()
    {
        var operandParser =
            from a in Parse.SkipSpaces()
            from n in Parse.ManySatisfy(char.IsNumber)
            from b in Parse.SkipSpaces()
            select new BinaryOperationOperandValue<string>(n);
        
        IParser<string> subOperationStart = Parse.SkipSpaces() << Parse.StringMatch("(") >> Parse.SkipSpaces();
        IParser<string> subOperationEnd = Parse.SkipSpaces() << Parse.StringMatch(")") >> Parse.SkipSpaces();

        var text = "1 + 25";

        var result = MathsParser.ParseAlgebraicExpression(operandParser,
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
        var operandParser =
            from a in Parse.SkipSpaces()
            from n in Parse.ManySatisfy(char.IsNumber)
            from b in Parse.SkipSpaces()
            select new BinaryOperationOperandValue<string>(n);
        
        var subOperationStart = Parse.SkipSpaces() << Parse.StringMatch("(") >> Parse.SkipSpaces();
        var subOperationEnd = Parse.SkipSpaces() << Parse.StringMatch(")") >> Parse.SkipSpaces();
        
        var text = "1 + 25 + 589";
        
        var result = MathsParser.ParseAlgebraicExpression(operandParser,
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
        var operandParser =
            from a in Parse.SkipSpaces()
            from n in Parse.ManySatisfy(char.IsNumber)
            from b in Parse.SkipSpaces()
            select new BinaryOperationOperandValue<string>(n);
        
        var subOperationStart = Parse.SkipSpaces() << Parse.StringMatch("(") >> Parse.SkipSpaces();
        var subOperationEnd = Parse.SkipSpaces() << Parse.StringMatch(")") >> Parse.SkipSpaces();
        
        var text = "1 * 25 + 589";
        
        var result = MathsParser.ParseAlgebraicExpression(operandParser,
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
        var operandParser =
            from a in Parse.SkipSpaces()
            from n in Parse.ManySatisfy(char.IsNumber)
            from b in Parse.SkipSpaces()
            select new BinaryOperationOperandValue<string>(n);
        
        var subOperationStart = Parse.SkipSpaces() << Parse.StringMatch("(") >> Parse.SkipSpaces();
        var subOperationEnd = Parse.SkipSpaces() << Parse.StringMatch(")") >> Parse.SkipSpaces();
        
        var text = "1 + 25 * 589";
        
        var result = MathsParser.ParseAlgebraicExpression(operandParser,
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
    
    
    [Test]
    public void ParsingAdditionWithMultiplicationOfThreeIntegers2_Should_Success()
    {
        var operandParser =
            from a in Parse.SkipSpaces()
            from n in Parse.ManySatisfy(char.IsNumber)
            from b in Parse.SkipSpaces()
            select new BinaryOperationOperandValue<string>(n);
        
        var text = "1 + 25 * 589 * (9 - 1)";
        
        var result = MathsParser.ParseAlgebraicExpression(operandParser,
        [
            new Operator<string>(OperatorKind.Infix, "+", 10),
            new Operator<string>(OperatorKind.Infix, "-", 10),
            new Operator<string>(OperatorKind.Infix, "*", 20),
            new Operator<string>(OperatorKind.Infix, "/", 20),
        ]).Parse(text);
        
        Assert.That(result.Success, Is.True);
        
        
    }
    
    [Test]
    public void ParsingMultiplicationWithParenthesizedAddition_Should_Success()
    {
        var operandParser =
            from _ in Parse.SkipSpaces()
            from n in Parse.ManySatisfy(char.IsNumber)
            from __ in Parse.SkipSpaces()
            select new BinaryOperationOperandValue<string>(n);

        var subOperationStart = Parse.SkipSpaces() << Parse.StringMatch("(") >> Parse.SkipSpaces();
        var subOperationEnd   = Parse.SkipSpaces() << Parse.StringMatch(")") >> Parse.SkipSpaces();

        var text = "(1 + 25) * 589";

        var parser = MathsParser.ParseAlgebraicExpression(
            operandParser,
            subOperationStart, subOperationEnd,
            [
                new Operator<string>(OperatorKind.Infix, "+", 10),
                new Operator<string>(OperatorKind.Infix, "-", 10),
                new Operator<string>(OperatorKind.Infix, "*", 20),
                new Operator<string>(OperatorKind.Infix, "/", 20),
            ]);

        var result = parser.Parse(text);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Result, Is.Not.Null);

        // racine = *
        var root = (BinaryOperation<string>)result.Result!;
        Assert.That(root.Operator, Is.EqualTo(new Operator<string>(OperatorKind.Infix, "*", 20)));

        // gauche = (1 + 25)
        Assert.That(root.Left, Is.InstanceOf<BinaryOperation<string>>());
        var left = (BinaryOperation<string>)root.Left;
        Assert.That(left.Operator, Is.EqualTo(new Operator<string>(OperatorKind.Infix, "+", 10)));
        Assert.That(left.Left, Is.EqualTo(new BinaryOperationOperandValue<string>("1")));
        Assert.That(left.Right, Is.EqualTo(new BinaryOperationOperandValue<string>("25")));

        // droite = 589
        Assert.That(root.Right, Is.EqualTo(new BinaryOperationOperandValue<string>("589")));
    }
    
    
}