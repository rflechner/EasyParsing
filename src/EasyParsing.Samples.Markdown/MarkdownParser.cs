using EasyParsing.Dsl;
using EasyParsing.Dsl.Linq;
using EasyParsing.Parsers;
using static EasyParsing.Dsl.Parse;

namespace EasyParsing.Samples.Markdown;

/// <summary>
/// Markdown parser based on syntax described by Github.
/// </summary>
/// <remarks>
/// - https://docs.github.com/fr/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/basic-writing-and-formatting-syntax#styling-text
/// </remarks>
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
    
    private static readonly IParser<string> LettersDigitsOrSpacesParser = 
        ManySatisfy(UrlValidChars);

    private static readonly IParser<string> UrlParser =
        ManySatisfy(UrlValidChars) >> SkipSpaces();

    private static readonly IParser<(string, Option<string>)> UrlAndTitleParser = 
        UrlParser.Combine(QuotedTextParser.Optionnal());
    
    internal static IParser<Link> LinkParser =>
        from label in Between(OneChar('['), LettersDigitsOrSpacesParser, OneChar(']'))
        from urlAndTitle in Between(OneChar('('), UrlAndTitleParser, OneChar(')'))
        select new Link(new RawText(label.Item), urlAndTitle.Item.Item1.Trim(), urlAndTitle.Item.Item2.GetValueOrDefault(string.Empty)!.Trim());
    
    internal static IParser<Image> ImageParser =>
        from _ in OneChar('!')
        from label in Between(OneChar('['), LettersDigitsOrSpacesParser, OneChar(']'))
        from url in Between(OneChar('('), UrlParser, OneChar(')'))
        select new Image(label.Item, url.Item);

    internal static IParser<Bold> BoldParser =>
        from prefix in StringPrefix("**") | StringPrefix("__") 
        from text in RichTextParser.Until(prefix)
        select new Bold(text);

    internal static IParser<Italic> ItalicParser =>
        from prefix in StringPrefix("*") | StringPrefix("_") 
        from text in RichTextParser.Until(StringPrefix(prefix))
        select new Italic(text.Item1);

    internal static IParser<RawText> RawTextParser =>
        ManySatisfy(c => c != '\r' && c != '\n').Select(s => new RawText(s));
    
    internal static IParser<RichText[]> RichTextParser =>
        Many(
            LinkParser.Cast<Link, RichText>()
            | ImageParser.Cast<Image, RichText>()
            | BoldParser.Cast<Bold, RichText>()
            | ItalicParser.Cast<Italic, RichText>()
            | RawTextParser.Cast<RawText, RichText>()
        )
        ;

}