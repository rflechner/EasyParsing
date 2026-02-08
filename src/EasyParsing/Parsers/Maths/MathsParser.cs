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
/// <param name="Precedence">
/// The priority level of the operator, used for determining the order of operations
/// during expression evaluation.
/// </param>
public record Operator<TToken>(OperatorKind Kind, string Text, int Precedence)
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
/// <param name="Value">
/// The token value representing the left operand in a binary operation.
/// This value is used as part of the expression evaluation process.
/// </param>
public record BinaryOperationOperandValue<TToken>(TToken Value) : BinaryOperationOperand<TToken>;

/// <summary>
/// Represents a parser for mathematical expressions.
/// </summary>
public class MathsParser
{

    /// <summary>
    /// Creates a parser for algebraic expressions composed of operands and operators.
    /// </summary>
    /// <param name="subOperationStart"></param>
    /// <param name="subOperationEnd"></param>
    /// <param name="operandParser"></param>
    /// <param name="ops"></param>
    /// <param name="minPrec"></param>
    /// <typeparam name="TToken"></typeparam>
    /// <returns></returns>
    public static IParser<BinaryOperationOperand<TToken>> ParseAlgebraicExpression<TToken>(
        IParser<string> subOperationStart,
        IParser<string> subOperationEnd,
        IParser<TToken> operandParser,
        Operator<string>[] ops,
        int minPrec = 0)
    {
        return new AlgebraicExpressionParser<TToken>(subOperationStart, subOperationEnd, operandParser, ops, minPrec);
    }
    
}