namespace EasyParsing.Parsers.Maths;

/// <summary>
/// Represents the kind of an operator.
/// </summary>
public enum OperatorKind
{
    /// <summary>
    /// Represents an operator that is placed between its operands in an expression.
    /// </summary>
    /// <remarks>
    /// Commonly used for binary operations such as addition (e.g., "A + B"), subtraction,
    /// multiplication, or division. An infix operator expects two operands with the operator
    /// positioned between them.
    /// </remarks>
    Infix,

    /// <summary>
    /// Represents an operator that is placed before its operand in an expression.
    /// </summary>
    /// <remarks>
    /// Commonly used for unary operations where the operator precedes the single operand,
    /// such as negation (e.g., "-A") or increment (e.g., "++A") in programming or mathematical expressions.
    /// </remarks>
    Prefix,

    /// <summary>
    /// Represents an operator that is placed after its operand in an expression.
    /// </summary>
    /// <remarks>
    /// Commonly used in expressions where the operator operates on a single operand
    /// and the operator is positioned immediately following the operand. Examples include
    /// post-increment (e.g., "A++") or post-decrement (e.g., "A--") operations.
    /// </remarks>
    Postfix
}

/// <summary>
/// Represents an operator used within a mathematical parser. This record encapsulates
/// the details about the operator's kind, the parser used to match the operator in an
/// expression, and its priority level for processing within expressions.
/// </summary>
/// <typeparam name="TToken">The type of tokens that the associated parser operates on.</typeparam>
/// <param name="Kind">
/// The kind of operator, indicating its position relative to operands
/// (infix, prefix, or postfix).
/// </param>
/// <param name="Parser">
/// The parser used for identifying the operator within a tokenized
/// mathematical expression.
/// </param>
/// <param name="Priority">
/// The priority level of the operator, used for determining the order of operations
/// during expression evaluation.
/// </param>
public record Operator<TToken>(OperatorKind Kind, IParser<TToken> Parser, int Priority);

/// <summary>
/// Represents a parser for mathematical expressions.
/// </summary>
public class MathsParser
{
    /// <summary>
    /// Tokenize an algebraic expression.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="operandParser"></param>
    /// <param name="operators"></param>
    /// <typeparam name="TToken"></typeparam>
    /// <returns></returns>
    public static IParsingResult<TToken[]> TokenizeAlgebraicExpression<TToken>(string text, 
        IParser<TToken> operandParser, 
        params Operator<TToken>[] operators)
    {
        var results = new List<IParsingResult<TToken>>();
        
        var result = operandParser.Parse(text);
        if (!result.Success) return new ParsingResult<TToken[]>(result.Context, false, null, result.FailureMessage);
        
        results.Add(result);
        var context = result.Context;
        
        while (context.Remaining.Length > 0)
        {
            foreach (var op in operators)
            {
                var opResult = op.Parser.Parse(context);
                if (!opResult.Success) continue;
                results.Add(opResult);
                context = opResult.Context;
            }
            
            result = operandParser.Parse(context);
            if (!result.Success) break;
        
            results.Add(result);
            context = result.Context;
        }

        if (results.Count == 0) return new ParsingResult<TToken[]>(result.Context, false, null, "No tokens found");
        
        return new ParsingResult<TToken[]>(result.Context, result.Success, results.Select(t => t.Result!).ToArray(), result.FailureMessage);
    }
}