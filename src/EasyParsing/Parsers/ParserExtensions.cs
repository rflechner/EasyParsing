namespace EasyParsing.Parsers;

public static class ParserExtensions
{
    public static CombineParser<TIn1, TIn2> Combine<TIn1, TIn2>(this IParser<TIn1> left, IParser<TIn2> right)
    {
        return new CombineParser<TIn1, TIn2>(left, right);
    }

    public static IParser<TIn1> ThenIgnore<TIn1, TIn2>(this IParser<TIn1> left, IParser<TIn2> right)
    {
        return new CombineParser<TIn1, TIn2>(left, right).Map(r => r.Item1);
    }
    
    public static IParser<TOut> Map<TIn, TOut>(this IParser<TIn> input, Func<TIn, TOut> mapper)
    {
        return new MapParser<TIn, TOut>(input, mapper);
    }
    
    public static IParser<TOut> Cast<TIn, TOut>(this IParser<TIn> input) where TIn : TOut
    {
        return new MapParser<TIn, TOut>(input, @in => (TOut) @in);
    }
    
    public static IParser<T> Until<T>(this IParser<T> parser, string text, bool skipDelimiter = true)
    {
        return new UntilTextParser<T>(parser, text, skipDelimiter);
    }

    public static IParser<(T1, T2)> Until<T1, T2>(this IParser<T1> innerParser, IParser<T2> limitParser) => new UntilParser<T1, T2>(innerParser, limitParser);

    public static IParser<T> Or<T>(this IParser<T> left, IParser<T> right)
    {
        return new OrElseParser<T>([left, right]);
    }
    
    public static IParser<T> Or<T>(this IParser<T> left, IParser<T>[] parsers)
    {
        return new OrElseParser<T>(parsers.Prepend(left).ToArray());
    }

}