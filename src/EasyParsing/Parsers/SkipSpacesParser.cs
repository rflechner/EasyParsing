namespace EasyParsing.Parsers;

public class SkipSpacesParser : ParserBase<string>
{
    public override ParsingResult<string> Parse(ParsingContext context)
    {
        var span = context.Remaining.Span;
        var index = 0;

        while (index < span.Length)
        {
            if (!char.IsWhiteSpace(span[index]))
                break;
            index++;
        }

        if (index == 0) return Success(context, string.Empty);

        var spaces = span[.. index];

        return Success(context.Forward(index), spaces.ToString());
    }
}