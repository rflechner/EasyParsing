using EasyParsing.Parsers;

namespace EasyParsing;

public interface IParser<T>
{
    ParsingResult<T> Parse(ParsingContext context);
    
    public static IParser<T> operator |(IParser<T> left, IParser<T> right)
    {
        return new OrElseParser<T>([left, right]);
    }

    public static IParser<(T,T)> operator +(IParser<T> left, IParser<T> right)
    {
        return new CombineParser<T, T>(left, right);
    }
    
}