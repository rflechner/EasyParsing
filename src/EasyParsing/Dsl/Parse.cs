using EasyParsing.Dsl.Linq;
using EasyParsing.Parsers;

namespace EasyParsing.Dsl;

public static partial class Parse
{

    public static SkipSpacesParser SkipSpaces() => new();
    
    public static IParser<string> OneChar(char c) => new OneCharParser(c).AsString();
    
    public static IParser<string> Satisfy(Func<char, bool> condition) => new SatisfyParser(condition).AsString();
    
    public static IParser<char> IsLetterOrDigit() => new SatisfyParser(char.IsLetterOrDigit);
    
    public static IParser<string> ManyLettersOrDigits() => new ManyParser<char>(IsLetterOrDigit()).AsString();
    
    public static IParser<string> Many(IParser<char> parser) => new ManyParser<char>(parser).AsString();
    
    public static IParser<string> ManySatisfy(Func<char, bool> condition) => new ManyParser<char>(new SatisfyParser(condition)).AsString();
    
    public static IParser<T[]> SeparatedBy<T>(this IParser<T> itemParser, IParser<string> separatorParser, bool matchTailingSeparator = false) 
        => new SeparatedByParser<T, string>(itemParser, separatorParser, matchTailingSeparator: matchTailingSeparator);
    
    
    
}