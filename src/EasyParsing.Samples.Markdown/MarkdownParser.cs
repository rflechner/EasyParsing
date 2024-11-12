using EasyParsing.Dsl.Linq;
using static EasyParsing.Dsl.Parse;

namespace EasyParsing.Samples.Markdown;

public class MarkdownParser
{
    internal static IParser<Title> TitleParser =>
        from tag in ManySatisfy(c => c == '#') >> SkipSpaces()
        from text in ManySatisfy(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))
        select new Title(tag.Length, text.Trim());


}