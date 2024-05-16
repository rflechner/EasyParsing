using EasyParsing.Dsl.Linq;
using EasyParsing.Parsers;

namespace EasyParsing.Dsl;

public static partial class Parse
{

    public static SkipSpacesParser SkipSpaces() => new();
    
    public static IParser<string> OneChar(char c) => new OneCharParser(c).AsString();
    
    public static IParser<string> Satisfy(Func<char, bool> condition) => new SatisfyParser(condition).AsString();
    
    public static IParser<char> IsLetterOrDigit() => new SatisfyParser(char.IsLetterOrDigit);
    
    public static IParser<string> ManyChars(IParser<char> f) => new ManyParser<char>(f).AsString();
    
    public static IParser<string> ManyLettersOrDigits() => new ManyParser<char>(IsLetterOrDigit()).AsString();
    
    
    
}