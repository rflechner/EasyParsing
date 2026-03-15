namespace EasyParsing.Parsers;

/// <summary>
/// Represents a parser that lazily evaluates another parser using a factory function.
/// This is useful for recursive parsers where the parser needs to reference itself.
/// </summary>
/// <typeparam name="T">The type of the parsed value.</typeparam>
public class LazyParser<T>(Func<IParser<T>> parserFactory) : ParserBase<T>
{
    private IParser<T>? _parser;

    /// <summary>
    /// Parses the given context by lazily invoking the parser factory and delegating to the resulting parser.
    /// </summary>
    /// <param name="context">The parsing context that contains the input data and position information.</param>
    /// <returns>The result of parsing using the lazily-created parser.</returns>
    public override IParsingResult<T> Parse(ParsingContext context)
    {
        _parser ??= parserFactory();
        return _parser.Parse(context);
    }
}
