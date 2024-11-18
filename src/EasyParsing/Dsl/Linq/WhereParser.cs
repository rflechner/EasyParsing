namespace EasyParsing.Dsl.Linq;

/// <summary>
/// Represents a parser that filters the parsing result based on a predicate.
/// </summary>
/// <typeparam name="T">The type of the element to parse.</typeparam>
public class WhereParser<T> : ParserBase<T>
{
    private readonly IParser<T> source;
    private readonly Func<T, bool> predicate;

    /// <summary>
    /// Represents a parser that filters the parsing result based on a predicate.
    /// </summary>
    /// <param name="source">The source of projection.</param>
    /// <param name="predicate">Predicate used to filter items.</param>
    public WhereParser(IParser<T> source, Func<T, bool> predicate)
    {
        this.source = source;
        this.predicate = predicate;
    }

    /// <summary>
    /// Parses the input based on the provided parsing context and applies a predicate to the result.
    /// </summary>
    /// <param name="context">The context used for parsing.</param>
    /// <returns>
    /// A parsing result containing the success status, the parsed result (if successful),
    /// and a failure message if the predicate is not satisfied.
    /// </returns>
    public override IParsingResult<T> Parse(ParsingContext context)
    {
        var result = source.Parse(context);
        if (!result.Success)
            return result;

        if (result.Result is null || !predicate(result.Result))
            return new ParsingResult<T>(result.Context, false, default, "Predicate not satisfied");

        return result;
    }
}