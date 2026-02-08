namespace EasyParsing.Parsers.Maths;

/// <summary>
/// Represents a parser that parses algebraic expressions.
/// </summary>
/// <param name="subOperationStart"></param>
/// <param name="subOperationEnd"></param>
/// <param name="operandParser"></param>
/// <param name="ops"></param>
/// <param name="minPrec"></param>
/// <typeparam name="TToken"></typeparam>
public class AlgebraicExpressionParser<TToken>(
    IParser<string> subOperationStart,
    IParser<string> subOperationEnd,
    IParser<TToken> operandParser,
    Operator<string>[] ops,
    int minPrec = 0) : ParserBase<BinaryOperationOperand<TToken>>
{
    /// <summary>
    /// Parse an algebraic expression.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override IParsingResult<BinaryOperationOperand<TToken>> Parse(ParsingContext context)
    {
        return ParseAlgebraicExpression(context, subOperationStart, subOperationEnd, operandParser, ops, minPrec);
    }
    
    /// <summary>
    /// Parse an expression with operators precedence.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="subOperationEnd"></param>
    /// <param name="operandParser"></param>
    /// <param name="ops"></param>
    /// <param name="minPrec"></param>
    /// <param name="subOperationStart"></param>
    /// <returns></returns>
    private static IParsingResult<BinaryOperationOperand<TToken>> ParseAlgebraicExpression(
        ParsingContext context,
        IParser<string> subOperationStart,
        IParser<string> subOperationEnd,
        IParser<TToken> operandParser,
        Operator<string>[] ops,
        int minPrec = 0)
    {
        //Parse.Between(subOperationStart, ParseAlgebraicExpression<TToken>(), subOperationEnd)
        
        // 1) parse left (operand ou sous-expression)
        var leftRes = operandParser.Parse(context);
        if (!leftRes.Success)
            return new ParsingResult<BinaryOperationOperand<TToken>>(context, false, null, leftRes.FailureMessage);

        BinaryOperationOperand<TToken> left = new BinaryOperationOperandValue<TToken>(leftRes.Result!);
        var ctx = leftRes.Context;

        // 2) boucle opérateurs
        while (true)
        {
            var parseOperatorResult = TryParseOperator(ctx, ops);
            if (!parseOperatorResult.Success) break;

            if (parseOperatorResult.Result!.Precedence < minPrec) break;

            // 3) parse right avec seuil
            var rightRes = 
                ParseAlgebraicExpression(parseOperatorResult.Context,
                    subOperationStart, subOperationEnd,
                operandParser, ops, parseOperatorResult.Result!.Precedence + 1);
            if (!rightRes.Success)
                return new ParsingResult<BinaryOperationOperand<TToken>>(ctx, false, null, rightRes.FailureMessage);

            left = new BinaryOperation<TToken>(left, rightRes.Result!, parseOperatorResult.Result!);
            ctx = rightRes.Context;
        }

        return new ParsingResult<BinaryOperationOperand<TToken>>(ctx, true, left, null);
    }
    
    private static IParsingResult<Operator<string>> TryParseOperator(ParsingContext context, Operator<string>[] operators)
    {
        foreach (var op in operators)
        {
            var opResult = op.Parser.Parse(context);
            if (!opResult.Success) continue;
            
            return new ParsingResult<Operator<string>>(opResult.Context, true, op, null);
        }

        return new ParsingResult<Operator<string>>(context, false, null, "No operator found");
    }
}