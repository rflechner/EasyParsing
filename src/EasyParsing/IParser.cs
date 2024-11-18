using EasyParsing.Dsl.Linq;
using EasyParsing.Parsers;

namespace EasyParsing;

/// <summary>
/// Defines a parser that parses input within a given parsing context and returns a parsing result.
/// </summary>
/// <typeparam name="T">The type of the result produced by the parser.</typeparam>
public interface IParser<out T>
{
    /// <summary>
    /// Parses the given context and returns the parsing result.
    /// </summary>
    /// <param name="context">The context containing the input to be parsed and the current parsing position.</param>
    /// <returns>The result of the parsing operation, encapsulated in an <see cref="IParsingResult{T}"/> instance.</returns>
    IParsingResult<T> Parse(ParsingContext context);

    /// <summary>
    /// Combines two parsers using the logical OR operation. The resulting parser
    /// will try to parse the input using the left parser first, and if the left
    /// parser fails, it will then try the right parser.
    /// </summary>
    /// <param name="left">The first parser to attempt.</param>
    /// <param name="right">The second parser to attempt if the first one fails.</param>
    /// <returns>A new parser that represents the combination of the two given parsers.</returns>
    public static IParser<T> operator |(IParser<T> left, IParser<T> right)
    {
        return new OrElseParser<T>([left, right]);
    }

    /// <summary>
    /// Defines a parser that two parsers and return a tuple of both results.
    /// </summary>
    /// <param name="left">The left parser to be combined.</param>
    /// <param name="right">The right parser to be combined.</param>
    public static IParser<(T,T)> operator +(IParser<T> left, IParser<T> right)
    {
        return new CombineParser<T, T>(left, right);
    }

    /// <summary>
    /// Combines two parsers and ignore result of second parser.
    /// </summary>
    /// <param name="left">The left parser to be combined.</param>
    /// <param name="right">The right parser to be combined.</param>
    /// <returns>A new parser that parses with both the left and right parsers, returning the result of the left parser.</returns>
    public static IParser<T> operator >>(IParser<T> left, IParser<T> right)
    {
        return new CombineParser<T, T>(left, right).Select(tuple => tuple.Item1);
    }

    /// <summary>
    /// Combines two parsers and ignore result of first parser.
    /// </summary>
    /// <param name="left">The left parser to be combined.</param>
    /// <param name="right">The right parser to be combined.</param>
    /// <returns>A new parser that parses with both the left and right parsers, returning the result of the right parser.</returns>
    public static IParser<T> operator <<(IParser<T> left, IParser<T> right)
    {
        return new CombineParser<T, T>(left, right).Select(tuple => tuple.Item2);
    }
}