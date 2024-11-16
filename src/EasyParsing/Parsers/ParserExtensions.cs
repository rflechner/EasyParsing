using EasyParsing.Dsl.Linq;

namespace EasyParsing.Parsers;

/// <summary>
/// Parsers extensions
/// </summary>
public static class ParserExtensions
{
    /// <summary>
    /// Combines two parsers into one new parser that parses both sequentially.
    /// </summary>
    /// <typeparam name="TIn1">The type of the input for the left parser.</typeparam>
    /// <typeparam name="TIn2">The type of the input for the right parser.</typeparam>
    /// <param name="left">The left parser to combine.</param>
    /// <param name="right">The right parser to combine.</param>
    /// <returns>A new parser that combines the left and right parsers.</returns>
    public static CombineParser<TIn1, TIn2> Combine<TIn1, TIn2>(this IParser<TIn1> left, IParser<TIn2> right)
    {
        return new CombineParser<TIn1, TIn2>(left, right);
    }

    /// <summary>
    /// Combines two parsers into one new parser that parses both sequentially, ignoring the result of the right parser.
    /// </summary>
    /// <typeparam name="TIn1">The type of the input for the left parser.</typeparam>
    /// <typeparam name="TIn2">The type of the input for the right parser.</typeparam>
    /// <param name="left">The left parser to combine.</param>
    /// <param name="right">The right parser to combine.</param>
    /// <returns>A new parser that combines the left and right parsers and returns the result of the left parser.</returns>
    public static IParser<TIn1> ThenIgnore<TIn1, TIn2>(this IParser<TIn1> left, IParser<TIn2> right)
    {
        return new CombineParser<TIn1, TIn2>(left, right).Select(r => r.Item1);
    }

    /// <summary>
    /// Casts the output type of the parser from TIn to TOut.
    /// </summary>
    /// <param name="input">The parser whose output type needs to be casted.</param>
    /// <typeparam name="TIn">The input type of the parser which is being casted.</typeparam>
    /// <typeparam name="TOut">The target type to cast the output of the parser to.</typeparam>
    /// <returns>A new parser that casts the output from TIn to TOut.</returns>
    public static IParser<TOut> Cast<TIn, TOut>(this IParser<TIn> input) where TIn : TOut
    {
        return new SelectParser<TIn, TOut>(input, @in => (TOut) @in);
    }

    /// <summary>
    /// Repeatedly parses using an inner parser until a specified limiter text is encountered.
    /// </summary>
    /// <typeparam name="T">The type of the values parsed by the inner parser.</typeparam>
    /// <param name="parser">The inner parser to use repeatedly.</param>
    /// <param name="text">The text limiter that determines when parsing stops.</param>
    /// <param name="skipDelimiter">Indicates whether the delimiter text should be skipped.</param>
    /// <returns>A parser that runs the inner parser until the specified text is encountered.</returns>
    public static IParser<T> Until<T>(this IParser<T> parser, string text, bool skipDelimiter = true)
    {
        return new UntilTextParser<T>(parser, text, skipDelimiter);
    }

    /// <summary>
    /// Parses content until the specified limit parser matches, providing both the parsed content and the matched limit.
    /// </summary>
    /// <typeparam name="T1">The type of the result from the inner parser.</typeparam>
    /// <typeparam name="T2">The type of the result from the limit parser.</typeparam>
    /// <param name="innerParser">The parser used to parse the main content.</param>
    /// <param name="limitParser">The parser used to identify the limit point.</param>
    /// <returns>A new parser that parses until the limit parser succeeds, returning a tuple containing the results of both parsers.</returns>
    public static IParser<(T1, T2)> Until<T1, T2>(this IParser<T1> innerParser, IParser<T2> limitParser) => new UntilParser<T1, T2>(innerParser, limitParser);

    /// <summary>
    /// Creates a parser that attempts to match the input using the first parser.
    /// If the first parser fails, it attempts to match using the second parser.
    /// </summary>
    /// <typeparam name="T">The type of the parsed output.</typeparam>
    /// <param name="left">The primary parser to try first.</param>
    /// <param name="right">The secondary parser to try if the primary fails.</param>
    /// <returns>A new parser that tries the primary parser first and the secondary parser if the primary fails.</returns>
    public static IParser<T> Or<T>(this IParser<T> left, IParser<T> right)
    {
        return new OrElseParser<T>([left, right]);
    }

    /// <summary>
    /// Represents an OR parser that attempts to parse using the left parser
    /// or the other parsers, and returns the result of the first successful parse.
    /// </summary>
    /// <typeparam name="T">The type of the value that the parser returns.</typeparam>
    /// <param name="left">The left parser to try first.</param>
    /// <param name="parsers">The other parsers to try if the left parser fails.</param>
    /// <returns>A parser that attempts to use the left parser first, and the right parser if the left parser fails.</returns>
    public static IParser<T> Or<T>(this IParser<T> left, IParser<T>[] parsers)
    {
        return new OrElseParser<T>(parsers.Prepend(left).ToArray());
    }
}