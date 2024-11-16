namespace EasyParsing.Dsl.Linq;

/// <summary>
/// Represents a parser that applies a given transformation function to the result of another parser.
/// </summary>
/// <typeparam name="TSource">The type of the input result produced by the source parser.</typeparam>
/// <typeparam name="TResult">The type of the result after applying the transformation function.</typeparam>
public class SelectParser<TSource, TResult> : ParserBase<TResult>
{
    private readonly IParser<TSource> source;
    private readonly Func<TSource, TResult> selector;

    /// <summary>
    /// Represents a parser that applies a given transformation function to the result of another parser.
    /// </summary>
    public SelectParser(IParser<TSource> source, Func<TSource, TResult> selector)
    {
        this.source = source;
        this.selector = selector;
    }

    /// <summary>
    /// Parses the input using the source parser and applies a transformation function to the result.
    /// </summary>
    /// <param name="context">The context of the parsing operation, including the input and position.</param>
    /// <returns>The result of the parsing operation after applying the transformation function.</returns>
    public override IParsingResult<TResult> Parse(ParsingContext context)
    {
        var result = source.Parse(context);
        if (!result.Success || result.Result == null)
            return Fail(context, result.FailureMessage!);

        TResult mappedResult = selector(result.Result);
        
        return Success(result.Context, mappedResult);
    }
}