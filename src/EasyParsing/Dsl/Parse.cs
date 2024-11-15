using EasyParsing.Dsl.Linq;
using EasyParsing.Parsers;

namespace EasyParsing.Dsl;

public static partial class Parse
{
    public static IParser<string> StringPrefix(string text) => new StringPrefixParser(text);
    
    public static IParser<Option<T>> Optionnal<T>(this IParser<T> parser) => new OptionnalParser<T>(parser);
    
    public static IParser<string> SkipSpaces() => new SkipSpacesParser();
    
    public static IParser<string> OneChar(char c) => new OneCharParser(c).AsString();
    
    public static IParser<string> Satisfy(Func<char, bool> condition) => new SatisfyParser(condition).AsString();
    
    public static IParser<char> IsLetterOrDigit() => new SatisfyParser(char.IsLetterOrDigit);
    
    public static IParser<string> ManyLettersOrDigits() => new ManyParser<char>(IsLetterOrDigit()).AsString();
    
    public static IParser<string> Many(IParser<char> parser) => new ManyParser<char>(parser).AsString();
    
    public static IParser<string> ManySatisfy(Func<char, bool> condition) => new ManyParser<char>(new SatisfyParser(condition)).AsString();
    
    public static IParser<T[]> SeparatedBy<T>(this IParser<T> itemParser, IParser<string> separatorParser, bool matchTailingSeparator = false) 
        => new SeparatedByParser<T, string>(itemParser, separatorParser, matchTailingSeparator: matchTailingSeparator);

    public static IParser<string> ConsumeWhile(Func<ReadOnlyMemory<char>, bool> condition) => new WhileTextParser(condition);
    
    public static BetweenParser<TLeft, T3, TRight> Between<TLeft, T3, TRight>(IParser<TLeft> left, IParser<T3> items, IParser<TRight> right) => new(left, items, right);

    public static IParser<string> CreateStringParser(char quoteChar)
    {
        var contentParser = ConsumeWhile
        (
            match => match.Span.EndsWith($"\\{quoteChar}") || !match.Span.EndsWith(quoteChar.ToString())
        );
        var quote = OneChar(quoteChar);

        var parser = from start in quote 
            from str in contentParser >> quote 
            select str;
        
        return parser.Map(s => s.Replace($"\\{quoteChar}", $"{quoteChar}"));
    }
    
    public static IParser<string> QuotedTextParser  = CreateStringParser('\'') | CreateStringParser('"');
}