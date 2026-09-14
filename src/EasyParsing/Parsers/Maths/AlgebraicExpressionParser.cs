using EasyParsing.Dsl.Linq;

namespace EasyParsing.Parsers.Maths;

/// <summary>
/// Represents a parser that parses algebraic expressions.
/// </summary>
/// <typeparam name="TToken"></typeparam>
public class AlgebraicExpressionParser<TToken> : ParserBase<BinaryOperationOperand<TToken>>
{
    private readonly IParser<BinaryOperationOperand<TToken>> _primary;
    private readonly Operator<string>[] _ops;
    private readonly int _minPrec;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="primary"></param>
    /// <param name="ops"></param>
    /// <param name="minPrec"></param>
    public AlgebraicExpressionParser(
        IParser<BinaryOperationOperand<TToken>> primary,
        Operator<string>[] ops,
        int minPrec = 0)
    {
        _primary = primary;
        _ops = ops;
        _minPrec = minPrec;
    }

    /// <summary>
    /// Parse an algebraic expression.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override IParsingResult<BinaryOperationOperand<TToken>> Parse(ParsingContext context)
    {
        var result = ParseAlgebraicExpression(context, _primary, _ops, _minPrec);
        if (result.Result is not BinaryOperation<TToken>)
            return new ParsingResult<BinaryOperationOperand<TToken>>(context, false, null, "No operator found");

        return result;
    }

    private static IParsingResult<BinaryOperationOperand<TToken>> ParseAlgebraicExpression(
        ParsingContext context,
        IParser<BinaryOperationOperand<TToken>> primaryParser,
        Operator<string>[] ops,
        int minPrec = 0)
    {
        // 1) parse left (operand OU sous-expression parenthésée)
        var leftRes = primaryParser.Parse(context);
        if (!leftRes.Success)
            return new ParsingResult<BinaryOperationOperand<TToken>>(context, false, null, leftRes.FailureMessage);

        var left = leftRes.Result!;
        var ctx = leftRes.Context;

        // 2) boucle opérateurs (Pratt)
        while (true)
        {
            var opRes = TryParseOperator(ctx, ops);
            if (!opRes.Success) break;

            var op = opRes.Result!;
            if (op.Precedence < minPrec) break;

            // 3) right avec seuil (assoc gauche => +1)
            var rightRes = ParseAlgebraicExpression(opRes.Context, primaryParser, ops, op.Precedence + 1);
            if (!rightRes.Success)
                return new ParsingResult<BinaryOperationOperand<TToken>>(ctx, false, null, rightRes.FailureMessage);

            left = new BinaryOperation<TToken>(left, rightRes.Result!, op);
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

    /// <summary>
    /// Creates a parser that parses algebraic expressions.
    /// </summary>
    /// <param name="lparen"></param>
    /// <param name="rparen"></param>
    /// <param name="operandParser"></param>
    /// <param name="ops"></param>
    /// <returns></returns>
    public static IParser<BinaryOperationOperand<TToken>> Create(
        IParser<string> lparen,
        IParser<string> rparen,
        IParser<TToken> operandParser,
        Operator<string>[] ops)
    {
        // expr est récursif => Lazy
        LazyParser<BinaryOperationOperand<TToken>> expr = null!;

        // operand => wrap en BinaryOperationOperandValue
        var atom = operandParser.Select(v => new BinaryOperationOperandValue<TToken>(v));
        
        // Mais on ne peut pas référencer `primary` avant de l’avoir.
        // Donc on construit en deux temps avec une variable.
        IParser<BinaryOperationOperand<TToken>> primary = null!;

        // ( expr )
        // Si Between renvoie le "items" directement, c’est bon.
        // Sinon, mappe pour extraire l’élément central.
        IParser<BinaryOperationOperand<TToken>> parenExpr =
            Dsl.Parse.Between(lparen, expr = new LazyParser<BinaryOperationOperand<TToken>>(
                () => new AlgebraicExpressionParser<TToken>(primary: new LazyParser<BinaryOperationOperand<TToken>>(() => primary), ops: ops)), rparen)
                .Select(r => r.Item);
        
        // Recrée expr maintenant que primary existe :
        expr = new LazyParser<BinaryOperationOperand<TToken>>(() => new AlgebraicExpressionParser<TToken>(primary, ops));

        parenExpr = Dsl.Parse.Between(lparen, expr, rparen).Select(r => r.Item);

        // primary = parens OR atom
        primary = parenExpr | atom;

        // on retourne expr (le vrai parseur d'expression)
        return expr;
    }
}
