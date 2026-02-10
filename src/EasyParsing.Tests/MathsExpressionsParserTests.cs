using EasyParsing.Dsl;
using EasyParsing.Dsl.Linq;
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

        // root = *
        var root = (BinaryOperation<string>)result.Result!;
        Assert.That(root.Operator, Is.EqualTo(new Operator<string>(OperatorKind.Infix, "*", 20)));

        // left = (1 + 25)
        Assert.That(root.Left, Is.InstanceOf<BinaryOperation<string>>());
        var left = (BinaryOperation<string>)root.Left;
        Assert.That(left.Operator, Is.EqualTo(new Operator<string>(OperatorKind.Infix, "+", 10)));
        Assert.That(left.Left, Is.EqualTo(new BinaryOperationOperandValue<string>("1")));
        Assert.That(left.Right, Is.EqualTo(new BinaryOperationOperandValue<string>("25")));

        // right = 589
        Assert.That(root.Right, Is.EqualTo(new BinaryOperationOperandValue<string>("589")));
    }
    
    [Test]
    public void ParsingAdditionWithMultiplicationOfMultipleIntegers_Should_Success()
    {
        var operandParser =
            from a in Parse.SkipSpaces()
            from n in Parse.ManySatisfy(char.IsNumber)
            from b in Parse.SkipSpaces()
            select new BinaryOperationOperandValue<string>(n);
        
        var text = "1 + 25 * 589 * (9 - 1)";
        
        var subOperationStart = Parse.SkipSpaces() << Parse.StringMatch("(") >> Parse.SkipSpaces();
        var subOperationEnd   = Parse.SkipSpaces() << Parse.StringMatch(")") >> Parse.SkipSpaces();
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
        var binaryOperation = (BinaryOperation<string>) result.Result!;
        Assert.That(result.Result, Is.Not.Null);

        // root = +
        Assert.That(binaryOperation.Operator, Is.EqualTo(new Operator<string>(OperatorKind.Infix,"+", 10)));

        // left = 1
        Assert.That(binaryOperation.Left, Is.InstanceOf<BinaryOperationOperandValue<string>>());
        var leftOperation = (BinaryOperationOperandValue<string>)binaryOperation.Left;
        Assert.That(leftOperation.Value, Is.EqualTo("1"));

        // right = (25 * 589) * (9 - 1)
        Assert.That(binaryOperation.Right, Is.InstanceOf<BinaryOperation<string>>());
        var rightOperation = (BinaryOperation<string>)binaryOperation.Right;

        // right.Operator = *
        Assert.That(rightOperation.Operator, Is.EqualTo(new Operator<string>(OperatorKind.Infix,"*", 20)));

        // right.left = 25 * 589
        Assert.That(rightOperation.Left, Is.InstanceOf<BinaryOperation<string>>());
        var rightLeftOperation = (BinaryOperation<string>)rightOperation.Left;
        Assert.That(rightLeftOperation.Operator, Is.EqualTo(new Operator<string>(OperatorKind.Infix,"*", 20)));
        Assert.That(rightLeftOperation.Left, Is.EqualTo(new BinaryOperationOperandValue<string>("25")));
        Assert.That(rightLeftOperation.Right, Is.EqualTo(new BinaryOperationOperandValue<string>("589")));

        // right.right = (9 - 1)
        Assert.That(rightOperation.Right, Is.InstanceOf<BinaryOperation<string>>());
        var subOperation = (BinaryOperation<string>)rightOperation.Right;
        Assert.That(subOperation.Operator, Is.EqualTo(new Operator<string>(OperatorKind.Infix,"-", 10)));
        Assert.That(subOperation.Left, Is.EqualTo(new BinaryOperationOperandValue<string>("9")));
        Assert.That(subOperation.Right, Is.EqualTo(new BinaryOperationOperandValue<string>("1")));
    }

    [TestCase("(9 - 1)", (9 - 1))]
    [TestCase("1 + 25 * 589 * (9 - 1)", 1 + 25 * 589 * (9 - 1))]
    [TestCase("1 + 25 / 589 * (9 - 1)", 1 + 25 / 589 * (9 - 1))]
    [TestCase("1021 / 7 + 2 * ((7-2) * 3)", 1021 / 7 + 2 * ((7-2) * 3))]
    public void EvaluatingParsedExpression_Should_ReturnCorrectResult(string text, int expectedResult)
    {
        var operandParser =
            from _ in Parse.SkipSpaces()
            from n in Parse.ManySatisfy(char.IsNumber)
            from __ in Parse.SkipSpaces()
            select new BinaryOperationOperandValue<int>(int.Parse(n));

        var subOperationStart = Parse.SkipSpaces() << Parse.StringMatch("(") >> Parse.SkipSpaces();
        var subOperationEnd   = Parse.SkipSpaces() << Parse.StringMatch(")") >> Parse.SkipSpaces();

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

        var computationResult = Evaluate(result.Result!);

        Assert.That(computationResult, Is.EqualTo(expectedResult));
    }

    private static int Evaluate(BinaryOperationOperand<int> operand)
    {
        return operand switch
        {
            BinaryOperationOperandValue<int> value => value.Value,
            BinaryOperation<int> operation => operation.Operator.Text switch
            {
                "+" => Evaluate(operation.Left) + Evaluate(operation.Right),
                "-" => Evaluate(operation.Left) - Evaluate(operation.Right),
                "*" => Evaluate(operation.Left) * Evaluate(operation.Right),
                "/" => Evaluate(operation.Left) / Evaluate(operation.Right),
                _ => throw new InvalidOperationException($"Unknown operator: {operation.Operator.Text}")
            },
            _ => throw new InvalidOperationException($"Unknown operand type: {operand.GetType()}")
        };
    }
}