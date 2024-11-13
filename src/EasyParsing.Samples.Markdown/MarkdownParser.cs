using System.Reflection.Metadata.Ecma335;
using EasyParsing.Dsl;
using EasyParsing.Dsl.Linq;
using EasyParsing.Parsers;
using static EasyParsing.Dsl.Parse;

namespace EasyParsing.Samples.Markdown;

public class MarkdownParser
{
    internal static IParser<Title> TitleParser =>
        from tag in ManySatisfy(c => c == '#') >> SkipSpaces()
        from text in ManySatisfy(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))
        select new Title(tag.Length, text.Trim());

    static bool LettersDigitsOrSpaces(char c) => 
        char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || char.IsPunctuation(c);
    
    static bool UrlValidChars(char c) => 
        (LettersDigitsOrSpaces(c) || c == ':' || c == '/' || c == '=' || c == '&') 
            && c != '[' && c != ']' 
            && c != '(' && c != ')'
            && c != '"';
    
    private static readonly IParser<string> LettersDigitsOrSpacesParser = ManySatisfy(UrlValidChars);
    
    internal static IParser<Link> LinkParser
    {
        get
        {
            var urlAndTitleParser = (ManySatisfy(UrlValidChars) >> SkipSpaces()).Combine(QuotedTextParser.Optionnal());
            
            return 
                from label in Between(OneChar('['), LettersDigitsOrSpacesParser, OneChar(']'))
                from urlAndTitle in Between(OneChar('('), urlAndTitleParser, OneChar(')'))
                select new Link(label.Item, urlAndTitle.Item.Item1.Trim(), urlAndTitle.Item.Item2.GetValueOrDefault(string.Empty)!.Trim());
        }
    }
}