namespace EasyParsing.Linq;

public class SelectManyParser<TFirst, TSecond, TResult> : IParser<TResult>
{
    private readonly IParser<TFirst> first;
    private readonly Func<TFirst, IParser<TSecond>> secondSelector;
    private readonly Func<TFirst, TSecond, TResult> resultSelector;

    public SelectManyParser(
        IParser<TFirst> first,
        Func<TFirst, IParser<TSecond>> secondSelector,
        Func<TFirst, TSecond, TResult> resultSelector)
    {
        this.first = first;
        this.secondSelector = secondSelector;
        this.resultSelector = resultSelector;
    }

    public ParsingResult<TResult> Parse(ParsingContext context)
    {
        var firstResult = first.Parse(context);
        if (!firstResult.Success)
            return new ParsingResult<TResult>(firstResult.Context, false, default, firstResult.FailureMessage);
        
        var secondParser = secondSelector(firstResult.Result!);
        var secondResult = secondParser.Parse(firstResult.Context);
        if (!secondResult.Success)
            return new ParsingResult<TResult>(secondResult.Context, false, default, secondResult.FailureMessage);
        
        var result = resultSelector(firstResult.Result!, secondResult.Result!);
        return new ParsingResult<TResult>(secondResult.Context, true, result, null);
    }
}