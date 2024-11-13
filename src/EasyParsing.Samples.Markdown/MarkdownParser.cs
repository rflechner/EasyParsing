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

    private static readonly IParser<(string, Option<string>)> UrlAndTitleParser = (ManySatisfy(UrlValidChars) >> SkipSpaces()).Combine(QuotedTextParser.Optionnal());
    
    internal static IParser<Link> LinkParser =>
        from label in Between(OneChar('['), LettersDigitsOrSpacesParser, OneChar(']'))
        from urlAndTitle in Between(OneChar('('), UrlAndTitleParser, OneChar(')'))
        select new Link(label.Item, urlAndTitle.Item.Item1.Trim(), urlAndTitle.Item.Item2.GetValueOrDefault(string.Empty)!.Trim());
    
    
    internal static IParser<Image> ImageParser =>
        from _ in OneChar('!')
        from link in LinkParser
        select new Image(link.Text, link.Url, link.Title);
    
}