using EasyParsing.Dsl;

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
/// <param name="Text">
/// The raw text representation of the operator, used for matching the operator in an expression.
/// </param>
/// <param name="Priority">
/// The priority level of the operator, used for determining the order of operations
/// during expression evaluation.
/// </param>
public record Operator<TToken>(OperatorKind Kind, string Text, int Priority)
{
    /// <summary>
    /// The parser used to match the operator in an expression.
    /// </summary>
    public IParser<string> Parser => Parse.SkipSpaces() << Parse.StringMatch(Text) >> Parse.SkipSpaces();
}

/// <summary>
/// Represents a base abstraction for an operand used in binary operations within a mathematical parser.
/// This record serves as the foundational type for all operands that are part of binary mathematical
/// expressions, providing a common structure for operations involving two operands.
/// </summary>
/// <typeparam name="TToken">
/// The type of tokens that the operand is associated with, typically representing
/// the atomic units of a parsed mathematical expression.
/// </typeparam>
public abstract record BinaryOperationOperand<TToken>;

/// <summary>
/// Represents a binary operation within a mathematical expression, composed of a left operand,
/// a right operand, and an operator connecting them. This record encapsulates the details
/// necessary for parsing and evaluating binary operations in mathematical expressions.
/// </summary>
/// <typeparam name="TToken">The type of tokens used to describe operands and operators.</typeparam>
/// <param name="Left">
/// The left operand of the binary operation, potentially formed of other operations or values.
/// </param>
/// <param name="Right">
/// The right operand of the binary operation, potentially formed of other operations or values.
/// </param>
/// <param name="Operator">
/// The operator defining the relationship between the left and right operands,
/// including information such as its kind and priority in the mathematical expression.
/// </param>
public record BinaryOperation<TToken>(
    BinaryOperationOperand<TToken> Left,
    BinaryOperationOperand<TToken> Right,
    Operator<string> Operator) : BinaryOperationOperand<TToken>;

/// <summary>
/// Represents a value operand in a binary operation within a mathematical parser.
/// This record serves as a concrete implementation of the base class
/// <see cref="BinaryOperationOperand{TToken}"/>, holding the value of the operand.
/// </summary>
/// <typeparam name="TToken">
/// The type of token that this operand holds, typically used in the parsing context
/// to identify or process a specific atomic unit of an expression.
/// </typeparam>
/// <param name="Left">
/// The token value representing the left operand in a binary operation.
/// This value is used as part of the expression evaluation process.
/// </param>
public record BinaryOperationOperandValue<TToken>(TToken Left) : BinaryOperationOperand<TToken>;

/// <summary>
/// Represents a parser for mathematical expressions.
/// </summary>
public class MathsParser
{
    /// <summary>
    /// Parses an algebraic expression into a binary operation structure.
    /// </summary>
    /// <param name="context">The algebraic expression to parse.</param>
    /// <param name="operandParser">The parser used to parse individual operands in the expression.</param>
    /// <param name="subOperationStart">The parser used to identify the start of a sub-operation in the expression.</param>
    /// <param name="subOperationEnd">The parser used to identify the end of a sub-operation in the expression.</param>
    /// <param name="operators">The set of operators used in the algebraic expression.</param>
    /// <typeparam name="TToken">The type of token produced by the operand and operator parsers.</typeparam>
    /// <returns>An object representing the parsed result of the algebraic expression, or an error indication if parsing fails.</returns>
    public static IParsingResult<BinaryOperation<TToken>> ParseAlgebraicExpression<TToken>(ParsingContext context,
        IParser<TToken> operandParser,
        IParser<string> subOperationStart, IParser<string> subOperationEnd,
        params Operator<string>[] operators)
    {
        var leftResult = operandParser.Parse(context);
        if (!leftResult.Success)
            return new ParsingResult<BinaryOperation<TToken>>(leftResult.Context, false, null, leftResult.FailureMessage);

        IParsingResult<Operator<string>> parseOperatorResult = TryParseOperator(leftResult.Context, operators);
        if (!parseOperatorResult.Success)
            return new ParsingResult<BinaryOperation<TToken>>(leftResult.Context, false, null, "No operator found");
        
        var rightResult = operandParser.Parse(parseOperatorResult.Context);
        if (!rightResult.Success)
            return new ParsingResult<BinaryOperation<TToken>>(rightResult.Context, false, null, rightResult.FailureMessage);

        BinaryOperationOperand<TToken> right = new BinaryOperationOperandValue<TToken>(rightResult.Result!);
        var nextResult = ParseAlgebraicExpression(parseOperatorResult.Context, operandParser, subOperationStart, subOperationEnd, operators);
        if (nextResult is { Success: true }) 
            right = nextResult.Result!;
        
        return new ParsingResult<BinaryOperation<TToken>>(
            rightResult.Context, rightResult.Success, 
            new BinaryOperation<TToken>(
                new BinaryOperationOperandValue<TToken>(leftResult.Result!), 
                right, 
                parseOperatorResult.Result!), 
            rightResult.FailureMessage);
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