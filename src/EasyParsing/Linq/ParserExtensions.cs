namespace EasyParsing.Linq;

public static class ParserExtensions
{
    public static ParserBase<TResult> SelectMany<TFirst, TSecond, TResult>(
        this ParserBase<TFirst> first,
        Func<TFirst, ParserBase<TSecond>> secondSelector,
        Func<TFirst, TSecond, TResult> resultSelector)
    {
        return new SelectManyParser<TFirst, TSecond, TResult>(first, secondSelector, resultSelector);
    }
    
    public static ParserBase<TResult> Select<TSource, TResult>(
        this ParserBase<TSource> source,
        Func<TSource, TResult> selector)
    {
        return new SelectParser<TSource, TResult>(source, selector);
    }
    
}