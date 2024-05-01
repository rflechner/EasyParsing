namespace EasyParsing.Linq;

public class SelectManyParser<TFirst, TSecond, TResult> : ParserBase<TResult>
{
    private readonly ParserBase<TFirst> first;
    private readonly Func<TFirst, ParserBase<TSecond>> secondSelector;
    private readonly Func<TFirst, TSecond, TResult> resultSelector;

    public SelectManyParser(
        ParserBase<TFirst> first,
        Func<TFirst, ParserBase<TSecond>> secondSelector,
        Func<TFirst, TSecond, TResult> resultSelector)
    {
        this.first = first;
        this.secondSelector = secondSelector;
        this.resultSelector = resultSelector;
    }

    public override ParsingResult<TResult> Parse(ParsingContext context)
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