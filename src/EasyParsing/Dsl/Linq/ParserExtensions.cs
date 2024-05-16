using EasyParsing.Parsers;

namespace EasyParsing.Dsl.Linq;

public static class ParserExtensions
{
    public static IParser<TResult> SelectMany<TFirst, TSecond, TResult>(
        this IParser<TFirst> first,
        Func<TFirst, IParser<TSecond>> secondSelector,
        Func<TFirst, TSecond, TResult> resultSelector)
    {
        return new SelectManyParser<TFirst, TSecond, TResult>(first, secondSelector, resultSelector);
    }
    
    public static IParser<TResult> Select<TSource, TResult>(
        this IParser<TSource> source,
        Func<TSource, TResult> selector)
    {
        return new SelectParser<TSource, TResult>(source, selector);
    }

    public static IParser<string> AsString(this IParser<char[]> source)
    {
        return source.Map(chars => new string(chars));
    }

    public static IParser<string> AsString(this IParser<char> source)
    {
        return source.Map(c => c.ToString());
    }
    
    
}