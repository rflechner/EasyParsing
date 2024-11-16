namespace EasyParsing.Dsl.Linq;

/// <summary>
/// The SelectManyParser class is a parser that parses a sequence of elements by combining two parsers.
/// </summary>
/// <typeparam name="TFirst">The type of the first parser's result.</typeparam>
/// <typeparam name="TSecond">The type of the second parser's result.</typeparam>
/// <typeparam name="TResult">The type of the final combined result.</typeparam>
public class SelectManyParser<TFirst, TSecond, TResult> : IParser<TResult>
{
    private readonly IParser<TFirst> first;
    private readonly Func<TFirst, IParser<TSecond>> secondSelector;
    private readonly Func<TFirst, TSecond, TResult> resultSelector;

    /// <summary>
    /// The SelectManyParser class is a parser that parses a sequence of elements by combining two parsers.
    /// </summary>
    /// <param name="first">The first parser's result.</param>
    /// <param name="secondSelector">The second parser's selector.</param>
    /// <param name="resultSelector">The selector used to build projection.</param>
    public SelectManyParser(
        IParser<TFirst> first,
        Func<TFirst, IParser<TSecond>> secondSelector,
        Func<TFirst, TSecond, TResult> resultSelector)
    {
        this.first = first;
        this.secondSelector = secondSelector;
        this.resultSelector = resultSelector;
    }

    /// <summary>
    /// Parses a sequence of elements by combining two parsers.
    /// </summary>
    /// <param name="context">The context in which parsing is carried out.</param>
    /// <returns>A result of the combined parsing operation.</returns>
    public IParsingResult<TResult> Parse(ParsingContext context)
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