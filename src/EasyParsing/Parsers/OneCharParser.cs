namespace EasyParsing.Parsers;

/// <summary>
/// Match a char.
/// </summary>
public class OneCharParser : ParserBase<char>
{
    private readonly char c;

    /// <summary>
    /// Represents a parser that matches a specific char within an input.
    /// </summary>
    /// <param name="c">The char to match.</param>
    public OneCharParser(char c)
    {
        this.c = c;
    }

    /// <inheritdoc />
    public override IParsingResult<char> Parse(ParsingContext context)
    {
        var span = context.Remaining.Span;
        if (span.Length < 1)
            return Fail(context, "end of buffer");

        if (span[0] != c)
            return Fail(context, $"not expected char '{c}'");

        return Success(context.ForwardChar(c), c);
    }
}
