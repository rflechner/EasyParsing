namespace EasyParsing;

/// <summary>
/// Serves as the base class for implementing parsers.
/// </summary>
/// <typeparam name="T">The type of the result produced by the parser.</typeparam>
public abstract class ParserBase<T> : IParser<T>
{
    /// <summary>
    /// Returns a failed parsing result with the specified context and failure message.
    /// </summary>
    /// <param name="context">The current parsing context.</param>
    /// <param name="message">The failure message to include in the result.</param>
    /// <returns>A parsing result indicating failure, with the given context and failure message.</returns>
    protected IParsingResult<T> Fail(ParsingContext context, string message)
    {
        return new ParsingResult<T>(context, false, default, message);
    }

    /// <summary>
    /// Returns a successful parsing result with the specified context and result.
    /// </summary>
    /// <param name="context">The current parsing context.</param>
    /// <param name="result">The result of the parsing operation.</param>
    /// <returns>A parsing result indicating success, with the given context and result.</returns>
    protected IParsingResult<T> Success(ParsingContext context, T result)
    {
        return new ParsingResult<T>(context, true, result, default);
    }

    /// <summary>
    /// Attempts to parse the input within the given parsing context.
    /// </summary>
    /// <param name="context">The current parsing context containing the input to parse and the position within the input.</param>
    /// <returns>An object representing the result of the parse attempt, including success status, the resulting value if successful, and any failure message if not.</returns>
    public abstract IParsingResult<T> Parse(ParsingContext context);
    
}