namespace EasyParsing.Parsers;

public static class ParserExtensions
{
    public static CombineParser<TIn1, TIn2> Combine<TIn1, TIn2>(this IParser<TIn1> left, IParser<TIn2> right)
    {
        return new CombineParser<TIn1, TIn2>(left, right);
    }
    
    public static IParser<TOut> Map<TIn, TOut>(this IParser<TIn> input, Func<TIn, TOut> mapper)
    {
        return new MapParser<TIn, TOut>(input, mapper);
    }
    
    public static IParser<string> Until<T>(this IParser<T> parser, string text, bool skipMatch = true)
    {
        return new UntilTextParser(text, skipMatch);
    }
    
    public static IParser<T> Or<T>(this IParser<T> left, IParser<T> right)
    {
        return new OrElseParser<T>([left, right]);
    }
    
    public static IParser<T> Or<T>(this IParser<T> left, IParser<T>[] parsers)
    {
        return new OrElseParser<T>(parsers.Prepend(left).ToArray());
    }

}