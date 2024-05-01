namespace EasyParsing.Parsers;

/// <summary>
/// Match a char.
/// </summary>
public class OneCharParser : ParserBase<char>
{
    private readonly char c;

    public OneCharParser(char c)
    {
        this.c = c;
    }

    public override ParsingResult<char> Parse(ParsingContext context)
    {
        var span = context.Remaining.Span;
        if (span.Length < 1)
            return Fail(context, "end of buffer");

        if (span[0] != c)
            return Fail(context, "not expected char");

        return Success(context.ForwardChar(c), c);
    }
}
