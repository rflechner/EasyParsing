namespace EasyParsing.Parsers;

public class SpacesParser(bool failIfNothingMatched) : ParserBase<string>
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

        if (index == 0)
        {
            return failIfNothingMatched 
                ? Fail(context, "No spaces matched") 
                : Success(context, string.Empty);
        }

        var spaces = span[.. index];

        return Success(context.Forward(index), spaces.ToString());
    }
}