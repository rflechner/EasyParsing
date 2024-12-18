using EasyParsing.Dsl;
using EasyParsing.Dsl.Linq;
using EasyParsing.Markdown.Ast;
using EasyParsing.Markdown.Exceptions;
using EasyParsing.Parsers;
using static EasyParsing.Dsl.Parse;

namespace EasyParsing.Markdown;

/// <summary>
/// Markdown parser based on syntax described by GitHub.
/// </summary>
/// <remarks>
/// - https://docs.github.com/fr/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/basic-writing-and-formatting-syntax#styling-text
/// </remarks>
public static class MarkdownParser
{
    public static bool TryParseMarkdown(string markdown, out MarkdownAst[] markdownAsts)
    {
        var result = MarkdownSyntaxParser.Parse(markdown);

        if (!result.Success || result.Result is null)
        {
            markdownAsts = [];
            return false;
        }

        markdownAsts = result.Result;
        return true;
    }
    
    public static MarkdownAst[] ParseMarkdown(string markdown)
    {
        var result = MarkdownSyntaxParser.Parse(markdown);

        if (!result.Success || result.Result is null)
        {
            var errorMessage = result.FailureMessage ?? "No failure message provided.";
            throw new MarkdownParsingException($"Failed to parse markdown: {errorMessage}");
        }

        return result.Result;
    }

    internal static IParser<MarkdownAst[]> MarkdownSyntaxParser =>
        Many(
            TextObjectsParser
            | RawTextParser
            | AnyTextChar
        ).MergeRawTextParts();
    
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
    
    internal static IParser<MarkdownAst> LinkParser =>
        from label in Between(OneCharText('['), LettersDigitsOrSpacesParser, OneCharText(']'))
        from urlAndTitle in Between(OneCharText('('), UrlAndTitleParser, OneCharText(')'))
        select new Link(new RawText(label.Item), urlAndTitle.Item.Item1.Trim(), urlAndTitle.Item.Item2.GetValueOrDefault(string.Empty)!.Trim());
    
    internal static IParser<Image> ImageParser =>
        from _ in OneCharText('!')
        from label in Between(OneCharText('['), LettersDigitsOrSpacesParser, OneCharText(']'))
        from url in Between(OneCharText('('), UrlParser, OneCharText(')'))
        select new Image(label.Item, url.Item);

    internal static IParser<Strikethrough> StrikethroughParser =>
        from prefix in StringMatch("~~") 
        from text in MarkdownSyntaxParser.Until(prefix)
        select new Strikethrough(text);

    internal static IParser<Bold> BoldParser =>
        from prefix in StringMatch("**") | StringMatch("__") 
        from text in MarkdownSyntaxParser.Until(prefix)
        select new Bold(text);

    internal static IParser<Italic> ItalicParser =>
        from prefix in StringMatch("*") | StringMatch("_") 
        from text in MarkdownSyntaxParser.Until(StringMatch(prefix))
        select new Italic(text.Item1);
    
    internal static IParser<Title> TitleParser
    {
        get
        {
            var titleTextParser = Many(StyledTextParser | AnyTextChar).MergeRawTextParts();
            
            return from tag in ManySatisfy(c => c == '#') >> SkipSpaces()
                from text in titleTextParser
                select new Title(tag.Length, text);
        }
    }

    internal static IParser<MarkdownAst> RawTextParser =>
        TextObjectsParser
        | ManySatisfy(c => !char.IsWhiteSpace(c) && !char.IsPunctuation(c))
            .Select(MarkdownAst (s) => new RawText(s));

    internal static IParser<MarkdownAst> StyledTextParser =>
        LinkParser
        | BoldParser
        | ItalicParser
        | StrikethroughParser
        | InlineQuotingCodeParser
        ;

    internal static IParser<MarkdownAst> TextObjectsParser =>
        StyledTextParser
        | TitleParser
        | ImageParser
        | TaskListItemParser
        | ListParser
        | ParagraphStartParser
        | CrlfParser
        | QuotingCodeParser
        | QuotingTextParser
        ;

    internal static IParser<MarkdownAst> AnyTextChar => NotSatisfy(IsNewLine).Select(s => new RawText(s));
    
    internal static IParser<MarkdownAst> EntireLine => ManyExcept(IsNewLine).Select(s => new RawText(s));
    
    internal static IParser<QuotingText> QuotingTextParser =>
        from _ in OneCharText('>') >> SkipSpaces()
        from s in ManySatisfy(c => !IsNewLine(c))
        select new QuotingText(s);
    
    internal static IParser<InlineQuotingCode> InlineQuotingCodeParser =>
        Between(OneCharText('`'), ManySatisfy(c => !IsNewLine(c) && c != '`'), OneCharText('`'))
            .Select(r => new InlineQuotingCode(r.Item));
        
    internal static IParser<QuotingCode> QuotingCodeParser
    {
        get
        {
            IParser<string> langParser = StringMatch("```") << ManySatisfy(c => !char.IsWhiteSpace(c));
            IParser<string> noLangParser = StringMatch("```").Select(_ => string.Empty);
            
            return 
                from lang in langParser | noLangParser
                from _ in NewLine()
                from code in ManySatisfy(_ => true).Until("```")
                select new QuotingCode(code.TrimEnd('\r', '\n'), lang);
        }
    }
    
    internal static IParser<ListItem> ListItemParser
    {
        get
        {
            var bulletParser = 
                StringMatch("-") 
                | StringMatch("+") 
                | StringMatch("*")
                | Integer().Combine(OneCharText('.')).Select(r => $"{r.Item1}{r.Item2}");
            
            return 
                from spaces in InlineSpaces().Optionnal().DefaultWith(string.Empty)
                from bullet in bulletParser
                from separatorSpaces in InlineSpaces()
                from content in Many(StyledTextParser | EntireLine)
                from endOfLine in NewLine().Optionnal()
                select new ListItem(spaces.Length, bullet, content.ToArray(), []);
        }
    }
    
    internal static IParser<ListItems> ListParser => Many(ListItemParser).Select(AstProjectionsBuilder.BuildListTree);

    internal static IParser<TaskListItem> TaskListItemParser
    {
        get
        {
            var checkBoxContentParser = (OneChar(' ') | OneChar('x') | OneChar('X'))
                .Optionnal()
                .DefaultWith(' ')
                .Select(c => !char.IsWhiteSpace(c));
            
            return 
                from trimmedSpaces in SkipSpaces()
                from bullet in OneChar('-')
                from separator1 in InlineSpaces()
                from openBox in OneChar('[')
                from isChecked in checkBoxContentParser
                from closeBox in OneChar(']')
                from separator2 in InlineSpaces()
                from content in MarkdownSyntaxParser
                select new TaskListItem(isChecked, content);
        }
    }

    internal static IParser<MarkdownAst> ParagraphStartParser =>
        from spaces in Spaces()
        where spaces.Count(IsNewLine) >= 3
        select new ParagraphStart();

    internal static IParser<MarkdownAst> CrlfParser =>
        from cr in OneChar('\r').Optionnal().DefaultWith('\r')
        from lf in OneChar('\n')
        select new Crlf();
}