using EasyParsing.Dsl.Linq;

namespace EasyParsing.Parsers;

/// <summary>
/// Represents a parser that conditionally parses input using an inner parser and wraps the result in an Option type.
/// </summary>
/// <typeparam name="T">The type of the parsed value.</typeparam>
public class OptionnalParser<T>(IParser<T> innerParser) : IParser<Option<T>>
{
    /// <summary>
    /// Parses the given context using the inner parser and wraps the result in an Option type.
    /// If the parsing is successful, it returns a Some with the parsed result.
    /// If the parsing fails, it returns a None.
    /// </summary>
    /// <param name="context">The parsing context that contains the input data and position information.</param>
    /// <returns>An IParsingResult containing an Option which is either a Some with the parsed value or a None.</returns>
    public IParsingResult<Option<T>> Parse(ParsingContext context)
    {
        var result = innerParser.Parse(context);

        if (result.Success)
            return new ParsingResult<Option<T>>(result.Context, true, new Some<T>(result.Result!), result.FailureMessage);

        return new ParsingResult<Option<T>>(result.Context, true, new None<T>(), result.FailureMessage);
    }

    /// <summary>
    /// Provides a default value to use in place of a None result from the Option value.
    /// </summary>
    /// <param name="defaultValue">The value to use when the Option is None.</param>
    /// <returns>An IParser that provides the specified default value if the Option is None, or the original value if the Option is Some.</returns>
    public IParser<T?> DefaultWith(T defaultValue) => this.Select(r => r.GetValueOrDefault(defaultValue));
}