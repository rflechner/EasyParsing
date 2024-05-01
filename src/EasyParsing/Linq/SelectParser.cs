namespace EasyParsing.Linq;

public class SelectParser<TSource, TResult> : ParserBase<TResult>
{
    private readonly ParserBase<TSource> source;
    private readonly Func<TSource, TResult> selector;

    public SelectParser(ParserBase<TSource> source, Func<TSource, TResult> selector)
    {
        this.source = source;
        this.selector = selector;
    }

    public override ParsingResult<TResult> Parse(ParsingContext context)
    {
        var result = source.Parse(context);
        if (!result.Success)
            return new ParsingResult<TResult>(result.Context, false, default, result.FailureMessage);

        TResult mappedResult = selector(result.Result!);
        return new ParsingResult<TResult>(result.Context, true, mappedResult, null);
    }
}